using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;

namespace LANMatching.Sample
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkSettingSyncBehaviour : NetworkBehaviour
    {
        public static NetworkSettingSyncBehaviour Instance { get; private set; }
        private Dictionary<ulong, string> playerNames = new Dictionary<ulong, string>();        

        public void Awake()
        {
            Instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);
        }

        private void OnDestroy()
        {

            NetworkManager.OnClientDisconnectCallback -= OnDisconnectClient;
            Instance = null;
        }

        private void Start()
        {
            NetworkManager.OnClientDisconnectCallback += OnDisconnectClient;
            RequestNamesServerRpc();
            SetNameServerRpc(InformationInputUI.playerName);
        }

        private void OnDisconnectClient(ulong client)
        {
            playerNames.Remove(client);
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable,RequireOwnership =false)]
        private void SetNameServerRpc(string name ,ServerRpcParams rpcParams=default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;
            
            playerNames[clientId] = name;
            ClientRpcParams clientRpc = default;
            SetNameClientRpc(clientId, name,clientRpc);
        }

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

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void SetNameClientRpc(ulong clientId, string name,ClientRpcParams rpcParams= default)
        {
            playerNames[clientId] = name;
        }


    }
}