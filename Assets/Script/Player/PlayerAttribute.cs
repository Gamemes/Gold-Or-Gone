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
            CreatePlayerUI();
        }
        void CreatePlayerUI()
        {
            Debug.Assert(playerUIObject != null);
            if (playerUIObject == null)
                return;
            var pui = Instantiate(playerUIObject);
            var uimanager = pui.GetComponent<Player.PlayerUIManager>();
            uimanager.targetPlayer = this;
            uimanager.playerTransform = transform;
            pui.name = $"{this.playerName} UI(auto create by stageManager)";
            // 重置ui object到生成的
            playerUIObject = pui;
        }
        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            playerHealth.onPlayerDead += this.OnPlayerDied;
            stageManager.onReGame += this.OnReGame;
        }
        void OnGodPlayerChange(GameObject godPlayer)
        {
            if (godPlayer.Equals(this.gameObject))
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
            this.frameInput._input.Player.Enable();
            this.frameInput._input.God.Disable();
            this.playerController.enabled = true;
            this.playerController._activate = true;
            this.godController.enabled = false;
            rb.bodyType = RigidbodyType2D.Kinematic;
            stageManager.stageCamera.Follow = transform;
        }
        void ChangeToGod()
        {
            this.frameInput._input.Player.Disable();
            this.frameInput._input.God.Enable();
            this.playerController._activate = false;
            this.godController.enabled = true;
            this.playerHealth.energy = 0;
            rb.bodyType = RigidbodyType2D.Static;
            playerAnimation.changeState(PlayerAnimationController.PlayerState.Idle);
        }
        private void OnDestroy()
        {
            Destroy(playerUIObject);
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