using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[RequireComponent(typeof(ClientSend))][RequireComponent(typeof(ClientHandle))][RequireComponent(typeof(PlayFabSample))]
public class ClientTCP : MonoBehaviour
{
    public static ClientTCP Instance;

    [field: SerializeField, Tooltip("Service Authorization Key"), Header("Service Configuration")]
    public string ServiceAuthorizationKey { get; private set; } = default;

    [field: SerializeField, Tooltip("IP Address of server"), Header("Client Configuration")]
    public string IP { get; private set; } = default;

    [field: SerializeField, Tooltip("Port number of server")]
    public int Port { get; private set; } = default;

    [field: SerializeField, Tooltip ("User ID on the server")]
    public int UserID { get; set; } = default;

    public TcpClient socket;
    public NetworkStream stream;
    public SslStream sslStream;
    private byte[] receiveBuffer;

    private void Awake()
    {
        if(Instance == null) { Instance = this; } else if(Instance != this) { Destroy(this); }
        DontDestroyOnLoad(this);
    }
    /*
    private void Start()
    {
        ConnectToServer();
    }
    */

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
        try
        {
            socket.EndConnect(_result);
        }
        catch(Exception e)
        {
            print(e.ToString());
        }
        

        if (!socket.Connected) { print("Service offline..."); return; } // TODO Return error to the user.

        else
        {
            socket.NoDelay = true;
            stream = socket.GetStream();
            sslStream = new SslStream(stream, false, new RemoteCertificateValidationCallback(CertificateValidationCallback), null);
            
            sslStream.AuthenticateAsClient(PlayFabSample.Instance.ServerName);

            sslStream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, ReceivedData, null);
        }
    }
    static bool CertificateValidationCallback(object sender, 
        X509Certificate certificate, X509Chain chain, 
        SslPolicyErrors sslPolicyErrors)
    {
        if(sslPolicyErrors == SslPolicyErrors.None)
            return true;
        
        if(chain.ChainStatus.Length == 1)
            if(sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || certificate.Subject == certificate.Issuer)
            {
                if (chain.ChainStatus[0].Status == X509ChainStatusFlags.UntrustedRoot)
                {
                    return true;
                }
            }

        print($"Certificate error: {sslPolicyErrors}");
        return false;
    }

    private void ReceivedData(IAsyncResult _result)
    {
        try
        {
            int _byteLenght = sslStream.EndRead(_result);
            if (_byteLenght <= 0) { CloseConnection(); return; }

            byte[] _tempBuffer = new byte[_byteLenght];
            Array.Copy(receiveBuffer, _tempBuffer, _byteLenght);

            ClientHandle.Instance.HandleData(_tempBuffer);
            sslStream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize,
                ReceivedData, null);
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error while receiving data: {_ex}");
            CloseConnection();
            return;
        }
    }

    private void CloseConnection()
    {
        print("Connection was terminated...");
        if(sslStream != null)
        sslStream.Close();
        if(stream != null)
        stream.Close();
        if(socket != null)
        socket.Close();
    }
}
