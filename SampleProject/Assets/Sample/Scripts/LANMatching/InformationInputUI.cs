using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LANMatching.Sample
{
    public class InformationInputUI : MonoBehaviour
    {
        public static string playerName;
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
            this.searchRoomBtn.onClick.AddListener(this.OnClickSerchRoom);
            this.createRoomBtn.onClick.AddListener(this.OnClickHostRoom);

            this.playerNameField.text = SystemInfo.deviceName;
            this.playerNameField.text = System.Environment.UserName;
            this.roomNameField.text = SystemInfo.deviceName;
        }

        void OnClickSerchRoom()
        {
            playerName = this.playerNameField.text;
            this.roomSearchUI.Setup( this);
            this.roomSearchUI.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }

        void OnClickHostRoom()
        {
            playerName = this.playerNameField.text;
            this.roomHostUI.Setup(this , this.roomNameField.text );
            this.roomHostUI.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }

    }
}
