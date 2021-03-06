using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LANMatching.Sample
{
    /// <summary>
    /// クライアントが切断されたときに行われる処理周り
    /// </summary>
    public class ClientDisconectBehaviour : MonoBehaviour
    {
        // 生成されたInstance取得
        public static ClientDisconectBehaviour Instance
        {
            get;private set;
        }
        // 切断判定を実行すべきかのフラグ
        private bool shouldExecute = false;

        // Awake処理
        private void Awake()
        {
            if (Instance)
            {
                GameObject.Destroy( this.gameObject );
                return;
            }
            Instance = this;
            GameObject.DontDestroyOnLoad( this.gameObject);
        }

        // OnDestroy処理
        private void OnDestroy()
        {
            if(this == Instance)
            {
                Instance = null;
            }
        }

        // クライアント開始前に呼ぶ処理
        public void SetupBeforeClientStart()
        {
            var netMgr = MLAPI.NetworkManager.Singleton;
            if (netMgr)
            {
                netMgr.OnClientConnectedCallback += OnClientConnect;
                netMgr.OnClientDisconnectCallback += OnClientDisconnect;
            }
        }


        // クライアント停止時に呼ぶ処理
        public void SetupBeforeClientStop()
        {
            var netMgr = MLAPI.NetworkManager.Singleton;
            if (netMgr)
            {
                netMgr.OnClientConnectedCallback -= OnClientConnect;
            }
            shouldExecute = false;
        }

        // クライアントが接続されたときの処理
        private void OnClientConnect(ulong clientId)
        {
            var netMgr = MLAPI.NetworkManager.Singleton;
            if (netMgr && clientId == netMgr.LocalClientId)
            {
                shouldExecute = true;
                netMgr.OnClientDisconnectCallback -= OnClientDisconnect;
            }
        }
        // クライアントが切断されたときの処理
        // StartClientしてタイムアウトした時を想定
        private void OnClientDisconnect(ulong clientId)
        {
            var netMgr = MLAPI.NetworkManager.Singleton;
            netMgr.OnClientDisconnectCallback -= OnClientDisconnect;
            this.ExecuteDisconnect();
        }

        // 自身がDisconnectされたときに行う処理
        private void ExecuteDisconnect()
        {
            var netMgr = MLAPI.NetworkManager.Singleton;
            SceneManager.LoadScene("LanMatching");
            shouldExecute = false;
            netMgr.OnClientDisconnectCallback -= OnClientDisconnect;
            netMgr.OnClientConnectedCallback -= OnClientConnect;
        }

        // Update処理
        private void Update()
        {
            if (!shouldExecute) { return; }
            var netMgr = MLAPI.NetworkManager.Singleton;
            // 切断を検知したら最初のシーンに戻ります
            if (netMgr && !netMgr.IsConnectedClient)
            {
                ExecuteDisconnect();
            }
        }
    }
}