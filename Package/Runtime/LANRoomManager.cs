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
        [SerializeField]
        private int udpPacketPort = 9999;
        [SerializeField]
        private int broadCastSpanMs = 1000;
        
        private byte[] buffer = new byte[1024];
        private int bufferSize;

        private Thread thread;
        private bool executeFlag;
        private bool shouldCallRecieveAsync = true;

        private Dictionary<EndPoint, HostRoomInfo> seachedRooms;

        public RoomInfo hostRoomInfo { get; set; }
        public static LANRoomManager Instance
        {
            get; private set;
        }


        private void Awake()
        {
            Instance = this;
            this.hostRoomInfo = new RoomInfo();
            this.hostRoomInfo.name = SystemInfo.deviceName;

            DontDestroyOnLoad(this.gameObject);
            this.executeFlag = false;
            this.seachedRooms = new Dictionary<EndPoint, HostRoomInfo>();
        }
        private void OnDestroy()
        {
            this.Stop();
            Instance = null;
        }

        // Debug
        private void Start()
        {
            StartHostThread();
            StartClientThread();
        }

        public void StartHostThread()
        {
            if (thread == null)
            {
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
            {
                this.executeFlag = true;
                thread = new Thread(ExecuteSearchRoomThreaded);
                thread.Name = "LANClientThread";
                thread.Start();
            }
        }

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
            if(!this.seachedRooms.TryGetValue(endPoint,out info))
            {
                info = new HostRoomInfo();
                info.connectPoint = new IPEndPoint(ipEndPoint.Address, ipEndPoint.Port);
                this.seachedRooms.Add(endPoint, info);
            }
            info.roomInfo.ReadFromByteArray(evt.Buffer, 0);
            info.connectPoint.Port = info.roomInfo.port;
            shouldCallRecieveAsync = true;            
        }

        #endregion CLIENT_LOGIC

        public void Stop()
        {
            this.executeFlag = false;
            this.thread = null;
        }
    }

}