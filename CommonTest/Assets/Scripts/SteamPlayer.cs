using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class SteamPlayer : NetworkBehaviour
{
    private static NetworkManagerTest nm;
    int playerNum;

    public override void OnStartClient()
    {
        if (nm == null)
            nm = NetworkManagerTest.instance;
        base.OnStartClient();
        if (isLocalPlayer)
        {
            playerNum = nm.numPlayers;
            print("started player " + playerNum);
        }
        
        gameObject.name = $"Player {playerNum}";
    }


    private void OnDestroy()
    {
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        
        
        
    }
}
