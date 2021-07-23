using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        print("Joined Game");
    }
}
