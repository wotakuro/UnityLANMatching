﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LANMatching
{
    public class LANRoomManager : MonoBehaviour
    {
        public enum RunningStatus
        {
            None,
            HostRoom,
            SearchRoom,
        }

        [SerializeField]
        private int udpPacketPort = 9999;
        [SerializeField]
        private int broadCastSpanMs = 500;
        [SerializeField]
        private int timeoutMs = 1500;

        private byte[] buffer = new byte[1024];
        private int bufferSize;

        private Thread thread;
        private bool executeFlag;
        private bool shouldCallRecieveAsync = true;

        private Dictionary<EndPoint, HostRoomInfo> recievedRoomDictionary;
        private List<HostRoomInfo> roomBuffer;

        public RoomInfo hostRoomInfo { get; set; }

        public RunningStatus status
        {
            get; private set;
        }

        public delegate void RoomEvent (HostRoomInfo info);

        public RoomEvent OnFindNewRoom { get; set; }
        public RoomEvent OnLoseRoom { get; set; }

        public static LANRoomManager Instance
        {
            get;
            private set;
        }


        public List<HostRoomInfo> searchedRooms
        {
            get
            {
                if (this.roomBuffer == null) { this.roomBuffer = new List<HostRoomInfo>(); }
                this.roomBuffer.Clear();
                if(this.recievedRoomDictionary == null)
                {
                    return roomBuffer;
                }
                lock (recievedRoomDictionary)
                {
                    foreach (var kvs in this.recievedRoomDictionary)
                    {
                        this.roomBuffer.Add(kvs.Value);
                    }
                }
                return this.roomBuffer;
            }
        }

        public void StartHostThread()
        {
            if (thread == null)
            {
                this.status = RunningStatus.HostRoom;
                this.executeFlag = true;
                thread = new Thread(ExecuteHostThreaded);
                thread.Name = "LANHostThread";
                thread.Start();
            }
            else
            {
                Debug.LogError("Already Runnning.");
            }
        }
        public void StartClientThread()
        {
            if (thread == null)
            {
                if (this.recievedRoomDictionary == null)
                {
                    this.recievedRoomDictionary = new Dictionary<EndPoint, HostRoomInfo>();
                }
                this.status = RunningStatus.SearchRoom;
                this.executeFlag = true;
                thread = new Thread(ExecuteSearchRoomThreaded);
                thread.Name = "LANClientThread";
                thread.Start();
            }
        }

        #region UNITY_EVENTS
        private void Awake()
        {
            Instance = this;
            this.hostRoomInfo = new RoomInfo();
            this.hostRoomInfo.name = SystemInfo.deviceName;

            DontDestroyOnLoad(this.gameObject);
            this.executeFlag = false;
            this.status = RunningStatus.None;
        }
        private void OnDestroy()
        {
            this.Stop();
            Instance = null;
        }

        private void Update()
        {
            if (recievedRoomDictionary == null)
            {
                return;
            }
            double currentTime = Time.realtimeSinceStartupAsDouble;
            lock (this.recievedRoomDictionary)
            {
                foreach (var kvs in recievedRoomDictionary)
                {
                    if (kvs.Value.isNew)
                    {
                        // Callback
                        kvs.Value.isNew = false;
                        OnFindNewRoom?.Invoke(kvs.Value);
                        continue;
                    }
                    // Timeout or close
                    if (kvs.Value.lastRecieved - currentTime > timeoutMs / 1000.0 ||
                        !kvs.Value.roomInfo.isOpen)
                    {
                        OnLoseRoom?.Invoke(kvs.Value);
                        recievedRoomDictionary.Remove(kvs.Key);
                    }
                }
            }
        }
        #endregion UNITY_EVENTS




        #region HOST_LOGIC
        private void ExecuteHostThreaded()
        {
            var remote = new IPEndPoint(
                                IPAddress.Broadcast,
                                this.udpPacketPort);
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                SendBroadCastLoop(socket, remote);
            }
            this.thread = null;
            this.status = RunningStatus.None;
        }

        private void SendBroadCastLoop(Socket socket, IPEndPoint sendTo)
        {
            socket.DontFragment = true;
            socket.EnableBroadcast = true;
            socket.MulticastLoopback = false;

            while (executeFlag)
            {
                this.bufferSize = this.hostRoomInfo.WriteToByteArray(buffer);
                socket.SendTo(this.buffer, 0, this.bufferSize, SocketFlags.None, sendTo);
                Thread.Sleep(broadCastSpanMs);
            }
        }
        #endregion HOST_LOGIC


        #region CLIENT_LOGIC
        private void ExecuteSearchRoomThreaded()
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, udpPacketPort);
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                RecieveBroadCastLoop(socket, ipEndPoint);
            }
            this.thread = null;
            this.status = RunningStatus.None;
        }
        private void RecieveBroadCastLoop(Socket socket, IPEndPoint recieveFrom)
        {
            socket.Bind(recieveFrom);
            SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
            arg.RemoteEndPoint = recieveFrom;
            shouldCallRecieveAsync = true;
            arg.SetBuffer(this.buffer, 0, buffer.Length);
            arg.Completed += OnRecieveComplete;

            while (executeFlag)
            {
                if (shouldCallRecieveAsync)
                {
                    shouldCallRecieveAsync = false;
                    socket.ReceiveFromAsync(arg);
                }
                Thread.Sleep(1);
            }
        }

        private void OnRecieveComplete(object obj, SocketAsyncEventArgs evt)
        {
            HostRoomInfo info = null;
            var endPoint = evt.RemoteEndPoint;
            var ipEndPoint = endPoint as IPEndPoint;

            lock (recievedRoomDictionary)
            {
                if (!this.recievedRoomDictionary.TryGetValue(endPoint, out info))
                {
                    info = new HostRoomInfo(ipEndPoint,evt.Buffer,0);
                    this.recievedRoomDictionary.Add(endPoint, info);
                }
                else
                {
                    info.lastRecieved = Time.realtimeSinceStartupAsDouble;
                    info.roomInfo.ReadFromByteArray(evt.Buffer, 0);
                }
            }
            shouldCallRecieveAsync = true;            
        }

        #endregion CLIENT_LOGIC

        public void Stop()
        {
            this.executeFlag = false;
        }
    }

}