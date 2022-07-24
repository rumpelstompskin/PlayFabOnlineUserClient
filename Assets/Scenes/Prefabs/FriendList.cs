using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendList : MonoBehaviour
{
    [field: SerializeField, Tooltip("Content GameObject reference."), Header("UI References")]
    public GameObject ContentGO { get; private set; } = default;

    [field: SerializeField, Tooltip("Friend Listing Prefab reference"), Header("Prefabs")]
    public GameObject FriendListingPrefab { get; private set; } = default;

    public HashSet<UserData> OnlineFriendsListing = new HashSet<UserData>();

    private void OnEnable()
    {
        Globals.OnFriendListUpdated += OnFriendListUpdated;
    }

    private void OnDisable()
    {
        Globals.OnFriendListUpdated -= OnFriendListUpdated;
    }

    public IEnumerator OnFriendListUpdated()
    {
        foreach(var onlineFriend in PlayFabSample.Instance.CurrentlyOnlineFriendsUserData)
        {
            if (OnlineFriendsListing.Contains(onlineFriend))
            {
                continue;
            }
            OnlineFriendsListing.Add(onlineFriend);
            GameObject listing = Instantiate(FriendListingPrefab, ContentGO.transform);
            if(listing.TryGetComponent(out FriendListing friendListing))
            {
                friendListing.FriendPlayFabID = onlineFriend.ID;
                friendListing.FriendDisplayName = onlineFriend.Name; // TODO Call PlayFab API to retrieve user's display name
                friendListing.FriendDisplayNameReference.text = onlineFriend.Name; // TODO
            }
        }
        yield return null;
    }
}
