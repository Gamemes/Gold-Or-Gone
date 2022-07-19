using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAttribute : MonoBehaviour
    {
        /// <summary>
        /// 如果是本地模式那么一直为true.
        /// 如果是联机模式那么只有本地玩家为true.
        /// </summary>
        public bool isLocalPlayer = true;
        public FrameInput frameInput { get; private set; }
        public PlayerController playerController { get; private set; }
        public GodController godController { get; private set; }
        public PlayerHealth playerHealth { get; private set; }
        public PlayerAnimationController playerAnimation { get; private set; }
        public Rigidbody2D rb { get; private set; }
        public SpriteRenderer spriteRenderer { get; private set; }
        public string playerName = "";
        private Manager.StageManager stageManager;
        /// <summary>
        /// 玩家的ui对象, 为玩家提供了 血量|能量|名称 的显示
        /// </summary>
        public GameObject playerUIObject;
        public Mirror.NetworkIdentity networkIdentity { get; private set; } = null;
        void Awake()
        {
            frameInput = GetComponent<FrameInput>();
            playerController = GetComponent<PlayerController>();
            godController = GetComponent<GodController>();
            playerHealth = GetComponent<PlayerHealth>();
            playerAnimation = GetComponent<PlayerAnimationController>();
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            networkIdentity = GetComponent<Mirror.NetworkIdentity>();
            stageManager = Manager.MyGameManager.instance.currentStage;
            if (playerName == "")
                playerName = gameObject.name;
            stageManager.onGlodPlayerChange += this.OnGodPlayerChange;
        }
        private void Start()
        {
            playerHealth.onPlayerDead += this.OnPlayerDied;
            stageManager.onReGame += this.OnReGame;
        }
        void OnGodPlayerChange(GameObject godPlayer)
        {
            if (this.gameObject.Equals(godPlayer))
            {
                ChangeToGod();
            }
            else
            {
                ChangeToHuman();
            }
        }
        void ChangeToHuman()
        {
            if (playerController.enabled)
            {
                playerController.OnEnable();
            }
            else
            {
                playerController.enabled = true;
            }
            if (!godController.enabled)
            {
                godController.OnDisable();
            }
            else
            {

                godController.enabled = false;
            }
        }
        void ChangeToGod()
        {
            if (!playerController.enabled)
            {
                playerController.OnDisable();
            }
            else
            {
                playerController.enabled = false;
            }
            if (godController.enabled)
            {
                godController.OnEnable();
            }
            else
            {

                godController.enabled = true;
            }
        }
        private void OnDestroy()
        {
            stageManager.onRemovePlayer?.Invoke(this.gameObject);
            stageManager.onGlodPlayerChange -= this.OnGodPlayerChange;
        }
        void OnPlayerDied()
        {
            if (!isLocalPlayer)
                return;
            this.playerController.enabled = false;
            this.godController.enabled = false;
            playerAnimation.changeState(PlayerAnimationController.PlayerState.Death);
            StartCoroutine(Utils.Utils.DelayInvoke(() => { stageManager.ReGame(); }, 2f));
        }
        // Update is called once per frame
        void Update()
        {
        }
        void OnReGame()
        {
            Debug.Log($"{gameObject} regame");
            ChangeToHuman();
            this.playerHealth.ReSetHealth();
            playerAnimation.changeState(PlayerAnimationController.PlayerState.Idle);
        }
    }
}