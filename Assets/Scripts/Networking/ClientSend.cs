using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.Instance.Tcp.SendData(packet);
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            // Confirm player with correct id and username
            packet.Write(Client.Instance.Id);
            packet.Write(ConnectionUIManager.Instance.GetEnteredUsername());

            Debug.Log("Send back Welcome received message to server");
            SendTCPData(packet);
        }

    }

    public static void RequestToJoinLobby(string lobbyCode)
    {
        using (Packet packet = new Packet((int)ClientPackets.joinLobby))
        {
            packet.Write(Client.Instance.Id);
            packet.Write(lobbyCode);

            Debug.Log($"Try to join lobby with code {JoinLobbyUIManager.Instance.GetEnteredLobbyCode()}");
            SendTCPData(packet);
        }
    }

    public static void CreateLobby(string lobbyName)
    {
        using (Packet packet = new Packet((int)ClientPackets.createLobby))
        {
            packet.Write(Client.Instance.Id);

            if (lobbyName == "")
            {
                Debug.LogWarning("Lobby name cannot be empty.");
                return;
            }

            packet.Write(lobbyName);

            Debug.Log($"Try to create lobby name {JoinLobbyUIManager.Instance.GetEnteredLobbyName()}");
            SendTCPData(packet);
        }
    }

    public static void QuitLobby()
    {
        using (Packet packet = new Packet((int)ClientPackets.quitLobby)) 
        {
            packet.Write(Client.Instance.Id);

            SendTCPData(packet);
        }
    }

    public static void StartGameRequest()
    {

        using (Packet packet = new Packet((int)ClientPackets.startGameRequest))
        {
            packet.Write(Client.Instance.Id);

            SendTCPData(packet);
        }
    }
    #endregion
}
