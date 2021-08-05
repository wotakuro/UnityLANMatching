using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MLAPI;

namespace LANMatching.Sample
{
    /// <summary>
    /// 停止するボタン
    /// </summary>
    public class ActiveGameObjectOnHost : MonoBehaviour
    {
        // Start処理
        private void Awake()
        {
            var netMgr = NetworkManager.Singleton;
            this.gameObject.SetActive (netMgr && netMgr.IsHost) ;
        }
    }

}