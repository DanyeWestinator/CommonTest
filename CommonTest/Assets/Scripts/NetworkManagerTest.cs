using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.FizzySteam;
using kcp2k;
using Steamworks;

public enum TransportType
{
    kcp,
    steam
}

public class NetworkManagerTest : NetworkManager
{
    public Transform startPositionsParent;
    public HashSet<Transform> usedStarts = new HashSet<Transform>();
    public List<Material> colors = new List<Material>();
    Dictionary<GameObject, Material> players = new Dictionary<GameObject, Material>();
    public static NetworkManagerTest instance;

    public bool mirrorDebug;
    public TransportType transportType;

    private new void Awake()
    {
        instance = this;
        print("instance is this");
        //KCP variables
        NetworkManagerHUD hud = GetComponent<NetworkManagerHUD>();
        KcpTransport kcp = GetComponent<KcpTransport>();

        //steamworks variables
        SteamManager sm = GetComponent<SteamManager>();
        SteamLobby lobby = GetComponent<SteamLobby>();
        FizzySteamworks fizzy = GetComponent<FizzySteamworks>();

        bool steam = (transportType == TransportType.steam);

        //set the kcp to the inverse of steam
        hud.enabled = !steam;
        kcp.enabled = !steam;

        sm.enabled = steam;
        lobby.enabled = steam;
        fizzy.enabled = steam;

        //disables the steam button if using KCP
        GetComponentInChildren<Canvas>().gameObject.SetActive(steam);
        if (steam)
            transport = fizzy;
        else
            transport = kcp;
        base.Awake();

    }
    private new void Start()
    {
        base.Start();
        for (var i = 0; i < startPositionsParent.childCount; i++)
        {
            startPositions.Add(startPositionsParent.GetChild(i));
        }
        
    }
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        var start = GetStartPosition();
        while (usedStarts.Contains(start) == true)
        {
            start = GetStartPosition();
        }
        usedStarts.Add(start);
        var player = Instantiate(playerPrefab, start.position, start.rotation);
        
        NetworkServer.AddPlayerForConnection(conn, player);
        print("added player");
    }
    public override void OnStartHost()
    {
        base.OnStartHost();
        SteamFriends.SetRichPresence("status", "Hosting a Common'Test game");
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        SteamFriends.SetRichPresence("status", "Playing a Common'Test lobby");
        SteamFriends.SetRichPresence("steam_display", "Testing some things in Common'Test!");
    }


}
