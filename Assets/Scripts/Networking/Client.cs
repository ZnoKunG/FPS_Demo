using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Client: MonoBehaviour
{
    public string Ip;
    public int Id;
    public int Port;

    public TCP Tcp;


    public static int dataBufferSize = 4096; // 4MB

    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;
    private bool isConnected = false;

    public static Client Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Tcp = new TCP(Id);
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }
    private void Disconnect()
    {
        if (!isConnected) return;

        isConnected = false;
        Tcp.Disconnect();
        Debug.Log("Disconnected");
    }

    public void ConnectToServer()
    {
        isConnected = true;
        InitializeClientData();
        Tcp.Connect();
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.createLobbySuccess, ClientHandle.CreateLobbySuccess },
            { (int)ServerPackets.updateLobby, ClientHandle.UpdateLobby },
            { (int)ServerPackets.joinLobbySuccess, ClientHandle.JoinLobbySuccess },
            { (int)ServerPackets.quitLobbySuccess, ClientHandle.QuitLobbySuccess },
            { (int)ServerPackets.forceQuitLobby, ClientHandle.ForceQuitLobby },
        };

        Debug.Log("Initialize Client packets");
    }

    public class TCP
    {
        public TcpClient Socket { get; private set; }

        private readonly int id;
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public TCP(int _id)
        {
            id = _id;
        }

        public void Connect()
        {
            Socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize,
            };

            receiveBuffer = new byte[dataBufferSize];

            Socket.BeginConnect(Instance.Ip, Instance.Port, ConnectCallback, Socket);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (Socket == null) return;

                stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);

            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error on Sending data to server: {e}");
            }
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            Socket.EndConnect(_result);

            if (!Socket.Connected)
            {
                Debug.LogWarning("Cannot connect to the server");
            }

            stream = Socket.GetStream();

            receivedData = new Packet();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int byteLength = stream.EndRead(_result);

                if (byteLength <= 0)
                {
                    Instance.Disconnect();
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                // TODO: Handle data (Difficult Part I think)
                receivedData.Reset(HandleData(data));
                
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch(Exception e)
            {
                Debug.LogWarning($"Error receiving TCP data: {e}");
                Instance.Disconnect();
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLength = 0;

            receivedData.SetBytes(data);

            if (receivedData.UnreadLength() >= 4)
            {
                packetLength = receivedData.ReadInt();

                if (packetLength <= 0)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
            {
                byte[] packetBytes = receivedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        packetHandlers[packetId](packet);
                    }
                });

                packetLength = 0;

                if (receivedData.UnreadLength() >= 4)
                {
                    packetLength = receivedData.ReadInt();

                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            // Still have some bytes waiting to be sent
            if (packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        public void Disconnect()
        {
            Instance.Disconnect();
            Socket.Close();
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            Socket = null;
        }
    }

    public class UDP
    {
        public UdpClient Socket;
        public IPEndPoint IpEndPoint;

        public UDP()
        {
            IpEndPoint = new IPEndPoint(IPAddress.Parse(Instance.Ip), Instance.Port);
        }

        public void Connect(int localPort)
        {
            Socket = new UdpClient(localPort);

            Socket.Connect(IpEndPoint);
            Socket.BeginReceive(ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                byte[] data = Socket.EndReceive(result, ref IpEndPoint);
                Socket.BeginReceive(ReceiveCallback, null);

                if (data.Length < 4)
                {
                    Disconnect();
                    return;
                }
            }
            catch (Exception ex) 
            {
                Debug.LogWarning($"Fail to receive data: {ex}");
            }
        }

        private void Disconnect() 
        {
            Debug.LogWarning($"Disconnect");
        }
    }

}
