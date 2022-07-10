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
        public GodController godCntroller { get; private set; }
        public PlayerHealth playerHealth { get; private set; }
        public Rigidbody2D rb { get; private set; }
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
                // if (value < 0 || value > 3)
                //     return;
                if (value == 3)
                    Manager.MyGameManager.instance.currentStage.ChangeGloadPlayer(gameObject);
                _energy = value % 3;
                onEneryChange?.Invoke(_energy);
            }
        }
        private int _energy = 0;
        private Manager.StageManager stageManager;
        void Awake()
        {
            frameInput = GetComponent<FrameInput>();
            playerController = GetComponent<PlayerController>();
            godCntroller = GetComponent<GodController>();
            playerHealth = GetComponent<PlayerHealth>();
            rb = GetComponent<Rigidbody2D>();
            stageManager = Manager.MyGameManager.instance.currentStage;
            if (playerName == "")
                playerName = gameObject.name;
            stageManager.onGlodPlayerChange += this.OnGodPlayerChange;
        }
        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            Manager.MyGameManager.CurrentStageManager().onGameStart += () => onEneryChange?.Invoke(energy);
        }
        void OnGodPlayerChange(GameObject godPlayer)
        {
            if (godPlayer.Equals(this.gameObject))
            {
                this.frameInput._input.Player.Disable();
                this.frameInput._input.God.Enable();
                this.playerController.enabled = false;
                this.godCntroller.enabled = true;
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
                this.godCntroller.enabled = false;
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
        // Update is called once per frame
        void Update()
        {
        }
    }
}