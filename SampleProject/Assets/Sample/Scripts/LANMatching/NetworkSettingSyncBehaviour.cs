using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;

namespace LANMatching.Sample
{

    /// <summary>
    /// MLAPIで接続直後に、設定の同期をします
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkSettingSyncBehaviour : NetworkBehaviour
    {
        // 生成されたインスタンスへのアクセス
        public static NetworkSettingSyncBehaviour Instance { get; private set; }

        //　プレイヤーごとの名前（クライアントIDがキー)
        private Dictionary<ulong, string> playerNames = new Dictionary<ulong, string>();

        // プレイヤーごとの名前を取得します
        public Dictionary<ulong,string> GetAllPlayers()
        {
            return this.playerNames;
        }


        //　Awake処理
        public void Awake()
        {
            Instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);
        }

        //　OnDestroy処理
        private void OnDestroy()
        {
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnDisconnectClient;
            }
            Instance = null;
        }

        // Start処理
        private void Start()
        {
            NetworkManager.OnClientDisconnectCallback += OnDisconnectClient;
            RequestNamesServerRpc();
            SetNameServerRpc(InformationInputUI.playerName);
        }

        // 切断時の処理
        private void OnDisconnectClient(ulong client)
        {
            playerNames.Remove(client);
        }

        // クライアント→ホストに対して、自分の名前を伝えます
        [ServerRpc(Delivery = RpcDelivery.Reliable,RequireOwnership =false)]
        private void SetNameServerRpc(string name ,ServerRpcParams rpcParams=default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;

            SetPlayerName(clientId, name);
            ClientRpcParams clientRpc = default;
            SetNameClientRpc(clientId, name,clientRpc);
        }

        // ホストに対して、全プレイヤー分の名前を要求します
        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
        private void RequestNamesServerRpc( ServerRpcParams rpcParams = default)
        {
            ulong targetClientId = rpcParams.Receive.SenderClientId;
            ClientRpcParams clientParam = default;
            clientParam.Send.TargetClientIds = new ulong[] { targetClientId };
            foreach ( var kvs in playerNames)
            {
                SetNameClientRpc(kvs.Key, kvs.Value, clientParam);
            }
        }

        // ホスト→クライアントで、ホストが持っている名前を伝えます
        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void SetNameClientRpc(ulong clientId, string name,ClientRpcParams rpcParams= default)
        {
            SetPlayerName(clientId, name);
        }

        // ローカル上のプレイヤー名辞書をセットします
        private void SetPlayerName(ulong clientId , string name)
        {
            playerNames[clientId] = name;
        }


    }
}