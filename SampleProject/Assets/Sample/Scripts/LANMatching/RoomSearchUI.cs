using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LANMatching.Sample
{
    public class RoomSearchUI : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect scrollRect;
        [SerializeField]
        private Button backButton;
        [SerializeField]
        private GameObject selectButtonObject;


        private InformationInputUI inputUI;

        public void SetUp(InformationInputUI ui)
        {
            this.inputUI = ui;
        }

        // Start is called before the first frame update
        private void OnEnable()
        {
            LANRoomManager.Instance.StartClientThread();
        }
        private void OnDisable()
        {
            LANRoomManager.Instance.Stop();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
