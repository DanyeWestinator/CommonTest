using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using Mirror.FizzySteam;
using UnityEngine.UI;

public class EmptyPlayerController : NetworkBehaviour
{
    /// <summary>
    /// The text component for the global console, that is reflected across all clients
    /// </summary>
    public Text globalConsole;

    [SyncVar(hook = nameof(SyncGlobalConsole))]
    string globalConsoleString = "";

    private static NetworkManagerTest nm;
    // Start is called before the first frame update
    void Start()
    {
        if (nm == null)
            nm = NetworkManagerTest.instance;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string name = SteamFriends.GetPersonaName();
        globalConsole = SteamLobby.instance.globalConsole;
        if (isLocalPlayer)
        {
            CmdWriteToConsole($"{name} joined");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command]
    private void CmdWriteToConsole(string console)
    {
        globalConsoleString = console;
    }
    private void SyncGlobalConsole(string old, string console)
    {
        globalConsole.text += console;
        globalConsole.text += $": {Time.time}\n";
    }
}
