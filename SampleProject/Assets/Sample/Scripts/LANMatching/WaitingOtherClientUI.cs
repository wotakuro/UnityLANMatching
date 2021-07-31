using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LANMatching.Sample
{
    public class WaitingOtherClientUI : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect scrollRect;
        [SerializeField]
        private Button backButton;

        private InformationInputUI inputUI;

        public void Setup(InformationInputUI ui)
        {
            this.inputUI = ui;
        }
        private void Awake()
        {
            backButton.onClick.AddListener(OnClickBack);
        }

        private void OnClickBack()
        {
            this.gameObject.SetActive(false);
            this.inputUI.gameObject.SetActive(true);
            MLAPI.NetworkManager.Singleton.StopClient();
        }
    }
}
