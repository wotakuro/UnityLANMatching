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

        // 戻るボタン
        [SerializeField]
        private Button backButton;

        // ルームの情報UI
        [SerializeField]
        private RoomInfoUI roomInfoUI;

        // 戻る先のUI
        private InformationInputUI inputUI;

        //　初期化処理
        public void Setup(RoomInfo roomInfo,InformationInputUI ui)
        {
            this.roomInfoUI.Setup(roomInfo);
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

            ClientDisconectBehaviour.Instance.SetupBeforeClientStop();
            MLAPI.NetworkManager.Singleton.StopClient();
        }
    }
}
