using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using Mirror.FizzySteam;

public class FriendsList : MonoBehaviour
{
    public EFriendFlags friendType;
    public List<CSteamID> friends { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        GetAllFriends();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetAllFriends()
    {
        if (friends == null)
            friends = new List<CSteamID>();
        int friendCount = SteamFriends.GetFriendCount(friendType);
        for (int i = 0; i < friendCount; i++)
        {
            CSteamID id = SteamFriends.GetFriendByIndex(i, friendType);
            friends.Add(id);
        }
    }
}
