using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public GameObject localPlayerPrefab;
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
    }

    void Update()
    {

    }

    public void SpawnPlayer(int id, string username, int score)
    {
        GameObject player;

        if (id == Client.Instance.Id)
        {
            player = Instantiate(localPlayerPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }

        player.GetComponent<PlayerManager>().Id = id;
        player.GetComponent<PlayerManager>().Username = username;
        players.Add(id, player.GetComponent<PlayerManager>());
    }

    public void StartGame()
    {

    }
}
