using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LANMatching.Sample
{
    /// <summary>
    /// ���[���̏��UI
    /// </summary>
    public class RoomInfoUI : MonoBehaviour
    {
        // ���[����
        [SerializeField]
        private Text roomName;
        // ���݂̃��[�U�[��
        [SerializeField]
        private Text currentUser;
        // �ő�l��
        [SerializeField]
        private Text limitUser;

        // �Z�b�g�A�b�v����
        public void Setup(RoomInfo roomInfo)
        {
            this.roomName.text = roomInfo.name;
            this.currentUser.text = roomInfo.currentUser.ToString();
            this.limitUser.text = roomInfo.capacity.ToString();
        }

        // �X�V����
        private void Update()
        {
            if (!NetworkSettingSyncBehaviour.Instance)
            {
                return;
            }
            int playerNum = NetworkSettingSyncBehaviour.Instance.GetAllPlayers().Count;
            this.currentUser.text = playerNum.ToString();
        }
    }
}