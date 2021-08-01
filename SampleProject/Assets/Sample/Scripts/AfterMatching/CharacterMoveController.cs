using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LANMatching.Sample
{
    // キャラクターの動きのコントローラー
    public class CharacterMoveController : MLAPI.NetworkBehaviour
    {

        public TextMesh playerNameTextMesh;


        private Rigidbody rigidbodyComponent;
        private Animator animatorComponent;

        // Networkで同期する変数を作成します
        #region NETWORKED_VAR
        // Animationに流すスピード変数
        private NetworkVariable<float> speed = new NetworkVariable<float>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }, 0.0f);
        // プレイヤー名
        private NetworkVariable<string> playerName = new NetworkVariable<string>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }, "");
        #endregion NETWORKED_VAR

        private void Awake()
        {
            this.rigidbodyComponent = this.GetComponent<Rigidbody>();
            this.animatorComponent = this.GetComponent<Animator>();

            // Player名が変更になった時のコールバック指定
            this.playerName.OnValueChanged += OnChangePlayerName;
        }

        public void SetPlayerName(string name)
        {
            playerName.Value = name;
        }
        private void Start()
        {
            if (IsOwner)
            {
                // コントローラーの有効化をします
                ControllerBehaviour.Instance.Enable();
            }
        }
        private void OnDestroy()
        {
            if (IsOwner)
            {
                // コントローラーの無効化をします
                if (ControllerBehaviour.Instance)
                {
                    ControllerBehaviour.Instance.Disable();
                }
            }
        }

        // player名変更のコールバック
        void OnChangePlayerName(string prev, string current)
        {
            if (playerNameTextMesh != null)
            {
                playerNameTextMesh.text = current;
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Animatorの速度更新(歩き・走り・静止などをSpeedでコントロールしてます)
            animatorComponent.SetFloat("Speed", speed.Value);
            // オーナーとして管理している場合、ここのUpdateを呼びます
            if (IsOwner)
            {
                UpdateAsOwner();
            }
        }

        // オーナーとしての処理
        private void UpdateAsOwner()
        {
            // 移動処理
            Vector3 move = ControllerBehaviour.Instance.LPadVector;
            float speedValue = move.magnitude;

            this.speed.Value = speedValue;
            move *= Time.deltaTime * 4.0f;
            rigidbodyComponent.position += move;

            // 移動している方角に向きます
            if (move.sqrMagnitude > 0.00001f)
            {
                rigidbodyComponent.rotation = Quaternion.LookRotation(move, Vector3.up);
            }
            // 底に落ちたら適当に復帰します。
            if (transform.position.y < -10.0f)
            {
                var randomPosition = new Vector3(Random.Range(-7, 7), 5.0f, Random.Range(-7, 7));
                transform.position = randomPosition;
            }

            // 入力の通知を通知します
            ControllerBehaviour.Instance.OnUpdateEnd();
        }

    }
}