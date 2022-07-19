using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FriendListing : MonoBehaviour
{
    [field: SerializeField, Tooltip("Display name of friend"), Header("UI References")]
    public TMP_Text FriendDisplayNameReference { get; private set; } = default;

    [field: SerializeField, Tooltip("PlayFab ID of friend."), Header("User Data")]
    public string FriendPlayFabID { get; set; } = default;

    [field: SerializeField, Tooltip("Friends display name.")]
    public string FriendDisplayName { get; set; } = default;

    public void InviteFriendToParty()
    {
        // TODO
    }

    public void RemoveFriend()
    {

    }
}
