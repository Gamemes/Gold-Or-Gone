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
        private Manager.StageManager stageManager => Manager.StageManager.CurrentStageManager();
        public bool isGod => this.gameObject.Equals(stageManager.GodPlayer);
        public bool isHuman => !isGod;
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
        public Action onPlayerChangeToHuman;
        public Action onPlayerChangeToGod;
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
            onPlayerChangeToHuman?.Invoke();
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
            onPlayerChangeToGod?.Invoke();
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
            stageManager.stageInfo.ShowInfo("上帝玩家胜利!");
            stageManager.GameOver();
        }
        // Update is called once per frame
        void Update()
        {
        }
        void OnReGame()
        {
            Debug.Log($"{gameObject} regame");
            ChangeToHuman();
            this.transform.position = new Vector3(0, 0, 0);
            this.playerHealth.ReSetHealth();
            this.playerController.ReSetController();
            playerAnimation.changeState(PlayerAnimationController.PlayerState.Idle);
        }
    }
}