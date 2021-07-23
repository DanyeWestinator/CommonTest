using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.FizzySteam;
using kcp2k;

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
        //KCP variables
        NetworkManagerHUD hud = GetComponent<NetworkManagerHUD>();
        KcpTransport kcp = GetComponent<KcpTransport>();

        //steamworks variables
        SteamManager sm = GetComponent<SteamManager>();
        SteamLobby lobby = GetComponent<SteamLobby>();
        FizzySteamworks fizzy = GetComponent<FizzySteamworks>();

        bool steam = (transportType == TransportType.steam);

        //set the kcp to the inverse of steam
        if (hud != null && kcp != null)
        {
            hud.enabled = !steam;
            kcp.enabled = !steam;
        }
        if (sm != null && lobby != null && fizzy != null)
        {
            sm.enabled = steam;
            lobby.enabled = steam;
            fizzy.enabled = steam;
        }
        

        

        //disables the steam button if using KCP
        //GetComponentInChildren<Canvas>().gameObject.SetActive(steam);
        if (steam && fizzy != null)
            transport = fizzy;
        else
            transport = kcp;
        base.Awake();

    }
    private new void Start()
    {
        base.Start();
        
    }
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        var start = GetStartPosition();
        while (usedStarts.Contains(start) == true)
        {
            start = GetStartPosition();
        }
        usedStarts.Add(start);
        GameObject player;
        try
        {
            player = Instantiate(playerPrefab, start.position, start.rotation);
        }
        catch (System.NullReferenceException e)
        {
            player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }
        SteamLobby.WriteToConsole("Added player to server");
        NetworkServer.AddPlayerForConnection(conn, player);
    }
    public override void OnStartClient()
    {
        
    }
    private new void OnDestroy()
    {
        Debug.Log(UnityEngine.StackTraceUtility.ExtractStackTrace().ToString());
        base.OnDestroy();
    }
}
