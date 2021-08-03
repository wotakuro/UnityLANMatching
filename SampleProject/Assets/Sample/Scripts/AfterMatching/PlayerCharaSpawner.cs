using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

namespace LANMatching.Sample
{

    /// <summary>
    /// マッチングしてシーン遷移後に、操作キャラクターをSpawnします
    /// </summary>
    public class PlayerCharaSpawner : MonoBehaviour
    {
        // Network上でSpawnしたいPrefab
        // NetworkManagerで登録している必要があります
        [SerializeField]
        private GameObject playerChara;


        // Start処理
        void Start()
        {
            // Editor実行中に、このシーンを開いたときに行われるべき初期化が行われていないので…
            // 強引に初期化して対処します
#if UNITY_EDITOR
            if (!NetworkManager.Singleton)
            {
                string networkManagerPrefabPath = "Assets/Sample/Prefabs/LANMatching/NetworkManager.prefab";
                var netMgrPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(networkManagerPrefabPath);
                GameObject.Instantiate(netMgrPrefab);
                NetworkManager.Singleton.StartHost();
                var gmo = GameObject.Instantiate(playerChara,Vector3.zero, Quaternion.identity);
                gmo.GetComponent<NetworkObject>().Spawn();

                return;
            }
#endif
            // ホスト以外は処理しません
            if (!NetworkManager.Singleton.IsHost) {
                return;
            }
            // プレイヤー用のオブジェクトをNetworkでSpawnします
            var allPlayer = NetworkSettingSyncBehaviour.Instance.GetAllPlayers();
            int idx = 0;
            foreach( var kvs in allPlayer)
            {
                var gmo = GameObject.Instantiate(playerChara,new Vector3( idx * 2-2,3,0),Quaternion.identity);
                ulong clientId = kvs.Key;
                gmo.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
                // 名前をセットします
                gmo.GetComponent<CharacterMoveController>().SetPlayerName(kvs.Value);
                ++idx;
            }
        }

    }
}