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
        if(Instance == null) { Instance = this; } else if (Instance != this) { Destroy(this); }
    }

    public void InitPackets()
    {
        Debug.Log("Initializing packets...");
        packets = new Dictionary<int, Packet>
        {
            { (int)ServerPackets.HandShake, HandShake },
            { (int)ServerPackets.UserInfoRequest, MultiUserInfoReceived },
            { (int)ServerPackets.AuthorizeClient, AuthorizationRequested }
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

    private static void HandShake(byte[] _data) // Initial handshake. Tells the client it has successfully connected.
    {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteBytes(_data);
        _buffer.ReadInt();

        string _msg = _buffer.ReadString();
        int _myPlayerID = _buffer.ReadInt();
        _buffer.Dispose();
        Debug.Log("Message from server: " + _msg);
        ClientTCP.Instance.UserID = _myPlayerID;
        ClientSend.Instance.HandShakeReceived();
        //ClientSend.Instance.AuthorizeClient();
    }
    /*
    public static void UserInfoReceived(byte[] _data)
    {
        string _response = string.Empty;
        bool _status = false;
        string _friendPlayFabID;
        string _friendPlayFabNetworkID;

        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteBytes(_data);
        _buffer.ReadInt();

        _status = _buffer.ReadBool();
        _friendPlayFabID = _buffer.ReadString();
        if (_status == true)
        {
            //User is online.
            _friendPlayFabNetworkID = _buffer.ReadString();
            _response = $"User: {_friendPlayFabID} Online Status: {_status} Network ID: {_friendPlayFabNetworkID}";
        }
        else
        {
            _response = $"User: {_friendPlayFabID} Online Status: {_status}";
        }

        print($"Debug: {_response}");
        _buffer.Dispose();
    }
    */
    public static void MultiUserInfoReceived(byte[] _data)
    {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteBytes(_data);
        _buffer.ReadInt();

        int count = _buffer.ReadInt();

        for (int i = 0; i < count; i++)
        {
            string friendPlayFabID = _buffer.ReadString();
            if(!PlayFabSample.Instance.OnlineFriends.Contains(friendPlayFabID))
            PlayFabSample.Instance.OnlineFriends.Add(friendPlayFabID);
        }

        _buffer.Dispose();
    }

    public static void AuthorizationRequested(byte[] _data)
    {

        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteBytes(_data);
        _buffer.ReadInt();
        ClientSend.Instance.AuthorizeClient();
        _buffer.Dispose();
        //ClientSend.Instance.HandShakeReceived();
        //print("Client is authorized...");
    }
}
