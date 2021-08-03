using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LANMatching.Sample
{
    /// <summary>
    /// ルームを探す時のUI
    /// </summary>
    public class RoomSearchUI : MonoBehaviour
    {
        // ルーム一覧が出るScrollView
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

        // 初期化処理
        public void Setup(InformationInputUI ui)
        {
            this.inputUI = ui;
        }

        // Awake処理
        private void Awake()
        {
            this.backButton.onClick.AddListener(this.OnBackButton);
            this.roomSelectButtons = new List<RoomSelectButton>();
        }

        // OnEnable処理
        private void OnEnable()
        {
            this.roomSelectButtons.Clear();
            LANRoomManager.Instance.OnFindNewRoom = OnFindNewRoom;
            LANRoomManager.Instance.OnChangeRoom = OnChangeRoom;
            LANRoomManager.Instance.OnLoseRoom = OnLoseRoom;
            LANRoomManager.Instance.StartClientThread();
        }

        // OnDisable処理
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

        // 新しいルームが見つかった時の処理
        private void OnFindNewRoom(HostRoomInfo info)
        {
            var obj = GameObject.Instantiate(this.selectButtonPrefab, this.scrollRect.content);
            var selectBtn = obj.GetComponent<RoomSelectButton>();
            selectBtn.Setup(this,info);
            this.roomSelectButtons.Add(selectBtn);
            UpdatePositions();
        }

        // ルーム情報が変更された時の処理処理
        private void OnChangeRoom(HostRoomInfo info)
        {
            foreach( var roomBtn in this.roomSelectButtons)
            {
                if(roomBtn.IsSameHostRoom(info) ){
                    roomBtn.Setup(this, info);
                    break;
                }
            }
        }

        // ルーム情報を見失った時の処理処理
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
            UpdatePositions();
        }

        // ルーム情報のUIの位置を更新します
        private void UpdatePositions()
        {
            int cnt = this.roomSelectButtons.Count;
            for (int i = 0; i < cnt; ++i)
            {
                this.roomSelectButtons[i].SetPosition(new Vector2(0, -5-i * 55));
            }

            var size = scrollRect.content.sizeDelta;
            size.y = cnt * 55 + 10;
            scrollRect.content.sizeDelta = size;
        }

        // 戻るボタンが押された時の処理
        private void OnBackButton()
        {
            this.gameObject.SetActive(false);
            this.inputUI.gameObject.SetActive(true);
        }

        // ルーム選択時の処理
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
            ClientDisconectBehaviour.Instance.SetupBeforeClientStart();
            netMgr.StartClient();
            this.gameObject.SetActive(false);
        }



    }
}
