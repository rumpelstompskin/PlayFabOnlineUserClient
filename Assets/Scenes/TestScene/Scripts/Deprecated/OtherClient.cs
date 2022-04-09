using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class OtherClient : MonoBehaviour
{
    public static OtherClient Instance;

    public static int dataBufferSize = 512;

    [field: SerializeField, Tooltip("Service IP"), Header("Service Configuration")] // For testing LocalHost, production www.metagamez.net
    public string IP { get; private set; } = default;
    [field: SerializeField, Tooltip("Service Port")] // default port 26950
    public int Port { get; private set; } = 26950;
    [field: SerializeField, Tooltip("Local Client ID. This would be the users PlayFab ID")]
    public int LocalID { get; private set; } = 0;

    public TCP tcp;

    private void Awake()
    {
        if(Instance == null) { Instance = this; } else if (Instance != this) { Destroy(this); }
    }

    private void Start()
    {
        tcp = new TCP();
    }

    public void ConnectToServer()
    {
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient _socket;

        private NetworkStream _stream;
        private byte[] _receiveBuffer;

        public void Connect()
        {
            _socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            _receiveBuffer = new byte[dataBufferSize];
            _socket.BeginConnect(Instance.IP, Instance.Port, ConnectCallBack, _socket);
        }

        private void ConnectCallBack(IAsyncResult _result)
        {
            _socket.EndConnect(_result);

            if (!_socket.Connected)
            {
                return;
            }

            _stream = _socket.GetStream();

            _stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallBack, null);
        }

        private void ReceiveCallBack(IAsyncResult _result)
        {
            try
            {
                int _byteLenght = _stream.EndRead(_result);
                if (_byteLenght <= 0) { return; }

                byte[] _data = new byte[_byteLenght];
                Array.Copy(_receiveBuffer, _data, _byteLenght);
                // TODO: Handle data
                Debug.Log("We have received data.");
                _stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallBack, null);
            }
            catch (Exception _ex)
            {
                // TODO: Disconnect
            }
        }
    }
}
