using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Player
{
    public struct RayRange
    {
        public RayRange(float x1, float y1, float x2, float y2, Vector2 dir)
        {
            Start = new Vector2(x1, y1);
            End = new Vector2(x2, y2);
            Dir = dir;
        }
        public RayRange(Vector2 start, Vector2 end, Vector2 dir, Vector3 offset, float angle = 0f)
        {
            Start = Quaternion.Euler(0, 0, angle) * start + offset;
            End = Quaternion.Euler(0, 0, angle) * end + offset;
            Dir = Quaternion.Euler(0, 0, angle) * dir;

        }

        public readonly Vector2 Start, End, Dir;
    }
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        #region IPlayerController接口部分

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
        #endregion


        #region 跳跃参数
        public float fallMultiplier = 4f;

        public float lowJumpMultiplier = 10f;
        [Range(1, 100)]
        public float jumpSpeed = 24f;
        public float maxFallSpeed = 25f;
        public int maxJumpTime = 2;
        #endregion

        public float walkSpeed = 11f;
        public LayerMask groundLayer;
        public Bounds playerSize;
        Rigidbody2D rb;
        Vector2 speedThisFrame = new Vector2();
        Vector2 speedPreFrame = new Vector2();
        int jumpTime = 0;
        #region 碰撞
        RayRange _coldown, _colLef, _colRig, _colUp;
        public float detectionLength = 0.5f;
        public int detectionNums = 10;
        #endregion
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
            speedPreFrame = Quaternion.Euler(0, 0, -Manager.MyGameManager.instance.stageManager.gravityAngle) * rb.velocity;
            //Debug.Log(speedPreFrame);
            speedThisFrame.Set(0, 0);
            playerInput.update();
            collisionCheck();
            updateState();
            move();
            jump();
            climb();
            //rb.velocity = speedThisFrame;

            rb.velocity = Quaternion.Euler(0, 0, Manager.MyGameManager.instance.stageManager.gravityAngle) * speedThisFrame;
            Velocity = speedThisFrame;
        }
        void collisionCheck()
        {
            CalculateRayRanged();
            bool runDetection(RayRange rayRange)
            {
                return lerpPoint(rayRange, detectionNums).Any(point => Physics2D.Raycast(point, rayRange.Dir, detectionLength, groundLayer));
            }
            colDow = runDetection(_coldown);
            colLef = runDetection(_colLef);
            colRig = runDetection(_colRig);
            colUp = runDetection(_colUp);
        }
        void CalculateRayRanged()
        {
            var b = new Bounds(playerSize.center, playerSize.size);
            var minpos = b.min;
            var maxpos = b.max;
            float angle = transform.rotation.eulerAngles.z;
            // if (Manager.MyGameManager.instance && Manager.MyGameManager.instance.stageManager != null)
            //     angle = Manager.MyGameManager.instance.stageManager.gravityAngle;
            // else
            //     angle = 0;
            //Debug.Log(angle);
            _coldown = new RayRange(minpos, new Vector2(maxpos.x, minpos.y), Vector2.down, transform.position, angle);
            _colLef = new RayRange(minpos, new Vector2(minpos.x, maxpos.y), Vector2.left, transform.position, angle);
            _colRig = new RayRange(new Vector2(maxpos.x, minpos.y), maxpos, Vector2.right, transform.position, angle);
            _colUp = new RayRange(new Vector2(minpos.x, maxpos.y), maxpos, Vector2.up, transform.position, angle);
        }
        private IEnumerable<Vector2> lerpPoint(RayRange ran, int nums)
        {
            //Debug.Log($"{ran.Start} {ran.End} {ran.Dir}");
            for (var i = 1; i <= nums; i++)
            {
                yield return Vector2.Lerp(ran.Start, ran.End, (float)i / (nums + 1));
            }
        }
        void updateState()
        {
            LandingThisFrame = false;
            JumpingThisFrame = false;
            if (colDow != Grounded)
            {
                if (colDow)
                {
                    LandingThisFrame = true;
                    jumpTime = 0;
                }
                Grounded = colDow;
            }
        }

        void move()
        {
            speedThisFrame.x = playerInput.Horizontal * walkSpeed;
        }
        void jump()
        {
            speedThisFrame.y = speedPreFrame.y;
            if (playerInput.Jump && jumpTime < maxJumpTime)
            {
                JumpingThisFrame = true;
                speedThisFrame.y = jumpSpeed;
                ++jumpTime;
            }

            // 下面两个增加手感
            if (speedThisFrame.y < 0)
            {
                speedThisFrame += Vector2.up * -14 * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (speedThisFrame.y > 0 && !Input.GetButton("Jump"))
            {
                speedThisFrame += Vector2.up * -14 * (lowJumpMultiplier - 1) * Time.deltaTime;
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
        private void drawRayRang(RayRange rayRange)
        {
            Gizmos.color = Color.red;
            foreach (var pos in lerpPoint(rayRange, detectionNums))
            {
                //Debug.Log(pos);
                Gizmos.DrawLine(pos, pos + rayRange.Dir * detectionLength);
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, playerSize.size);
            CalculateRayRanged();
            drawRayRang(_coldown);
            drawRayRang(_colLef);
            drawRayRang(_colRig);
            drawRayRang(_colUp);
        }
    }

}
