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

        private void OnEnable()
        {
            var mlapiTransport = MLAPI.NetworkManager.Singleton.NetworkConfig.NetworkTransport as MLAPI.Transports.UNET.UNetTransport;
            int port = mlapiTransport.ServerListenPort; ;


            var roomInfo = new RoomInfo(this.roomName,port , 4);
            LANRoomManager.Instance.hostRoomInfo = roomInfo;
            LANRoomManager.Instance.StartHostThread();
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
        }
        private void OnStartGameButton()
        {
            LANRoomManager.Instance.Stop();
        }
    }
}