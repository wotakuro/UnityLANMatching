using System.Collections;
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

        public enum BroadcastMethod
        {
            IPv4 = 0,
            IPv6 = 1,
        }

        [SerializeField]
        private int udpPacketPort = 9999;
        [SerializeField]
        private int broadCastSpanMs = 500;
        [SerializeField]
        private int timeoutMs = 1500;

        // ipv6 is not yet
        [SerializeField]
        [HideInInspector]
        private BroadcastMethod method = BroadcastMethod.IPv4;

        private byte[] buffer = new byte[1024];
        private int bufferSize;

        private Thread thread;
        private bool executeFlag;
        private bool shouldCallRecieveAsync = true;

        private Dictionary<EndPoint, HostRoomInfo> recievedRoomDictionary;
        private List<HostRoomInfo> roomBuffer;
        private List<EndPoint> keyBuffer;

        internal static double currentTime;

        public RoomInfo hostRoomInfo { get; set; }

        public RunningStatus status
        {
            get; private set;
        }

        public delegate void RoomEvent (HostRoomInfo info);

        public RoomEvent OnFindNewRoom { private get; set; }
        public RoomEvent OnLoseRoom { private get; set; }
        public RoomEvent OnChangeRoom { private get; set; }

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
                this.recievedRoomDictionary.Clear();
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
            if (Instance)
            {
                GameObject.Destroy(this.gameObject);
                return;
            }

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
            if (this == Instance)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            currentTime = Time.realtimeSinceStartupAsDouble;
            if (recievedRoomDictionary == null)
            {
                return;
            }
            if(keyBuffer == null)
            {
                keyBuffer = new List<EndPoint>();
            }
            keyBuffer.Clear();
            lock (this.recievedRoomDictionary)
            {
                foreach (var kvs in recievedRoomDictionary)
                {
                    if (kvs.Value.isNew)
                    {
                        // Callback
                        kvs.Value.UpdateFlags();
                        OnFindNewRoom?.Invoke(kvs.Value);
                        continue;
                    }
                    // Timeout or close
                    else if (currentTime - kvs.Value.lastRecieved > timeoutMs / 1000.0 ||
                        !kvs.Value.roomInfo.isOpen)
                    {
                        keyBuffer.Add(kvs.Key);
                        OnLoseRoom?.Invoke(kvs.Value);
                    }
                    // information changed
                    else if (kvs.Value.isChanged)
                    {
                        kvs.Value.UpdateFlags();
                        OnChangeRoom?.Invoke(kvs.Value);
                    }
                }
                // Remove Room info
                foreach( var key in keyBuffer)
                {
                    recievedRoomDictionary.Remove(key);
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
                for (int left = broadCastSpanMs; left > 0;)
                {
                    if (!executeFlag) { break; }
                    if (left > 10)
                    {
                        Thread.Sleep(10);
                    }
                    else
                    {
                        Thread.Sleep(left);
                    }
                    left -= 10;
                }
            }
            // close info send
            this.hostRoomInfo.isOpen = false;
            this.bufferSize = this.hostRoomInfo.WriteToByteArray(buffer);
            socket.SendTo(this.buffer, 0, this.bufferSize, SocketFlags.None, sendTo);
            // and back to normal
            this.hostRoomInfo.isOpen = false;
        }
        #endregion HOST_LOGIC


        #region CLIENT_LOGIC
        private void ExecuteSearchRoomThreaded()
        {
            bool flag = true;
            while (flag)
            {
                try
                {
                    var ipEndPoint = new IPEndPoint(IPAddress.Any, udpPacketPort);
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                    {
                        RecieveBroadCastLoop(socket, ipEndPoint);
                        flag = false;
                    }
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    Thread.Sleep(1000);
                }
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
                    if (info.roomInfo.isOpen)
                    {
                        this.recievedRoomDictionary.Add(endPoint, info);
                    }
                }
                else
                {
                    info.UpdateRecievedTime();
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