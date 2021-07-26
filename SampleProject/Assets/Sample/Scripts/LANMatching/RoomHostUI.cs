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


        public void SetUp(InformationInputUI ui,string name)
        {
            this.roomName = name;
            this.inputUI = ui;
        }


        // Start is called before the first frame update
        void Awake()
        {

        }

        private void OnEnable()
        {
            var roomInfo = new RoomInfo(this.roomName, 12000, 4);
            LANRoomManager.Instance.hostRoomInfo = roomInfo;
            LANRoomManager.Instance.StartHostThread();
        }
        private void OnDisable()
        {
            LANRoomManager.Instance.Stop();
        }


        private void OnBackButton()
        {
        }
        private void OnStartGameButton()
        {
            LANRoomManager.Instance.StartHostThread();
        }
    }
}