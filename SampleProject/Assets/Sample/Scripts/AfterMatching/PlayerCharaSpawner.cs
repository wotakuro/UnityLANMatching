using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

namespace LANMatching.Sample
{
    public class PlayerCharaSpawner : MonoBehaviour
    {
        public GameObject playerChara;
        // Start is called before the first frame update
        void Start()
        {
            // ホストなら生成
            if (!NetworkManager.Singleton.IsHost) {
                return;
            }
            var allPlayer = NetworkSettingSyncBehaviour.Instance.GetAllPlayers();
            int idx = 0;
            foreach( var kvs in allPlayer)
            {
                var gmo = GameObject.Instantiate(playerChara,new Vector3( idx * 2-2,3,0),Quaternion.identity);
                ulong clientId = kvs.Key;
                gmo.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
                gmo.GetComponent<UTJ.MLAPISample.CharacterMoveController>().SetPlayerName(kvs.Value);
                ++idx;
            }
        }

    }
}