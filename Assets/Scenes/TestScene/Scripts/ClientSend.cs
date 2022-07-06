using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    public static ClientSend Instance;

    [field: SerializeField, Tooltip("Friend PlayFab ID used to request status")]
    public string FriendPlayFabID { get; set; } = default;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else if (Instance != this) { Destroy(this); }
    }

    public void SendDataToServer(byte[] _data)
    {
        try
        {
            if(ClientTCP.Instance.socket != null)
            {
                ByteBuffer _buffer = new ByteBuffer();
                _buffer.WriteInt(_data.GetUpperBound(0) - _data.GetLowerBound(0) + 1);
                _buffer.WriteBytes(_data);
                ClientTCP.Instance.sslStream.BeginWrite(_buffer.ToArray(), 0, _buffer.ToArray().Length, null, null);
                _buffer.Dispose();
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error sending data: {_ex}");
        }
    }

    /// <summary>
    /// This is where the information is sent to the server. 
    /// If we want to send other data. 
    /// We can make a method similar to this one.
    /// </summary>

    public void HandShakeReceived()
    {
        ByteBuffer _buffer = new ByteBuffer(); 
        _buffer.WriteInt((int)ClientPackets.HandShakeReceived);

        //_buffer.WriteBool(false);
        _buffer.WriteString(PlayFabSample.Instance.PlayFabDisplayName);
        _buffer.WriteString(PlayFabSample.Instance.PlayFabID);
        _buffer.WriteString(PlayFabSample.Instance.PlayFabNetworkID);

        SendDataToServer(_buffer.ToArray());
        _buffer.Dispose();
    }

    public void GetFriendOnlineStatus()
    {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteInt((int)ClientPackets.UserInfoRequestReceived);

        //_buffer.WriteBool(true);
        _buffer.WriteString(FriendPlayFabID);

        SendDataToServer(_buffer.ToArray());
        _buffer.Dispose();
    }

    public void GetMultiUserOnlineStatus()
    {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteInt((int)ClientPackets.UserInfoRequestReceived);
        int friendcount = PlayFabSample.Instance.FriendsPlayFabIDs.Count;
        _buffer.WriteInt(friendcount);

        foreach(var friend in PlayFabSample.Instance.FriendsPlayFabIDs)
        {
            _buffer.WriteString(friend);
        }

        SendDataToServer(_buffer.ToArray());
        _buffer.Dispose();
    }

    public void AuthorizeClient()
    {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteInt((int)ClientPackets.AuthorizeClientReceived);

        _buffer.WriteString(ClientTCP.Instance.ServiceAuthorizationKey);

        SendDataToServer(_buffer.ToArray());
        _buffer.Dispose();
    }
}
