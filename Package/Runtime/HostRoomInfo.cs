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
        public bool isNew;
        
        public HostRoomInfo(IPEndPoint ipEndPoint, byte[] buffer,int index)
        {
            this.roomInfo = new RoomInfo();
            this.roomInfo.ReadFromByteArray(buffer, index);
            this.lastRecieved = Time.realtimeSinceStartupAsDouble;
            this.isNew = true;
            this.connectPoint = new IPEndPoint(ipEndPoint.Address, this.roomInfo.port);
        }
    }
}