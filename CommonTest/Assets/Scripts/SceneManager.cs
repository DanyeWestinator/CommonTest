using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using Mirror;
using Mirror.FizzySteam;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance;
    public GameObject FriendsListScrollView;
    public Text playerCountLabel;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    
    public void HostGame()
    {
        if (FriendsListScrollView.activeInHierarchy == false)
        {
            FriendsListScrollView.SetActive(true);
        }
        print("Hosted Game");
    }
    public void JoinGame()
    {
        SteamFriends.ActivateGameOverlayInviteDialogConnectString("Test this");
        print("Joined Game");
    }
}
