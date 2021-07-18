using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class SteamLobby : MonoBehaviour
{
    [SerializeField]
    private GameObject startButton;

    private NetworkManagerTest netManager;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private const string hostAddressKey = "HostAddress";

    //Start is called before the first frame update
    private void Start()
    {
        if (SteamManager.Initialized == false)
        {
            Debug.LogError("Steam manager not initialized!");
            return;
        }
        netManager = GetComponent<NetworkManagerTest>();
        if (startButton == null)
        {
            startButton = GetComponentInChildren<UnityEngine.UI.Button>().gameObject;
        }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

    }

    public void HostLobby()
    {
        startButton.SetActive(false);

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 3);
    }

    void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            startButton.SetActive(true);
            return;
        }
        netManager.StartHost();
        //https://youtu.be/QlbBC07dqnE?t=644
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),
            hostAddressKey, 
            SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active)
            return;
        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            hostAddressKey);
        netManager.networkAddress = hostAddress;
        netManager.StartClient();
        startButton.SetActive(false);

    }
}
