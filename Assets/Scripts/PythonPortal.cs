// Copyright (c) 2021 Kourosh T. Baghaei
//
// This source code is licensed under the license found in the
// LICENSE file in the root directory of this source tree.



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Text;
using DefaultNamespace;

public class PythonPortal : MonoBehaviour
{
    #region "Inspector Members"
    [SerializeField] int port = 65432;
    [Tooltip("Distance to move at each move input (must match with client)")]
    [SerializeField] float moveDistance = 1f;
    [Tooltip("Number of frames to wait until next processing")]
    [SerializeField] int packetSize = 4096;
    [SerializeField] int frameWait = 2;
    [SerializeField] int maxClients = 2;
    #endregion

    #region "Private Members"
    Socket udp;
    int idAssignIndex = 0;
    #endregion

    private KeyPointsPack2D latestPack;
    
    public KeyPointsPack2D getLatestPack2D()
    {
        return latestPack;
    }

    private static PythonPortal theOnlyPortal = null;
    public static PythonPortal mainPortal()
    {
        return theOnlyPortal;
    }
    
    void Start()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, port);

        Debug.Log("Server IP Address: " + IPAddress.Loopback);
        Debug.Log("Port: " + port);
        udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        udp.Bind(endPoint);
        udp.Blocking = false;
        theOnlyPortal = this;
    }

    void Update()
    {
        if(Time.frameCount % frameWait == 0)
        {
            if (udp.Available != 0)
            {
                byte[] packet = new byte[this.packetSize];
                EndPoint sender = new IPEndPoint(IPAddress.Any, port);
                int rec = udp.ReceiveFrom(packet, ref sender);
                string info = Encoding.UTF8.GetString(packet);
                this.latestPack = KeyPointsPack2D.GetPackFromJSON(info);
            }
        }
    }
}