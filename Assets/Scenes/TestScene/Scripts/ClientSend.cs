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
                ClientTCP.Instance.stream.BeginWrite(_buffer.ToArray(), 0, _buffer.ToArray().Length, null, null);
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
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteInt((int)ClientPackets.HandShakeReceived);

        _buffer.WriteString(PlayFabSample.Instance.PlayFabDisplayName);
        _buffer.WriteString(PlayFabSample.Instance.PlayFabID);
        _buffer.WriteString(PlayFabSample.Instance.PlayFabNetworkID);

        SendDataToServer(_buffer.ToArray());
        _buffer.Dispose();
    }
}
