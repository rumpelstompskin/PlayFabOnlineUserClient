using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    public static ClientSend Instance;
    
    private void Awake()
    {
       if(Instance != null && Instance != this)
        {
            Destroy(Instance);
        } else
        {
            Instance = this;
        }

        //UnityMainThreadDispatcher.Instance().Enqueue
        //(Logger.Instance.FindOrCreateLog());
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

    public void HandShakeReceived()
    {
        Globals.OnConsoleUpdatedCallBack("Handshake confirmed, sending server our data...");
        ByteBuffer _buffer = new ByteBuffer(); 
        _buffer.WriteInt((int)ClientPackets.HandShakeReceived);
        _buffer.WriteString(PlayFabSample.Instance.PlayFabDisplayName);
        _buffer.WriteString(PlayFabSample.Instance.PlayFabID);
        _buffer.WriteString(PlayFabSample.Instance.PlayFabNetworkID);

        SendDataToServer(_buffer.ToArray());
        _buffer.Dispose();
    }
    public void GetMultiUserOnlineStatus()
    {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteInt((int)ClientPackets.UserInfoRequestReceived);
        int friendcount = PlayFabSample.Instance.FriendsUserData.Count;
        _buffer.WriteInt(friendcount);

        foreach(var friend in PlayFabSample.Instance.FriendsUserData)
        {
            _buffer.WriteString(friend.ID);
        }

        SendDataToServer(_buffer.ToArray());
        _buffer.Dispose();
    }

    public void AuthorizeClient()
    {
        Globals.OnConsoleUpdatedCallBack("Authorizing client's connection...");

        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteInt((int)ClientPackets.AuthorizeClientReceived);
        _buffer.WriteString(ClientTCP.Instance.ServiceAuthorizationKey);

        SendDataToServer(_buffer.ToArray());
        _buffer.Dispose();
    }

    public void ClientSendFriendRequest(string userToSendTo)
    {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteInt((int)ClientPackets.FriendsRequestReceived);
        _buffer.WriteString(userToSendTo);

        SendDataToServer(_buffer.ToArray());
        _buffer.Dispose();
    }

    public void ClientSendFriendResponse(string requestingUser, bool response)
    {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteInt((int)ClientPackets.FriendsRequestResponseReceived);
        _buffer.WriteBool(response);
        _buffer.WriteString(requestingUser);

        SendDataToServer(_buffer.ToArray());
        _buffer.Dispose();
    }
}
