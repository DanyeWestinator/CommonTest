using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerTest : NetworkManager
{
    public Transform startPositionsParent;
    public HashSet<Transform> usedStarts = new HashSet<Transform>();
    public List<Material> colors = new List<Material>();
    Dictionary<GameObject, Material> players = new Dictionary<GameObject, Material>();
    public static NetworkManagerTest instance;
    private new void Start()
    {
        base.Start();
        for (var i = 0; i < startPositionsParent.childCount; i++)
        {
            startPositions.Add(startPositionsParent.GetChild(i));
        }
        instance = this;
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
        print("adding player");

    }
    public override void OnStartClient()
    {
        
    }
}
