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
        private GameObject selectButtonObject;


        private InformationInputUI inputUI;

        public void SetUp(InformationInputUI ui)
        {
            this.inputUI = ui;
        }

        // Start is called before the first frame update
        private void OnEnable()
        {
            LANRoomManager.Instance.OnFindNewRoom = OnFindNewRoom;
            LANRoomManager.Instance.OnLoseRoom = OnLoseRoom;
            LANRoomManager.Instance.StartClientThread();
        }
        private void OnDisable()
        {
            LANRoomManager.Instance.OnFindNewRoom = null;
            LANRoomManager.Instance.OnLoseRoom = null;
            LANRoomManager.Instance.Stop();
        }

        private void OnFindNewRoom(HostRoomInfo info)
        {
        }

        private void OnLoseRoom(HostRoomInfo info)
        {
        }

    }
}
