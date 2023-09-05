using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ClientHandle: MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string message = packet.ReadString();
        int clientId = packet.ReadInt();

        Debug.Log($"Message from the server: {message}");
        Client.Instance.Id = clientId;
        ClientSend.WelcomeReceived();
    }

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        int score = packet.ReadInt();

        GameManager.Instance.SpawnPlayer(id, username, score);
    }

    #region Lobby
    public static void CreateLobbySuccess(Packet packet)
    {
        int id = packet.ReadInt();
        bool isSuccess = packet.ReadBool();

        if (!isSuccess)
        {
            Debug.LogWarning($"Cannot create lobby");
            return;
        }

        string lobbyName = packet.ReadString();
        string lobbyCode = packet.ReadString();
        int maxPlayers = packet.ReadInt();
        int currentPlayerCount = packet.ReadInt();
        string hostUsername = packet.ReadString();

        LobbyManager.Instance.CreateLobbyAsHost(lobbyName, lobbyCode, currentPlayerCount, maxPlayers, hostUsername);
    }

    public static void QuitLobbySuccess(Packet packet)
    {
        int id = packet.ReadInt();
        int quitClientId = packet.ReadInt();

        LobbyManager.Instance.LeaveLobby();
    }

    public static void JoinLobbySuccess(Packet packet)
    {
        int id = packet.ReadInt();
        bool canJoin = packet.ReadBool();

        if (!canJoin) JoinLobbyUIManager.Instance.OnJoinLobbyFail(id);

        string lobbyName = packet.ReadString();
        string lobbyCode = packet.ReadString();
        int maxPlayers = packet.ReadInt();
        int currentPlayerCount = packet.ReadInt();
        List<string> memberNames = new List<string>();

        for (int i = 0; i < currentPlayerCount; i++)
        {
            string memberName = packet.ReadString();
            memberNames.Add(memberName);
        }

        JoinLobbyUIManager.Instance.OnJoinLobbySuccess(id);
        LobbyManager.Instance.CreateLobbyAsMember(lobbyName, lobbyCode, currentPlayerCount, maxPlayers, memberNames);
    }

    public static void UpdateLobby(Packet packet)
    {
        int id = packet.ReadInt();
        int joinClientId = packet.ReadInt();
        string joinClientName = packet.ReadString();
        bool isAddToLobby = packet.ReadBool();

        if (isAddToLobby)
        {
            LobbyManager.Instance.UpdateLobby(joinClientName);
        }
        else
        {
            LobbyManager.Instance.RemovePlayerFromLobby();
        }

    }

    public static void ForceQuitLobby(Packet packet)
    {
        int id = packet.ReadInt();

        LobbyManager.Instance.LeaveLobby();
    }
    #endregion

    #region Gameplay
    public static void StartGame(Packet packet)
    {
        int id = packet.ReadInt();

        GameManager.Instance.StartGame();
    }
    #endregion
}
