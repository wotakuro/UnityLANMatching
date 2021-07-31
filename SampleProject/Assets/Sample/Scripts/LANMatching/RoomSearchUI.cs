using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LANMatching.Sample
{
    public class RoomSearchUI : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect scrollRect;
        [SerializeField]
        private Button backButton;
        [SerializeField]
        private GameObject selectButtonPrefab;

        [SerializeField]
        private WaitingOtherClientUI waitingOtherClientUI;


        private InformationInputUI inputUI;


        private List<RoomSelectButton> roomSelectButtons;

        public void Setup(InformationInputUI ui)
        {
            this.inputUI = ui;
        }

        private void Awake()
        {
            this.backButton.onClick.AddListener(this.OnBackButton);
            this.roomSelectButtons = new List<RoomSelectButton>();
        }

        // Start is called before the first frame update
        private void OnEnable()
        {
            LANRoomManager.Instance.OnFindNewRoom = OnFindNewRoom;
            LANRoomManager.Instance.OnChangeRoom = OnChangeNewRoom;
            LANRoomManager.Instance.OnLoseRoom = OnLoseRoom;
            LANRoomManager.Instance.StartClientThread();
        }
        private void OnDisable()
        {
            if (LANRoomManager.Instance)
            {
                LANRoomManager.Instance.OnFindNewRoom = null;
                LANRoomManager.Instance.OnChangeRoom = null;
                LANRoomManager.Instance.OnLoseRoom = null;
                LANRoomManager.Instance.Stop();
            }
        }

        private void OnFindNewRoom(HostRoomInfo info)
        {
            var obj = GameObject.Instantiate(this.selectButtonPrefab, this.scrollRect.transform);
            var selectBtn = obj.GetComponent<RoomSelectButton>();
            selectBtn.Setup(this,info);
            this.roomSelectButtons.Add(selectBtn);
        }

        private void OnChangeNewRoom(HostRoomInfo info)
        {
            foreach( var roomBtn in this.roomSelectButtons)
            {
                if(roomBtn.IsSameHostRoom(info) ){
                    roomBtn.Setup(this, info);
                    break;
                }
            }
        }

        private void OnLoseRoom(HostRoomInfo info)
        {
            int cnt = this.roomSelectButtons.Count;
            for(int i = 0; i < cnt; ++i)
            {
                if (this.roomSelectButtons[i].IsSameHostRoom(info))
                {
                    GameObject.Destroy(this.roomSelectButtons[i].gameObject);
                    this.roomSelectButtons.RemoveAt(i);
                    break;
                }
            }
        }

        private void OnBackButton()
        {
            this.gameObject.SetActive(false);
            this.inputUI.gameObject.SetActive(true);
        }

        internal void OnClickRoomButton(HostRoomInfo roomInfo)
        {
            waitingOtherClientUI.Setup(this.inputUI);
            waitingOtherClientUI.gameObject.SetActive(true);
            // Connect to MLAPI
            var netMgr = MLAPI.NetworkManager.Singleton;
            var transport = netMgr.NetworkConfig.NetworkTransport as MLAPI.Transports.UNET.UNetTransport;
            transport.ConnectPort = roomInfo.connectPoint.Port;
            transport.ConnectAddress = roomInfo.connectPoint.Address.ToString();
            //            Debug.Log(transport.ConnectAddress);
            netMgr.OnClientConnectedCallback += OnStartClient;
            netMgr.StartClient();
            this.gameObject.SetActive(false);
        }
        void OnStartClient(ulong clientId)
        {
        }



    }
}
