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
        public bool colDow { get; private set; }
        public bool colUp { get; private set; }
        public bool colRig { get; private set; }
        public bool colLef { get; private set; }
        public bool Climbing { get; private set; }
        public float fallMultiplier = 4f;
        public float lowJumpMultiplier = 10f;
        [Range(1, 100)]
        public float jumpSpeed = 24f;
        public float walkSpeed = 11f;
        public float maxFallSpeed = 25f;
        public int maxJumpTime = 2;
        public LayerMask groundLayer;
        public Bounds playerSize;
        Rigidbody2D rb;
        Vector2 speedThisFrame = new Vector2();
        int jumpTime = 0;
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerInput = new FrameInput();
        }
        private void FixedUpdate()
        {

        }
        void Update()
        {
            speedThisFrame.Set(0, 0);
            playerInput.update();
            updateState();
            move();
            jump();
            climb();
            //rb.velocity = speedThisFrame;

            rb.velocity = speedThisFrame;
            Velocity = speedThisFrame;
        }
        void updateState()
        {
            LandingThisFrame = false;
            JumpingThisFrame = false;
            bool t = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - playerSize.size.y / 2), Vector2.down, 0.05f, groundLayer).collider != null;
            if (t != Grounded)
            {
                if (t)
                {
                    LandingThisFrame = true;
                    jumpTime = 0;
                }
                Grounded = t;
                colDow = Grounded;
            }
            colUp = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + playerSize.size.y / 2), Vector2.up, 0.05f, groundLayer).collider != null;
            colLef = Physics2D.Raycast(new Vector2(transform.position.x - playerSize.size.x / 2, transform.position.y), Vector2.left, 0.15f, groundLayer).collider != null;
            colRig = Physics2D.Raycast(new Vector2(transform.position.x + playerSize.size.x / 2, transform.position.y), Vector2.right, 0.15f, groundLayer).collider != null;
        }
        void move()
        {
            speedThisFrame.x = playerInput.Horizontal * walkSpeed;
        }
        void jump()
        {
            speedThisFrame.y = rb.velocity.y;
            if (playerInput.Jump && jumpTime < maxJumpTime)
            {
                JumpingThisFrame = true;
                speedThisFrame.y = jumpSpeed;
                ++jumpTime;
            }

            // 下面两个增加手感
            if (speedThisFrame.y < 0)
            {
                speedThisFrame += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (speedThisFrame.y > 0 && !Input.GetButton("Jump"))
            {
                speedThisFrame += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
            // 下落最大速度
            speedThisFrame.y = Mathf.Max(speedThisFrame.y, -maxFallSpeed);
        }

        void climb()
        {
            if ((colLef || colRig) && Input.GetKey(KeyCode.LeftShift))
            {
                // todo: 跳跃过程中应该减小移动
                if (JumpingThisFrame)
                {
                    if (colLef)
                    {
                        speedThisFrame.x = walkSpeed * 10;
                    }
                    else
                    {
                        speedThisFrame.x = -walkSpeed * 10;
                    }
                }
                else
                {
                    speedThisFrame.y = Mathf.Max(1, speedThisFrame.y);
                    speedThisFrame.x = .0f;
                }
                Climbing = true;
            }
            else
            {
                Climbing = false;
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, playerSize.size);
        }
    }

}
