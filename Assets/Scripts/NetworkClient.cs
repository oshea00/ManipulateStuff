using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
#if !UNITY_EDITOR
using Windows.Networking;
using Windows.Networking.Sockets;
#endif

public class NetworkClient : Singleton<NetworkClient> {
    public static string sendMessage="";
    public static string rcvdMessage="";

    public int pollSeconds = 1;
    public string HostIP = "10.0.5.132";
    public string HostPort = "8088";
    public bool startSendLoop = false;

    private void Start()
    {
        if (startSendLoop)
        {
            StartCoroutine(ExchangeMessage());
        }
    }

    IEnumerator ExchangeMessage()
    {
        while (true)
        {
            yield return new WaitForSeconds(pollSeconds);
            RequestResponse();
        }        
    }

    public void Send(string msg)
    {
        sendMessage = msg;
        RequestResponse();
    }

#if !UNITY_EDITOR
    async void RequestResponse () {

        try
        {
            StreamSocket socket = new StreamSocket();
            HostName serverHost = new HostName(HostIP);
            string serverPort = HostPort;
            await socket.ConnectAsync(serverHost, serverPort);
            Stream streamOut = socket.OutputStream.AsStreamForWrite();
            Stream streamIn = socket.InputStream.AsStreamForRead();
            StreamWriter writer = new StreamWriter(streamOut);
            StreamReader reader = new StreamReader(streamIn);
            if (string.IsNullOrEmpty(sendMessage)==false)
            {
                await writer.WriteLineAsync(sendMessage);
                await writer.FlushAsync();
                string received = await reader.ReadLineAsync();
                if (received != null)
                {
                    rcvdMessage = received;
                }
                else
                {
                    rcvdMessage = "";
                }
            }
            writer.Dispose();
            reader.Dispose();
            socket.Dispose();
        }
        catch (Exception)
        {
            rcvdMessage = "Error connecting Host: "+HostIP+":"+HostPort;
        }
    }
#else
    void RequestResponse()
    {
        if (string.IsNullOrEmpty(sendMessage)==false)
        {
            rcvdMessage = "Host: "+HostIP+":"+HostPort+" - Test Response";
        }
    }
#endif

}
