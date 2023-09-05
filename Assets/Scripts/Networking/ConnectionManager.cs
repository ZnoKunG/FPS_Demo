using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public int Port => port;

    [SerializeField]
    private int port = 80;

    [SerializeField]
    private Client client;

    private bool isStartedServer = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
        }
    }
}
