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
        private Text currentUserNum;
        [SerializeField]
        private Text limitUserNum;
        

        private HostRoomInfo roomInfo;
        private RoomSearchUI roomSearchUI;

        private RectTransform rectTransform;

        private void Awake()
        {
            button.onClick.AddListener(this.OnClickButton);
            this.rectTransform = this.GetComponent<RectTransform>();
        }

        internal bool IsSameHostRoom(HostRoomInfo info)
        {
            return (roomInfo == info);
        }

        public void Setup(RoomSearchUI searchUI, HostRoomInfo info)
        {
            this.roomSearchUI = searchUI;
            this.roomInfo = info;
            roomName.text = info.roomInfo.name;
            currentUserNum.text = info.roomInfo.currentUser.ToString();
            limitUserNum.text = info.roomInfo.capacity.ToString();
        }

        public void SetPosition(Vector2 pos)
        {
            this.rectTransform.anchoredPosition = pos;
        }

        private void OnClickButton()
        {
            if (this.roomSearchUI)
            {
                this.roomSearchUI.OnClickRoomButton(this.roomInfo);
            }
        }
    }
}
