using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

[RequireComponent(typeof(ClientSend))][RequireComponent(typeof(ClientHandle))][RequireComponent(typeof(PlayFabSample))]
public class ClientTCP : MonoBehaviour
{
    public static ClientTCP Instance;

    [field: SerializeField, Tooltip("IP Address of server"), Header("Client Configuration")]
    public string IP { get; private set; } = default;

    [field: SerializeField, Tooltip("Port number of server")]
    public int Port { get; private set; } = default;

    [field: SerializeField, Tooltip ("User ID on the server")]
    public int UserID { get; set; } = default;

    public TcpClient socket;
    public NetworkStream stream;
    private byte[] receiveBuffer;

    private void Awake()
    {
        if(Instance == null) { Instance = this; } else if(Instance != this) { Destroy(this); }
    }

    private void Start()
    {
        ConnectToServer();
    }

    private void OnDisable()
    {
        CloseConnection();
    }

    public void ConnectToServer()
    {
        ClientHandle.Instance.InitPackets();

        socket = new TcpClient
        {
            ReceiveBufferSize = 4096,
            SendBufferSize = 4096,
            NoDelay = false
        };

        receiveBuffer = new byte[socket.ReceiveBufferSize];
        socket.BeginConnect(IP, Port, ConnectCallBack, socket);
    }

    private void ConnectCallBack(IAsyncResult _result)
    {
        socket.EndConnect(_result);

        if (!socket.Connected) { return; }

        else
        {
            socket.NoDelay = true;
            stream = socket.GetStream();
            stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, ReceivedData, null);
        }
    }

    private void ReceivedData(IAsyncResult _result)
    {
        try
        {
            int _byteLenght = stream.EndRead(_result);
            if (_byteLenght <= 0) { CloseConnection(); return; }

            byte[] _tempBuffer = new byte[_byteLenght];
            Array.Copy(receiveBuffer, _tempBuffer, _byteLenght);

            ClientHandle.Instance.HandleData(_tempBuffer);
            stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize,
                ReceivedData, null);
        }
        catch (Exception _ex)
        {
            Console.WriteLine($"Error while receiving data: {_ex}");
            CloseConnection();
            return;
        }
    }

    private void CloseConnection()
    {
        socket.Close();
    }
}