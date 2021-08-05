using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LANMatching.Sample
{
    /// <summary>
    /// ルーム選択ボタン
    /// </summary>
    internal class RoomSelectButton : MonoBehaviour
    {
        // 選択用のボタン
        [SerializeField]
        private Button button;
        // ルーム名のテキスト
        [SerializeField]
        private Text roomName;
        // 現在のユーザー数のテキスト
        [SerializeField]
        private Text currentUserNum;
        // 最大ユーザー数のテキスト
        [SerializeField]
        private Text limitUserNum;


        // ボタンと紐づくHostルーム情報
        private HostRoomInfo hostRoomInfo;

        // ボタン押したときに、処理してほしいことがあるので…ルーム探すUIを保持します
        private RoomSearchUI roomSearchUI;

        // RectTransformのキャッシュ
        private RectTransform rectTransform;

        // Awake処理
        private void Awake()
        {
            button.onClick.AddListener(this.OnClickButton);
            this.rectTransform = this.GetComponent<RectTransform>();
        }

        // 引数に渡されたHostRoomかチェックして返します
        internal bool IsSameHostRoom(HostRoomInfo info)
        {
            return (hostRoomInfo == info);
        }

        // セットアップ処理
        public void Setup(RoomSearchUI searchUI, HostRoomInfo info)
        {
            this.roomSearchUI = searchUI;
            this.hostRoomInfo = info;
            roomName.text = info.roomInfo.name;
            currentUserNum.text = info.roomInfo.currentUser.ToString();
            limitUserNum.text = info.roomInfo.capacity.ToString();
        }

        // 座標の更新処理
        public void SetPosition(Vector2 pos)
        {
            this.rectTransform.anchoredPosition = pos;
        }

        // ボタンが押された時の処理
        private void OnClickButton()
        {
            if (this.roomSearchUI && this.hostRoomInfo!=null && this.hostRoomInfo.roomInfo != null &&
                this.hostRoomInfo.roomInfo.currentUser < this.hostRoomInfo.roomInfo.capacity)
            {
                this.roomSearchUI.OnClickRoomButton(this.hostRoomInfo);
            }
        }
    }
}
