using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LANMatching.Sample
{
    /// <summary>
    /// ルームに居るユーザー一覧のUI
    /// </summary>
    public class RoomUserListUI : MonoBehaviour
    {
        //　ユーザー　一覧が表示されるScrollView
        [SerializeField]
        private ScrollRect scrollRect;

        //　ユーザー名を表示するようPrefab
        [SerializeField]
        private GameObject userNamePrefab;

        private struct InstantiatedInfo{
            public ulong clientId;
            public GameObject gameObject;
            public RectTransform rectTransform;
        }

        private Dictionary<ulong, InstantiatedInfo> instantiatedInfo = new Dictionary<ulong, InstantiatedInfo>();
        private List<InstantiatedInfo> sortBuffer = new List<InstantiatedInfo>();
        private List<ulong> buffer = new List<ulong>();

        // Update処理
        void Update()
        {
            // update player
            var syncBehaviour = NetworkSettingSyncBehaviour.Instance;
            if (!syncBehaviour) { return; }
            var players = syncBehaviour.GetAllPlayers();

            // Add User
            foreach(var kvs in players)
            {
                ulong cliId = kvs.Key;
                if (this.instantiatedInfo.ContainsKey(cliId))
                {
                    continue;
                }
                string name = kvs.Value;
                GameObject gmo = GameObject.Instantiate(this.userNamePrefab, this.scrollRect.content) as GameObject;
                gmo.GetComponentInChildren<Text>().text = name;
                var rTrans = gmo.GetComponent<RectTransform>();
                var info = new InstantiatedInfo
                {
                    clientId = cliId,
                    gameObject = gmo,
                    rectTransform = rTrans
                };
                this.instantiatedInfo.Add(cliId, info);
            }
            // Remove User
            buffer.Clear();
            foreach (var kvs in this.instantiatedInfo)
            {
                ulong clientId = kvs.Key;
                if (!players.ContainsKey(clientId))
                {
                    buffer.Add(clientId);
                }
            }
            foreach( var clientId in buffer)
            {
                var info = instantiatedInfo[clientId];
                GameObject.Destroy(info.gameObject);
                this.instantiatedInfo.Remove(clientId);
            }
            // update Position
            UpdatePosition();
        }

        // プレイヤー名の座標の更新
        private void UpdatePosition()
        {
            sortBuffer.Clear();
            foreach (var kvs in instantiatedInfo)
            {
                sortBuffer.Add(kvs.Value);
            }
            sortBuffer.Sort((a, b) => { return (int)((long)a.clientId - (long)b.clientId); });
            int idx = 0;
            foreach( var obj in sortBuffer)
            {
                obj.rectTransform.anchoredPosition = new Vector2(5, -5 - 30 * idx);
                ++idx;
            }
            var size = scrollRect.content.sizeDelta;
            size.y = sortBuffer.Count * 30 + 10;
            scrollRect.content.sizeDelta = size;
        }
    }
}
