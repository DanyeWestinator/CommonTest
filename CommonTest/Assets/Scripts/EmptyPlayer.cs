using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class EmptyPlayer : NetworkBehaviour
{
    CSteamID id;
    public GameObject playerChild;
    public GameObject startButton;

    [SyncVar(hook = nameof(SyncName))]
    string _name;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isLocalPlayer)
        {
            id = SteamUser.GetSteamID();
            _name = SteamFriends.GetPersonaName();
            gameObject.name = _name;
            playerChild.SetActive(false);
        }
        else
        {
            startButton.SetActive(false);
            playerChild.SetActive(true);
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
        print($"started empty player as local: {isLocalPlayer}");
    }
    public void StartGame()
    {
        startButton.SetActive(false);
        playerChild.SetActive(true);
        Camera.main.gameObject.SetActive(false);
        GetComponentInChildren<Camera>().gameObject.SetActive(true);
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            //SteamFriends
        }
    }

    void SyncName(string old, string name)
    {
        print("synced name");
        gameObject.name = name;
    }
}
