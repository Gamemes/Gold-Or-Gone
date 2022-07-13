using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAttribute : MonoBehaviour
    {
        // Start is called before the first frame update
        public FrameInput frameInput { get; private set; }
        public PlayerController playerController { get; private set; }
        public GodController godController { get; private set; }
        public PlayerHealth playerHealth { get; private set; }
        public PlayerAnimationController playerAnimation { get; private set; }
        public Rigidbody2D rb { get; private set; }
        public SpriteRenderer spriteRenderer { get; private set; }
        public string playerName = "";
        public Action<int> onEneryChange;
        /// <summary>
        /// 能量值, 达3的时候变成上帝
        /// </summary>
        public int energy
        {
            get => _energy;
            set
            {
                if (value == 3)
                    Manager.MyGameManager.instance.currentStage.ChangeGloadPlayer(gameObject);
                _energy = value % 3;
                onEneryChange?.Invoke(_energy);
            }
        }
        private int _energy = 0;
        private Manager.StageManager stageManager;
        public GameObject playerUIObject;
        void Awake()
        {
            frameInput = GetComponent<FrameInput>();
            playerController = GetComponent<PlayerController>();
            godController = GetComponent<GodController>();
            playerHealth = GetComponent<PlayerHealth>();
            playerAnimation = GetComponent<PlayerAnimationController>();
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
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
            stageManager.onGameStart += () => onEneryChange?.Invoke(energy);
            playerHealth.onPlayerDead += this.OnPlayerDied;
            stageManager.onReGame += this.OnReGame;
        }
        void OnGodPlayerChange(GameObject godPlayer)
        {
            if (godPlayer.Equals(this.gameObject))
            {
                this.frameInput._input.Player.Disable();
                this.frameInput._input.God.Enable();
                this.playerController.enabled = false;
                this.godController.enabled = true;
                this._energy = 0;
                rb.bodyType = RigidbodyType2D.Static;
                if (stageManager.isOnline)
                {
                    var t = stageManager.stagePlayers.Find((player) => { return !player.Equals(godPlayer); });
                }
            }
            else
            {
                this.frameInput._input.Player.Enable();
                this.frameInput._input.God.Disable();
                this.playerController.enabled = true;
                this.godController.enabled = false;
                rb.bodyType = RigidbodyType2D.Kinematic;
                stageManager.stageCamera.Follow = transform;
                if (stageManager.isOnline)
                {

                }
                else
                {

                }
            }
        }
        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Destroy(playerUIObject);
            stageManager.stagePlayers.Remove(this.gameObject);
            stageManager.onGlodPlayerChange -= this.OnGodPlayerChange;
        }
        void OnPlayerDied()
        {
            this.playerController.enabled = false;
            this.godController.enabled = false;
            playerAnimation.changeState(PlayerAnimationController.PlayerState.Death);
            stageManager.ReGame();
        }
        // Update is called once per frame
        void Update()
        {
        }
        void OnReGame()
        {
            this.playerController.enabled = true;
            this.godController.enabled = false;
            this.playerHealth.ReSetHealth();
            playerAnimation.changeState(PlayerAnimationController.PlayerState.Idle);
            this.energy = 0;
        }
    }
}