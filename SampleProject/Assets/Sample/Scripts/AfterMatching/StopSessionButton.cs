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
    public class StopSessionButton : MonoBehaviour
    {
        // 停止ボタン
        [SerializeField]
        private Button stopBtn;

        // Awake処理
        private void Awake()
        {
            stopBtn.onClick.AddListener(this.OnClickStop);
        }

        // 停止ボタンが押されたときの処理
        private void OnClickStop()
        {
            var netMgr = NetworkManager.Singleton;
            if (netMgr)
            {
                if( netMgr.IsHost)
                {
                    netMgr.StopHost();
                }
                else
                {
                    ClientDisconectBehaviour.Instance.SetupBeforeClientStop();
                    netMgr.StopClient();
                }
            }
            SceneManager.LoadScene("LanMatching");
        }
    }

}