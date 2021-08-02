using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LANMatching.Sample
{
    /// <summary>
    /// 最初の入力画面
    /// </summary>
    public class InformationInputUI : MonoBehaviour
    {
        // 自身が入れた名前をほかから参照しやすいようにStaticで
        public static string playerName;

        // プレイヤー名入力場所
        [SerializeField]
        private InputField playerNameField;
        // ルーム名入力場所
        [SerializeField]
        private InputField roomNameField;

        // ルームを探すボタン
        [SerializeField]
        private Button searchRoomBtn;
        // ルームをつくるボタン
        [SerializeField]
        private Button createRoomBtn;

        // ルームをホストする時のUI画面
        [SerializeField]
        private RoomHostUI roomHostUI;
        // ルームを探す時のUI画面
        [SerializeField]
        private RoomSearchUI roomSearchUI;

        // Awake処理
        void Awake()
        {
            this.searchRoomBtn.onClick.AddListener(this.OnClickSerchRoom);
            this.createRoomBtn.onClick.AddListener(this.OnClickHostRoom);

            this.playerNameField.text = GetPlayerName();
            this.roomNameField.text = GetRoomName();
        }

        // プレイヤー名の取得をします
        private string GetPlayerName()
        {
            var str = PlayerPrefs.GetString("PlayerName");
            if (string.IsNullOrEmpty(str))
            {
                str = System.Environment.UserName;
            }
            return str;
        }
        // ルーム名の取得をします
        private string GetRoomName()
        {
            var str = PlayerPrefs.GetString("RoomName");
            if (string.IsNullOrEmpty(str))
            {
                str = SystemInfo.deviceName;
            }
            return str;
        }
        // プレイヤー名のセーブ
        private void SaveName()
        {
            PlayerPrefs.SetString("PlayerName", this.playerNameField.text);
            PlayerPrefs.SetString("RoomName", this.roomNameField.text);
        }

        // ルームを探すを押された時の処理
        void OnClickSerchRoom()
        {
            playerName = this.playerNameField.text;
            this.roomSearchUI.Setup( this);
            this.roomSearchUI.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
            SaveName();
        }

        // ルームをつくるを押された時の処理
        void OnClickHostRoom()
        {
            playerName = this.playerNameField.text;
            this.roomHostUI.Setup(this , this.roomNameField.text );
            this.roomHostUI.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
            SaveName();
        }

    }
}
