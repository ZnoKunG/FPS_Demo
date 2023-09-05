using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JoinLobbyUIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField createlobbyNameInputField;

    [SerializeField]
    private TMP_InputField joinlobbyCodeInputField;

    [SerializeField]
    private TextMeshProUGUI joinStatusText;

    [SerializeField]
    private GameObject lobbyMenu;

    private JoinStatus joinStatus;

    public static JoinLobbyUIManager Instance { get; private set; }

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

    private void OnEnable()
    {
        createlobbyNameInputField.text = "";
        joinlobbyCodeInputField.text = "";
    }

    public void CreateLobby()
    {
        ClientSend.CreateLobby(GetEnteredLobbyName());
    }

    public void JoinLobby()
    {
        UpdateJoinStatusText(JoinStatus.Waiting);
        ClientSend.RequestToJoinLobby(GetEnteredLobbyCode());
    }

    public void OnJoinLobbyFail(int joinClientId)
    {
        UpdateJoinStatusText(JoinStatus.Fail);
    }

    public void OnJoinLobbySuccess(int joinClientId)
    {
        UpdateJoinStatusText(JoinStatus.Success);
        lobbyMenu.SetActive(true);
        gameObject.SetActive(false);

    }

    private void UpdateJoinStatusText(JoinStatus joinStatus)
    {
        this.joinStatus = joinStatus;

        switch (joinStatus)
        {
            case JoinStatus.Success:
                joinStatusText.text = "Join Lobby success. Waiting to get into the lobby...";
                break;
            case JoinStatus.Waiting:
                joinStatusText.text = "Validating lobby code ...";
                break;
            case JoinStatus.Fail:
                joinStatusText.text = "Cannot join lobby. Correct the lobby code.";
                break;
        }
    }

    public string GetEnteredLobbyCode()
    {
        return joinlobbyCodeInputField.text;
    }

    public string GetEnteredLobbyName()
    {
        return createlobbyNameInputField.text;
    }
}

public enum JoinStatus
{
    Success,
    Waiting,
    Fail,
}
