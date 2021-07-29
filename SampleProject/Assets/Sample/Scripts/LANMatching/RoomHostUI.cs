using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LANMatching.Sample
{
    public class RoomHostUI : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect scrollRect;
        [SerializeField]
        private Button backButton;
        [SerializeField]
        private Button startGameButton;
        [SerializeField]
        private GameObject playerNameSyncPrefab;

        private string roomName;
        private InformationInputUI inputUI;


        public void Setup(InformationInputUI ui,string name)
        {
            this.roomName = name;
            this.inputUI = ui;
        }

        private void Awake()
        {
            this.backButton.onClick.AddListener(this.OnBackButton);
            this.startGameButton.onClick.AddListener(this.OnStartGameButton);
        }

        private void Update()
        {
            // update current user num
            int userNum = MLAPI.NetworkManager.Singleton.ConnectedClientsList.Count;
            LANRoomManager.Instance.hostRoomInfo.currentUser = (byte)(userNum);
        }

        private void OnEnable()
        {
            var mlapiTransport = MLAPI.NetworkManager.Singleton.NetworkConfig.NetworkTransport as MLAPI.Transports.UNET.UNetTransport;
            int port = mlapiTransport.ServerListenPort; ;
            int limitUser= mlapiTransport.MaxConnections;


            var roomInfo = new RoomInfo(this.roomName,port , (byte)limitUser);
            LANRoomManager.Instance.hostRoomInfo = roomInfo;
            LANRoomManager.Instance.StartHostThread();

            // Start MLAPI Host
            MLAPI.NetworkManager.Singleton.OnClientConnectedCallback += OnConnectClient;
            MLAPI.NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnectClient;
            MLAPI.NetworkManager.Singleton.StartHost();

            //
            var syncBehaviour = GameObject.Instantiate(playerNameSyncPrefab).GetComponent<NetworkSettingSyncBehaviour>();
            syncBehaviour.NetworkObject.Spawn();
        }

        private void OnConnectClient(ulong clientID)
        {
            Debug.Log("OnConnectClient " + clientID);
        }
        private void OnDisconnectClient(ulong clientID)
        {
            Debug.Log("OnDisconnectClient " + clientID);
        }

        private void OnDisable()
        {
            if (LANRoomManager.Instance)
            {
                LANRoomManager.Instance.Stop();
            }
        }


        private void OnBackButton()
        {
            this.gameObject.SetActive(false);
            this.inputUI.gameObject.SetActive(true);
            MLAPI.NetworkManager.Singleton.StopHost();
        }
        private void OnStartGameButton()
        {
            LANRoomManager.Instance.Stop();
            // シーンを移ります
            MLAPI.SceneManagement.NetworkSceneManager.SwitchScene("AfterMatching");
        }
    }
}