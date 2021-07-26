using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LANMatching.Sample
{
    public class InformationInputUI : MonoBehaviour
    {
        [SerializeField]
        private InputField playerNameField;
        [SerializeField]
        private InputField roomNameField;

        [SerializeField]
        private Button searchRoomBtn;
        [SerializeField]
        private Button createRoomBtn;

        [SerializeField]
        private RoomHostUI roomHostUI;
        [SerializeField]
        private RoomSearchUI roomSearchUI;


        void Awake()
        {
            searchRoomBtn.onClick.AddListener(this.OnClickSerchRoom);
            createRoomBtn.onClick.AddListener(this.OnClickHostRoom);
            playerNameField.text = SystemInfo.deviceName;
            roomNameField.text = SystemInfo.deviceName;
        }

        void OnClickSerchRoom()
        {
            roomSearchUI.SetUp( this);
            roomSearchUI.gameObject.SetActive(true);
        }

        void OnClickHostRoom()
        {
            roomHostUI.SetUp(this , this.roomNameField.text );
            roomHostUI.gameObject.SetActive(true);
        }

    }
}
