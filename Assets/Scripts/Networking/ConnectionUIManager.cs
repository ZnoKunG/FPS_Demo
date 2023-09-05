using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectionUIManager : MonoBehaviour
{
    public static ConnectionUIManager Instance { get; private set; }

    [SerializeField]
    private GameObject startMenu;

    [SerializeField]
    private TMP_InputField usernameField;

    [SerializeField]
    private TMP_InputField idField;

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

    public string GetEnteredUsername()
    {
        return usernameField.text;
    }

    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false;
        idField.interactable = false;
        Client.Instance.Id = int.Parse(idField.text);

        if (Client.Instance == null) 
        {
            Debug.LogWarning("Do not forget to create ClientManager with Client script attached to it!");
            return;
        }

        Client.Instance.ConnectToServer();
        Debug.Log($"Hi! Client {usernameField.text} is connected to the server");
    }
}
