using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    public delegate IEnumerator FriendRequest();
    public static event FriendRequest OnFriendRequest;

    public delegate IEnumerator FriendResponse();
    public static event FriendResponse OnFriendResponse;

    public delegate IEnumerator FriendListUpdated();
    public static event FriendListUpdated OnFriendListUpdated;

    /// <summary>
    /// Invokes FriendRequest delegate on Unity's main thread.
    /// </summary>
    public static void OnFriendRequestCallBack()
    {
        if(OnFriendRequest != null)
        UnityMainThreadDispatcher.Instance().Enqueue(OnFriendRequest?.Invoke());
    }

    /// <summary>
    /// Invokes FriendResponse delegate on Unity's main thread.
    /// </summary>
    public static void OnFriendResponseCallBack()
    {
        if(OnFriendResponse != null)
        UnityMainThreadDispatcher.Instance().Enqueue(OnFriendResponse?.Invoke());
    }

    /// <summary>
    /// Invokes FriendListUpdated on Unity's main thread.
    /// </summary>
    public static void OnFriendListUpdatedCallBack()
    {
        if (OnFriendListUpdated != null)
        UnityMainThreadDispatcher.Instance().Enqueue(OnFriendListUpdated?.Invoke());
    }
}
