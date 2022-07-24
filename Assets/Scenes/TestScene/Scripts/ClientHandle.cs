using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static ClientHandle Instance;

    private ByteBuffer buffer;

    public delegate void Packet(byte[] data);
    public Dictionary<int, Packet> packets;
    
    private void Awake()
    {
        if(Instance != null && Instance != this) 
        {
            Destroy(this);
        } else
        {
            Instance = this;
        }
    }
    
    public void InitPackets()
    {
        Debug.Log("Initializing packets...");
        packets = new Dictionary<int, Packet>
        {
            { (int)ServerPackets.HandShake, HandShake },
            { (int)ServerPackets.UserInfoRequest, MultiUserInfoReceived },
            { (int)ServerPackets.AuthorizeClient, AuthorizationRequested },
            { (int)ServerPackets.FriendRequest, FriendsRequestReceived },
            { (int)ServerPackets.FriendResponse, FriendsRequestResponseReceived }
        };
    }

    public void HandleData(byte[] _data)
    {
        byte[] _tempBuffer = (byte[])_data.Clone();
        int _packetLength = 0;
        if (buffer == null)
        {
            buffer = new ByteBuffer();
        }
        buffer.WriteBytes(_tempBuffer);
        if (buffer.Count() == 0)
        {
            buffer.Clear();
            return;
        }
        if (buffer.Length() >= 4)
        {
            _packetLength = buffer.ReadInt(false);
            if (_packetLength <= 0)
            {
                buffer.Clear();
                return;
            }
        }
        while (_packetLength > 0 && _packetLength <= buffer.Length() - 4)
        {
            if (_packetLength <= buffer.Length() - 4)
            {
                buffer.ReadInt();
                _data = buffer.ReadBytes(_packetLength);
                HandlePackets(_data);
            }
            _packetLength = 0;
            if (buffer.Length() >= 4)
            {
                _packetLength = buffer.ReadInt(false);
                if (_packetLength <= 0)
                {
                    buffer.Clear();
                    return;
                }
            }
        }
        if (_packetLength <= 1)
        {
            buffer.Clear();
        }
    }

    private void HandlePackets(byte[] _data) // Reads the id of the packet and calls the dictionary.
    {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteBytes(_data);
        int _packetID = _buffer.ReadInt();
        _buffer.Dispose();
        if (packets.TryGetValue(_packetID, out Packet _packet))
        {
            _packet.Invoke(_data);
        }
    }

    private void HandShake(byte[] _data) // Initial handshake. Tells the client it has successfully connected.
    {
        Globals.OnConsoleUpdatedCallBack("Server has confirmed our handshake and welcomed us... " +
            "Attempting to reply with user data...");
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteBytes(_data);
        _buffer.ReadInt();

        string _msg = _buffer.ReadString();
        int _myPlayerID = _buffer.ReadInt();
        _buffer.Dispose();
        Debug.Log("Message from server: " + _msg);
        ClientTCP.Instance.UserID = _myPlayerID;
        ClientSend.Instance.HandShakeReceived();
    }

    public void MultiUserInfoReceived(byte[] _data)
    {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteBytes(_data);
        _buffer.ReadInt();

        int count = _buffer.ReadInt();

        for (int i = 0; i < count; i++)
        {
            string friendDisplayName = _buffer.ReadString();
            string friendPlayFabID = _buffer.ReadString();

            UserData friendUserData = new UserData(friendPlayFabID, friendDisplayName);

            if (!PlayFabSample.Instance.CurrentlyOnlineFriendsUserData.Contains(friendUserData))
            {
                PlayFabSample.Instance.CurrentlyOnlineFriendsUserData.Add(friendUserData);
            }
        }

        _buffer.Dispose();
        Globals.OnFriendListUpdatedCallBack();
    }

    public void AuthorizationRequested(byte[] _data)
    {
        Globals.OnConsoleUpdatedCallBack("Receiving authorization key request from server... Attempting to reply...");
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteBytes(_data);
        _buffer.ReadInt();

        ClientSend.Instance.AuthorizeClient();
        _buffer.Dispose();
    }

    public void FriendsRequestReceived(byte[] _data)
    {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteBytes(_data);
        _buffer.ReadInt();
        string requestingFriendID = _buffer.ReadString();
        string requestingFriendDisplayName = _buffer.ReadString();

        UserData userData = new UserData(requestingFriendID, requestingFriendDisplayName);

        if(!PlayFabSample.Instance.RequestingFriendShipUserData.Contains(userData))
        {
            PlayFabSample.Instance.RequestingFriendShipUserData.Add(userData);
        }

        _buffer.Dispose();

        Globals.OnFriendRequestCallBack();
    }

    public void FriendsRequestResponseReceived(byte[] _data)
    {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteBytes(_data);
        _buffer.ReadInt();

        bool response = _buffer.ReadBool();
        string responseFromUser = _buffer.ReadString();
        string fromUserName = _buffer.ReadString();

        if (response)
        {
            UserData userData = new UserData(responseFromUser, fromUserName);
            if(!PlayFabSample.Instance.FriendsUserData.Contains(userData))
            PlayFabSample.Instance.FriendsUserData.Add(userData);
        }

        _buffer.Dispose();

        Globals.OnFriendResponseCallBack();
    }
}
