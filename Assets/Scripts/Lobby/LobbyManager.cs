using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public int MaxPlayers => maxPlayers;

    private int currentPlayerCount;

    private int maxPlayers;

    public string lobbyName;

    private bool isHost;

    private string lobbyCode;

    [SerializeField]
    private GameObject lobbyUI;

    [SerializeField]
    private TextMeshProUGUI lobbyNameText;

    [SerializeField]
    private TextMeshProUGUI lobbyCodeText;

    [SerializeField]
    private TextMeshProUGUI player1Name;

    [SerializeField]
    private TextMeshProUGUI player2Name;

    [SerializeField]
    private GameObject joinUI;

    public static LobbyManager Instance;

    public bool IsReadyToStart => isReadyToStart;
    private bool isReadyToStart;
    private bool isInLobby;

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

    public void CreateLobbyAsHost(string lobbyName, string lobbyCode, int currentPlayerCount, int maxPlayers, string hostUsername)
    {
        isInLobby = true;
        this.lobbyCode = lobbyCode;
        this.lobbyName = lobbyName;
        this.currentPlayerCount = currentPlayerCount;
        this.maxPlayers = maxPlayers;

        SetUIProperties();
        joinUI.SetActive(false);
        lobbyUI.SetActive(true);
        Debug.Log($"Create lobby {lobbyName} as a host");
        player1Name.text = $"Player 1: {hostUsername}";
        player2Name.text = $"Player 2: -";
    }

    public void CreateLobbyAsMember(string lobbyName, string lobbyCode, int currentPlayerCount, int maxPlayers, List<string> memberNames)
    {
        isInLobby = true;
        isReadyToStart = true;
        this.lobbyCode = lobbyCode;
        this.lobbyName = lobbyName;
        this.currentPlayerCount = currentPlayerCount;
        this.maxPlayers = maxPlayers;

        player1Name.text = $"Player 1: {memberNames[0]}";
        player2Name.text = $"Player 2: {memberNames[1]}";
        Debug.Log(player1Name.text);
        Debug.Log(player2Name.text);

        SetUIProperties();
        joinUI.SetActive(false);
        lobbyUI.SetActive(true);
        Debug.Log($"Create lobby {lobbyName} as a member");
    }

    public void SetUIProperties()
    {
        lobbyNameText.text = lobbyName;
        lobbyCodeText.text = lobbyCode;
    }

    public void UpdateLobby(string joinClientName)
    {
        isReadyToStart = true;
        Debug.Log($"{joinClientName} has joined the lobby {lobbyName}");
        player2Name.text = $"Player 2: {joinClientName}";
    }

    public void TryToQuitLobby()
    {
        ClientSend.QuitLobby();
    }

    public void LeaveLobby()
    {
        isInLobby = false;
        isReadyToStart = false;
        lobbyUI.SetActive(false);
        joinUI.SetActive(true);
    }

    public void RemovePlayerFromLobby()
    {
        player2Name.text = "Player 2: -";
    }

    public void StartGame()
    {
        if (!LobbyManager.Instance.IsReadyToStart)
        {
            Debug.Log("Member is not enough (require 2 players)");
            return;
        }

        Debug.Log("Try to start the game...");
        ClientSend.StartGameRequest();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
