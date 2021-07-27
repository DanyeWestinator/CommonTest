using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
using UnityEngine.UI;


public class SteamLobby : MonoBehaviour
{
    [Header("UI Elements")]
    #region
    
    
    public GameObject startButton;
    /// <summary>
    /// The parent of the scroll view, that all Base GUI objects are childed to
    /// </summary>
    public Transform scrollviewParent;
    /// <summary>
    /// The base steam friend GUI element
    /// </summary>
    public GameObject baseGUI;
    /// <summary>
    /// the Text component that displays the current player count
    /// </summary>
    public Text playerCountLabel;
    /// <summary>
    /// the parent gameobject of the scroll view
    /// </summary>
    public GameObject FriendsListScrollView;

    /// <summary>
    /// the parent GameObject of the player count slider 
    /// </summary>
    public GameObject playerCountSlider;

    /// <summary>
    /// the text component for displaying messages ingame
    /// </summary>
    public Text inGameConsole;

    /// <summary>
    /// The text component for the global console, that is reflected across all clients
    /// </summary>
    public Text globalConsole;

    #endregion

    #region
    [Header("Internal variables")]
    [SerializeField]
    private NetworkManagerTest netManager;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<LobbyInvite_t> lobbyInvited;

    
    #endregion

    //Internal variables
    public static SteamLobby instance;
    int playerCount = 1;

    

    int friendCount;

    private const string hostAddressKey = "HostAddress";


    //Start is called before the first frame update
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            print($"Destroying {gameObject.name}");
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        if (SteamManager.Initialized == false)
        {
            Debug.LogError("Steam manager not initialized!");
            return;
        }
        //netManager = GetComponent<NetworkManagerTest>();
        

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyInvited = Callback<LobbyInvite_t>.Create(OnLobbyInvited);
        friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);

        //initialize the player count slider variables
        playerCount = (int)playerCountSlider.GetComponentInChildren<Slider>().value;
        SetPlayerCountText(playerCount);
    }

    private void Update()
    {
        
    }

    public void SetPlayerCountText(float f)
    {
        playerCountLabel.text = "Player Count: " + f.ToString();
        playerCount = (int)f;
    }

    /// <summary>
    /// Starts a steam lobby
    /// </summary>
    public void HostLobby()
    {
        startButton.SetActive(false);
        if (FriendsListScrollView.activeInHierarchy == false)
        {
            FriendsListScrollView.SetActive(true);
        }
        playerCountSlider.SetActive(false);
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, playerCount);
        WriteToConsole("Hosted lobby");

    }

    public static void WriteToConsole(string s)
    {
        Text console = instance.inGameConsole;
        if (console.text != "")
            console.text += "\n";
        s += $" {Time.realtimeSinceStartup}";
        instance.inGameConsole.text += s;
    }

    

    void OnLobbyInvited(LobbyInvite_t callback)
    {
        WriteToConsole($"Invited to lobby by {callback.m_ulSteamIDUser}");
    }

    void OnLobbyCreated(LobbyCreated_t callback)
    {
        WriteToConsole("Started to create lobby");
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
        WriteToConsole("Lobby created");
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        WriteToConsole("Requested to join lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active)
            return;
        WriteToConsole("Started entering lobby");
        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            hostAddressKey);
        netManager.networkAddress = hostAddress;
        //netManager.StartClient();
        startButton.SetActive(false);
        WriteToConsole("Lobby entered");

    }
}
