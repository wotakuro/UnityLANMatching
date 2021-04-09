using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

namespace LANMatching
{
    public class HostRoomInfo
    {
        public RoomInfo roomInfo;
        public IPEndPoint connectPoint;
        public double lastRecieved;
    }
}