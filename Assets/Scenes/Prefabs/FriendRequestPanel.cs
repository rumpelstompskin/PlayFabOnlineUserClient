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

    public UserData userData = default;

    public SendFriendRequestPanel requestPanel { get; set; } = default;

    public void AcceptFriendRequest()
    {
        ClientSend.Instance.ClientSendFriendResponse(userData.ID, true);
        AcceptDeclineFriendRequest();
    }

    public void DeclineFriendRequest()
    {
        ClientSend.Instance.ClientSendFriendResponse(userData.ID, false);
        AcceptDeclineFriendRequest();
    }

    private void AcceptDeclineFriendRequest()
    {
        requestPanel.FriendRequestShown = false;
        PlayFabSample.Instance.FriendsUserData.Add(userData);
        PlayFabSample.Instance.RequestingFriendShipUserData.Remove(userData);
        requestPanel.InProgressFriendRequest.Remove(userData);
        requestPanel.InProgressFriendRequest.TrimExcess();
        if (requestPanel.InProgressFriendRequest.Count > 0)
        {
            requestPanel.ProcessFriendRequest(requestPanel.InProgressFriendRequest[0]);
        }
        Destroy(gameObject);
    }

    public void UpdateDisplayName()
    {
        UserDisplayName.text = RequestingUserDisplayName;
    }
}
