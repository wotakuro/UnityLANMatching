using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LANMatching.Sample
{
    /// <summary>
    /// 他のユーザーを待っている時のUI
    /// </summary>
    public class WaitingOtherClientUI : MonoBehaviour
    {
        // ユーザー 一覧がリストされるScrollView
        [SerializeField]
        private ScrollRect scrollRect;

        // 戻るボタン
        [SerializeField]
        private Button backButton;

        // 戻る先のUI
        private InformationInputUI inputUI;

        //　初期化処理
        public void Setup(InformationInputUI ui)
        {
            this.inputUI = ui;
        }

        //　Awake処理
        private void Awake()
        {
            backButton.onClick.AddListener(OnClickBack);
        }

        // 戻るボタンを押した時の処理
        private void OnClickBack()
        {
            this.gameObject.SetActive(false);
            this.inputUI.gameObject.SetActive(true);
            MLAPI.NetworkManager.Singleton.StopClient();
        }
    }
}
