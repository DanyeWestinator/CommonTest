using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using Mirror.FizzySteam;
using UnityEngine.UI;

public enum imageSize
{
    small,
    medium,
    large
}
/// <summary>
/// A struct containing all useful info about their profile
/// </summary>
public readonly struct SteamFriend
{
    public SteamFriend(CSteamID ID)
    {
        _id = ID;
        name = SteamFriends.GetFriendPersonaName(ID);
        smallSprite = FriendsList.SpriteFromID(ID, imageSize.small);
        mediumSprite = FriendsList.SpriteFromID(ID, imageSize.medium);
        largeSprite = FriendsList.SpriteFromID(ID, imageSize.large);
    }
    private readonly CSteamID _id;

    public CSteamID id { get { return _id; } }
    public string name { get; }


    public Sprite smallSprite { get; }
    public Sprite mediumSprite { get; }
    public Sprite largeSprite { get; }

    public string CurrentGame()
    {
        string game = "";
        FriendGameInfo_t gameInfo;
        bool inGame = SteamFriends.GetFriendGamePlayed(_id, out gameInfo);
        if (inGame == false)
        {
            return game;
        }
        string gameID = gameInfo.m_gameID.ToString();
        if (gameID == "1661660")
        {
            game = "Common'Hood Demo";
        }
        else if (gameID == "962090")
        {
            game = "Common'Hood";
        }
        else if (gameID == "480")
        {
            game = "Spacewar!";
        }
        else
        {
            game = gameID;
        }

        return game;
    }


}
public class FriendsList : MonoBehaviour
{
    public string GameName = "Spacewar!";
    public Transform contentParent;
    public GameObject friendGUI;
    public EFriendFlags friendType;
    public List<SteamFriend> friends { get; private set; }
    public SteamFriend currentFriend;
    // Start is called before the first frame update
    void Start()
    {
        GetAllFriends(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Generates a list of all your steam friends that fit the flag friendType
    /// </summary>
    /// <param name="generateGUI">whether or not to generate and add a GUI element to the scroll view</param>
    void GetAllFriends(bool generateGUI = false)
    {
        if (friends == null)
            friends = new List<SteamFriend>();
        int friendCount = SteamFriends.GetFriendCount(friendType);
        for (int i = 0; i < friendCount; i++)
        {
            CSteamID id = SteamFriends.GetFriendByIndex(i, friendType);
            SteamFriend friend = new SteamFriend(id);
            friends.Add(friend);
            if (generateGUI)
            {
                GUIfromFriend(ref friend);
            }
        }
    }

    /// <summary>
    /// Generates the GUI element for the scroll view for a given friend
    /// </summary>
    /// <param name="friend">The SteamFriend struct instance for that friend, passed by reference because all fields are read-only</param>
    void GUIfromFriend(ref SteamFriend friend)
    {
        GameObject gui = Instantiate(friendGUI, contentParent);
        gui.name = friend.name;
        Text name = gui.transform.Find("name").GetComponent<Text>();
        name.text = friend.name;
        Image profilePic = gui.transform.Find("profilePic").GetComponent<Image>();
        profilePic.sprite = friend.largeSprite;
        Text gameName = gui.transform.Find("gameName").GetComponent<Text>();
        Button invite = gui.transform.Find("inviteButton").GetComponent<Button>();
        string currentGame = friend.CurrentGame();
        gui.GetComponentInChildren<FriendsList>().currentFriend = friend;
        //run this when the user is currently in the same game as the app
        if (currentGame == GameName)
        {
            invite.interactable = true;
            gui.transform.SetAsFirstSibling();
        }

        if (currentGame == "")
            currentGame = "Not currently in game";
        else
            currentGame = "Currently in:\n\t" + currentGame;
        gameName.text = currentGame;

    }

    public void InviteToGame()
    {
        SteamFriends.InviteUserToGame(currentFriend.id, "Invite to game");
        print($"Invited {currentFriend.name} to game");
    }

    /// <summary>
    /// Gets the users profile picture as a sprite
    /// </summary>
    /// <param name="id"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static Sprite SpriteFromID(CSteamID id, imageSize size = imageSize.large)
    {
        int rawImage = -1;
        if (size == imageSize.large)
            rawImage = SteamFriends.GetLargeFriendAvatar(id);
        else if (size == imageSize.medium)
            rawImage = SteamFriends.GetMediumFriendAvatar(id);
        else if (size == imageSize.small)
            rawImage = SteamFriends.GetSmallFriendAvatar(id);
        Texture2D texture = null;
        uint ImageWidth;
        uint ImageHeight;
        bool bIsValid = SteamUtils.GetImageSize(rawImage, out ImageWidth, out ImageHeight);
        Rect dimensions = new Rect(0, 0, ImageWidth, ImageHeight);
        if (bIsValid)
        {
            byte[] Image = new byte[ImageWidth * ImageHeight * 4];

            bIsValid = SteamUtils.GetImageRGBA(rawImage, Image, (int)(ImageWidth * ImageHeight * 4));
            if (bIsValid)
            {
                texture = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(Image);
                texture.Apply();
            }
        }

        Sprite sprite = Sprite.Create(texture, dimensions, Vector2.zero);

        return sprite;
    }
}
