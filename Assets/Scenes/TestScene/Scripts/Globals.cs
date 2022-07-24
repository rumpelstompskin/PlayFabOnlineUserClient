using System.Collections;

public static class Globals
{
    public delegate IEnumerator FriendRequest();
    public static event FriendRequest OnFriendRequest;

    public delegate IEnumerator FriendResponse();
    public static event FriendResponse OnFriendResponse;

    public delegate IEnumerator FriendListUpdated();
    public static event FriendListUpdated OnFriendListUpdated;

    public delegate IEnumerator ConsoleUpdated(string text);
    public static event ConsoleUpdated OnConsoleUpdated;

    public delegate IEnumerator LogsUpdated(string text);
    public static event LogsUpdated OnLogsUpdated;

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

    public static void OnConsoleUpdatedCallBack(string textToAdd)
    {
        if (OnConsoleUpdated != null)
        UnityMainThreadDispatcher.Instance().Enqueue(OnConsoleUpdated?.Invoke(textToAdd));
    }

    public static void OnLogsUpdatedCallBack(string textToAdd)
    {
        if (OnLogsUpdated != null)
        UnityMainThreadDispatcher.Instance().Enqueue(OnLogsUpdated?.Invoke(textToAdd));
    }
}
