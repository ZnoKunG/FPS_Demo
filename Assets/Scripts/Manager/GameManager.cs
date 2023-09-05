using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    [SerializeField]
    private GameObject localPlayerPrefab;

    [SerializeField]
    public GameObject playerPrefab;

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

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {

    }

    public void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject player;

        if (id == Client.Instance.Id)
        {
            player = Instantiate(localPlayerPrefab, position, rotation);
        }
        else
        {
            player = Instantiate(playerPrefab, position, rotation);
        }

        player.GetComponent<PlayerManager>().Id = id;
        player.GetComponent<PlayerManager>().Username = username;
        players.Add(id, player.GetComponent<PlayerManager>());
    }

    public void StartGame()
    {
        ClientSend.SpawnPlayersInLobby();
    }
}
