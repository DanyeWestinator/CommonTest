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

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isLocalPlayer)
        {
            id = SteamUser.GetSteamID();
            string name = SteamFriends.GetPersonaName();
            gameObject.name = name;
        }
        playerChild.SetActive(false);
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

        }
    }
}
