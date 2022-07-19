using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FriendRequestPanel : MonoBehaviour
{
    [field: SerializeField, Tooltip("TMPro Display name of user"), Header("UI Elements")]
    public TMP_Text UserDisplayName { get; set; } = default;

    [field: SerializeField, Tooltip("PlayFab ID of requesting user"), Header("Requesting user data")]
    public string RequestingPlayFabID { get; set; } = default;

    [field: SerializeField, Tooltip("Display name of requesting user")]
    public string RequestingUserDisplayName { get; set; } = default;

    public void AcceptFriendRequest()
    {
        ClientSend.Instance.ClientSendFriendResponse(RequestingPlayFabID, true);
        Destroy(gameObject);
    }

    public void DeclineFriendRequest()
    {
        ClientSend.Instance.ClientSendFriendResponse(RequestingPlayFabID, false);
        Destroy(gameObject);
    }

    public void UpdateDisplayName()
    {
        UserDisplayName.text = RequestingUserDisplayName;
    }
}
