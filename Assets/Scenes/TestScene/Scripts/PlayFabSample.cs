using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UserData
{
    public string Name;
    public string ID;

    public UserData(string id)
    {
        ID = id;
        Name = null;
    }
    public UserData(string id, string name)
    {
        Name = name;
        ID = id;
    }
}

public class PlayFabSample : MonoBehaviour
{
    public static PlayFabSample Instance;

    [field: SerializeField, Tooltip("PlayFab Display Name")]
    public string PlayFabDisplayName { get; set; } = default;

    [field: SerializeField, Tooltip("PlayFab ID")]
    public string PlayFabID { get; set; } = default;

    [field: SerializeField, Tooltip("PlayFab current Network ID")]
    public string PlayFabNetworkID { get; set; } = default;

    [field: SerializeField, Tooltip("Server Name Ie.: service.metagamez.net")]
    public string ServerName { get; set; } = default;

    /// <summary>
    /// List containing all user's friends data.
    /// </summary>
    public List<UserData> FriendsUserData { get; set; } = new List<UserData>();
    /// <summary>
    /// List containing all online friends user data.
    /// </summary>
    public List<UserData> CurrentlyOnlineFriendsUserData { get; set; } = new List<UserData>();
    /// <summary>
    /// List containing user data from friendship requesting users.
    /// </summary>
    public List<UserData> RequestingFriendShipUserData { get; set; } = new List<UserData>();

    [field: SerializeField, Tooltip("List of all the friends of the user")]
    public List<string> FriendsPlayFabIDs { get; set; } = new List<string>();

    [field: SerializeField, Tooltip("List of all online friends of the user")]
    public List<string> OnlineFriends { get; set; } = new List<string>();

    [field: SerializeField, Tooltip("List of Friend Request by PlayFabID")]
    public List<string> FriendRequestPlayFabIDs { get; set; } = new List<string>();

    
    private void Awake()
    {
        if(Instance == null) { Instance = this; } else if (Instance != this) { Destroy(this); }
    }
    
}
