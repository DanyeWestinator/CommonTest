using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
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
        smallSprite = FriendManager.SpriteFromID(ID, imageSize.small);
        mediumSprite = FriendManager.SpriteFromID(ID, imageSize.medium);
        largeSprite = FriendManager.SpriteFromID(ID, imageSize.large);
    }
    private readonly CSteamID _id;

    public CSteamID id { get { return _id; }  }
    public string name { get; }


    public Sprite smallSprite { get; }
    public Sprite mediumSprite { get; }
    public Sprite largeSprite { get; }

   
}

public class FriendManager : MonoBehaviour
{
    public SteamFriend currentFriend;
    public GameObject baseGUI;
    public Transform contentParent;
    public EFriendFlags friendType = EFriendFlags.k_EFriendFlagAll;

    private static List<SteamFriend> friends = new List<SteamFriend>();
    public int friendCount;

    // Start is called before the first frame update
    void Start()
    {
        if (friends.Count == 0)
        {
            GetFriendList(friendType);
            foreach (SteamFriend friend in friends)
            {
                GUIfromFriend(friend);
            }
            
        }
        friendCount = friends.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void GetFriendList(EFriendFlags flag = EFriendFlags.k_EFriendFlagAll)
    {
        int friendCount = SteamFriends.GetFriendCount(flag);
        for (int i = 0; i < friendCount; i++)
        {
            CSteamID id = SteamFriends.GetFriendByIndex(i, flag);
            friends.Add(new SteamFriend(id));
        }

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

    public void GUIfromFriend(SteamFriend friend)
    {
        GameObject go = Instantiate(baseGUI);
        //sets the name text component
        Text name = go.transform.GetChild(0).GetComponent<Text>();
        name.text = friend.name;

        //sets the sprite
        Image background = go.transform.GetChild(1).GetComponent<Image>();
        background.sprite = friend.largeSprite;
        //you have to flip it, for some reason it is upside down
        Vector3 scale = background.transform.localScale;
        scale.y *= -1f;
        background.transform.localScale = scale;

        go.GetComponentInChildren<FriendManager>().currentFriend = friend;

        //assigns the parent to be the content rect
        go.transform.parent = contentParent;
    }
    public void InviteToGame()
    {
        SteamFriends.InviteUserToGame(currentFriend.id, "Join game?");
        print($"Invited {currentFriend.name} to game");
    }
}
