using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LANMatching.Sample
{
    internal class RoomSelectButton : MonoBehaviour
    {
        [SerializeField]
        private Button button;
        [SerializeField]
        private Text roomName;
        [SerializeField]
        private Text currentRoomMemberNum;

        public System.Action onClickButton { private get; set; }

        private HostRoomInfo roomInfo;

        internal bool IsSameHostRoom(HostRoomInfo info)
        {
            return (roomInfo == info);
        }

        public void Setup(HostRoomInfo info)
        {
            this.roomInfo = info;
            roomName.text = info.roomInfo.name; 
        }

        private void OnClickButton()
        {
            // Connect to MLAPI
            var netMgr = MLAPI.NetworkManager.Singleton;
            var transport = netMgr.NetworkConfig.NetworkTransport as MLAPI.Transports.UNET.UNetTransport;
            transport.ConnectPort = this.roomInfo.connectPoint.Port;
            transport.ConnectAddress = this.roomInfo.connectPoint.Address.ToString();
            netMgr.StartClient();

            this.onClickButton?.Invoke();
        }
    }
}
