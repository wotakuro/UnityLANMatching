using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MLAPI;

namespace LANMatching.Sample
{
    public class StopGameButton : MonoBehaviour
    {
        [SerializeField]
        private Button stopBtn;

        private void Awake()
        {
            stopBtn.onClick.AddListener(this.OnClickStop);
        }

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
                    netMgr.StopClient();
                }
            }
            SceneManager.LoadScene("LanMatching");
        }
    }

}