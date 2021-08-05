using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LANMatching.Sample
{
    /// <summary>
    /// ルームの情報UI
    /// </summary>
    public class RoomInfoUI : MonoBehaviour
    {
        // ルーム名
        [SerializeField]
        private Text roomName;
        // 現在のユーザー数
        [SerializeField]
        private Text currentUser;
        // 最大人数
        [SerializeField]
        private Text limitUser;

        // セットアップ処理
        public void Setup(RoomInfo roomInfo)
        {
            this.roomName.text = roomInfo.name;
            this.currentUser.text = roomInfo.currentUser.ToString();
            this.limitUser.text = roomInfo.capacity.ToString();
        }

        // 更新処理
        private void Update()
        {
            if (!NetworkSettingSyncBehaviour.Instance)
            {
                return;
            }
            int playerNum = NetworkSettingSyncBehaviour.Instance.GetAllPlayers().Count;
            this.currentUser.text = playerNum.ToString();
        }
    }
}