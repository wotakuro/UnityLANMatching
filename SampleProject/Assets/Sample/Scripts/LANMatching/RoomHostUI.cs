using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LANMatching.Sample
{
    /// <summary>
    /// ホストとして起動した時のUI周りの処理
    /// </summary>
    public class RoomHostUI : MonoBehaviour
    {
        // 戻るボタン
        [SerializeField]
        private Button backButton;
        // 開始ボタン
        [SerializeField]
        private Button startGameButton;

        // 開始後にPlayer名などの設定を同期するためのNetworkPrefab
        [SerializeField]
        private GameObject playerNameSyncPrefab;

        // ルーム名
        private string roomName;
        // 開始時に戻る用のUI
        private InformationInputUI inputUI;

        // 初期化処理
        public void Setup(InformationInputUI ui,string name)
        {
            this.roomName = name;
            this.inputUI = ui;
        }

        // Awake処理
        private void Awake()
        {
            this.backButton.onClick.AddListener(this.OnBackButton);
            this.startGameButton.onClick.AddListener(this.OnStartSessionButton);
        }

        // Update処理
        private void Update()
        {
            // 現在接続中のユーザー数を更新します
            int userNum = MLAPI.NetworkManager.Singleton.ConnectedClientsList.Count;
            LANRoomManager.Instance.hostRoomInfo.currentUser = (byte)(userNum);
        }

        // OnEnable処理
        private void OnEnable()
        {
            var mlapiTransport = MLAPI.NetworkManager.Singleton.NetworkConfig.NetworkTransport as MLAPI.Transports.UNET.UNetTransport;
            int port = mlapiTransport.ServerListenPort; ;
            int limitUser= mlapiTransport.MaxConnections;


            var roomInfo = new RoomInfo(this.roomName,port , (byte)limitUser);
            LANRoomManager.Instance.hostRoomInfo = roomInfo;
            LANRoomManager.Instance.StartHostThread();

            // Start MLAPI Host
            MLAPI.NetworkManager.Singleton.StartHost();

            // Spawn SyncObject
            var syncBehaviour = GameObject.Instantiate(playerNameSyncPrefab).GetComponent<NetworkSettingSyncBehaviour>();
            syncBehaviour.NetworkObject.Spawn();
        }


        // OnDisable処理
        private void OnDisable()
        {
            if (LANRoomManager.Instance)
            {
                LANRoomManager.Instance.Stop();
            }
        }

        // 戻るボタンを押した時の処理
        private void OnBackButton()
        {
            this.gameObject.SetActive(false);
            this.inputUI.gameObject.SetActive(true);
            MLAPI.NetworkManager.Singleton.StopHost();
        }

        //　開始するボタンを押した時の処理
        private void OnStartSessionButton()
        {
            LANRoomManager.Instance.Stop();
            // シーンを移ります
            MLAPI.SceneManagement.NetworkSceneManager.SwitchScene("AfterMatching");
        }
    }
}