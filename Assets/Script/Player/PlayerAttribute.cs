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
                    Manager.MyGameManager.instance.stageManager.changeGloadPlayer(gameObject);
                _energy = value % 3;
            }
        }
        private int _energy = 0;
        void Awake()
        {
            frameInput = GetComponent<FrameInput>();
            playerController = GetComponent<PlayerController>();
            godCntroller = GetComponent<GodController>();
            playerHealth = GetComponent<PlayerHealth>();
            rb = GetComponent<Rigidbody2D>();
            if (playerName == "")
                playerName = gameObject.name;
            Manager.MyGameManager.instance.stageManager.onGlodPlayerChange += this.onGodPlayerChange;
        }
        void onGodPlayerChange(GameObject godPlayer)
        {
            if (godPlayer.Equals(this.gameObject))
            {
                this.frameInput._input.Player.Disable();
                this.frameInput._input.God.Enable();
                this.playerController.enabled = false;
                this.godCntroller.enabled = true;
                this._energy = 0;
                rb.bodyType = RigidbodyType2D.Static;
                Debug.Log($"{this.gameObject} {godPlayer} {this.energy}");
            }
            else
            {
                this.frameInput._input.Player.Enable();
                this.frameInput._input.God.Disable();
                this.playerController.enabled = true;
                this.godCntroller.enabled = false;
                rb.bodyType = RigidbodyType2D.Kinematic;
                Manager.MyGameManager.instance.stageManager.stageCamera.Follow = transform;
            }
        }
        // Update is called once per frame
        void Update()
        {
        }
    }
}