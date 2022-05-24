using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        public Vector2 Velocity { get; private set; }
        public FrameInput playerInput { get; private set; }
        public bool JumpingThisFrame { get; private set; }
        public bool LandingThisFrame { get; private set; }
        public bool Grounded { get; private set; }
        public float fallMultiplier = 4f;
        public float lowJumpMultiplier = 10f;
        [Range(1, 100)]
        public float jumpSpeed = 24f;
        public float walkSpeed = 11f;
        public float maxFallSpeed = 25f;
        public int maxJumpTime = 2;
        Rigidbody2D rb;
        Vector2 speedThisFrame = new Vector2();
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerInput = new FrameInput();
        }

        // Update is called once per frame
        void Update()
        {
            speedThisFrame.Set(0, 0);
            playerInput.update();
            move();
            jump();
            rb.velocity = speedThisFrame;
            Velocity = speedThisFrame;
        }
        void move()
        {
            speedThisFrame.x = playerInput.Horizontal * walkSpeed;
        }
        void jump()
        {
            speedThisFrame.y = rb.velocity.y;
            if (playerInput.Jump)
            {
                JumpingThisFrame = true;
                speedThisFrame.y = jumpSpeed;
            }
            // 下面两个增加手感
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && !playerInput.Jump)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
            // 下落最大速度
            speedThisFrame.y = Mathf.Max(speedThisFrame.y, -maxFallSpeed);
        }
    }

}
