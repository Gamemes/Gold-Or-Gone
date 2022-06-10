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
        void onGodPlayerChange(GameObject player)
        {
            Debug.Log($"{this.gameObject} {player}");
            if (player == this.gameObject)
            {
                this.playerController.enabled = false;
                this.godCntroller.enabled = true;
                rb.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                this.playerController.enabled = true;
                this.godCntroller.enabled = false;
                rb.bodyType = RigidbodyType2D.Dynamic;
                Manager.MyGameManager.instance.stageManager.stageCamera.Follow = transform;
            }
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}