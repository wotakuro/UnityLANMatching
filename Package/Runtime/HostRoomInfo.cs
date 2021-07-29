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
        internal double lastRecieved;
        internal bool isNew;
        internal bool isChanged;


        public HostRoomInfo(IPEndPoint ipEndPoint, byte[] buffer, int index)
        {
            this.roomInfo = new RoomInfo();
            this.isChanged = this.roomInfo.ReadFromByteArray(buffer, index);
            this.isNew = true;
            this.connectPoint = new IPEndPoint(ipEndPoint.Address, this.roomInfo.port);
            this.UpdateRecievedTime();
        }
        internal void UpdateRecievedTime()
        {
            this.lastRecieved = LANRoomManager.currentTime;
        }

        internal void UpdateFlags()
        {
            this.isNew = false;
            this.isChanged = false;
        }

    }
}