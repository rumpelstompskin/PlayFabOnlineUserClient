using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SendFriendRequestPanel : MonoBehaviour
{
    [field: SerializeField, Tooltip("Input Field"), Header("UI Elements")]
    public TMP_InputField InputField { get; set; } = default;

    [field: SerializeField, Tooltip("Friend Request Panel Reference")]
    public GameObject RequestPanel { get; set; } = default;

    [field: SerializeField, Tooltip("Requesting user PlayFab ID")]
    public string RequestingUserPlayFabID { get; set; } = default;

    [field: SerializeField, Tooltip("Requesting user PlayFab display name")]
    public string RequestingUserDisplayName { get; set; } = default;

    public List<UserData> InProgressFriendRequest = new List<UserData>();

    public bool FriendRequestShown = false;

    public void SendFriendRequestToPlayFabID()
    {
        print($"Requesting friendship with playerID {InputField.text}");
        ClientSend.Instance.ClientSendFriendRequest(InputField.text);
        InputField.text = "";
    }

    private void OnEnable()
    {
        Globals.OnFriendRequest += UpdateFriendRequests;
    }

    private void OnDisable()
    {
        Globals.OnFriendRequest -= UpdateFriendRequests;
    }

    public IEnumerator UpdateFriendRequests()
    {
        foreach(var userData in PlayFabSample.Instance.RequestingFriendShipUserData)
        {
            if (InProgressFriendRequest.Contains(userData))
            {
                continue;
            }
            InProgressFriendRequest.Add(userData);
            RequestingUserPlayFabID = userData.ID;
            RequestingUserDisplayName = userData.Name;

            ProcessFriendRequest(userData);
        }
        yield return null;
    }

    public void ProcessFriendRequest(UserData userData)
    {
        if(InProgressFriendRequest.Count > 0 && 
            !PlayFabSample.Instance.FriendsUserData
            .Contains(userData))
        // Only run if we have friend requests in queue and we arn't already friends.
        {
            if(FriendRequestShown == false) // We currently are not processing any friend request.
            {
                FriendRequestShown = true;
                GameObject requestPanel = Instantiate(RequestPanel, transform.parent);
                if (requestPanel.TryGetComponent(out FriendRequestPanel friendRequestPanel))
                {
                    friendRequestPanel.requestPanel = this;
                    friendRequestPanel.userData = userData;
                    friendRequestPanel.RequestingPlayFabID = userData.ID;
                    friendRequestPanel.RequestingUserDisplayName = userData.Name;
                    friendRequestPanel.UserDisplayName.text = userData.Name;
                }
            }
        }
    }

    public void ProcessFriendRequestTest()
    {
        GameObject requestPanel = Instantiate(RequestPanel, transform.parent);
        if(requestPanel.TryGetComponent(out FriendRequestPanel friendRequestPanel))
        {
            friendRequestPanel.RequestingPlayFabID = RequestingUserPlayFabID;
            friendRequestPanel.RequestingUserDisplayName = RequestingUserDisplayName;
            friendRequestPanel.UserDisplayName.text = RequestingUserDisplayName;
        }
    }
}
