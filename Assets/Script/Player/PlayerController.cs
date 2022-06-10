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
    /// <summary>
    /// 最基本的玩家操作, 包括移动, 跳跃(二段跳)
    /// </summary>
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
        [Header("跳跃参数")]
        public float fallMultiplier = 4f;

        public float lowJumpMultiplier = 10f;
        [Range(1, 100)]
        public float jumpSpeed = 24f;
        public float maxFallSpeed = 25f;
        public int maxJumpTime = 2;
        #endregion
        #region 行走参数
        [Header("行走参数")]
        public float walkSpeed = 11f;
        #endregion
        #region 碰撞
        [Header("碰撞")]
        public Bounds playerSize;
        public LayerMask groundLayer;
        RayRange _coldown, _colLef, _colRig, _colUp;
        public float detectionLength = 0.5f;
        public int detectionNums = 10;
        #endregion

        #region 冲刺
        [Header("冲刺")]
        public bool activeSprint = true;
        [Tooltip("玩家获得的速度加强")]
        public float sprintVelocityGain = 20f;
        public float sprintDurition = 0.2f;
        #endregion
        #region 爬墙
        [Header("爬墙")]
        public bool activeClimb = true;
        public float maxClimbTime = 4f;
        float climbTime = 0f;
        public float climbMoveSpeed = 4f;
        public float climbJumpSpeed = 10f;
        public float climbJumpDurition = 0.3f;
        public float climbJumpWalkInfluence = 1f;
        #endregion
        Rigidbody2D rb;
        Vector2 speedThisFrame = new Vector2();
        Vector2 speedPreFrame = new Vector2();
        int jumpTime = 0;
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerInput = GetComponent<FrameInput>();
        }
        private void FixedUpdate()
        {
            rb.velocity = Quaternion.Euler(0, 0, Manager.MyGameManager.instance.stageManager.gravityAngle) * speedThisFrame;
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
            CalculateClimb();
            CalculateSprint();
            //rb.velocity = speedThisFrame;
            //MovePlayer();
            rb.velocity = Quaternion.Euler(0, 0, Manager.MyGameManager.instance.stageManager.gravityAngle) * speedThisFrame;
            //Debug.Log($"{rb.velocity}");
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
            else if (speedThisFrame.y > 0 && !playerInput.HoldJump)
            {
                speedThisFrame += Vector2.up * -14 * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
            // 下落最大速度
            speedThisFrame.y = Mathf.Max(speedThisFrame.y, -maxFallSpeed);
        }

        void CalculateClimb()
        {
            if (!activeClimb)
                return;
            if ((colLef || colRig) && playerInput.Climb)
            {
                climbTime += Time.deltaTime;
                if (climbTime > maxClimbTime)
                {

                }
                else
                {
                    if (JumpingThisFrame)
                    {
                        if (colLef)
                        {
                            StartCoroutine(_ClimbJump(climbJumpSpeed));
                        }
                        else
                        {
                            StartCoroutine(_ClimbJump(-climbJumpSpeed));
                        }
                    }
                    else
                    {
                        speedThisFrame.y = Mathf.Max(playerInput.Vertical * climbMoveSpeed, speedThisFrame.y);
                        speedThisFrame.x = .0f;
                    }
                }
                Climbing = true;
            }
            else
            {
                Climbing = false;
            }
            if (colDow)
                climbTime = 0f;
        }
        IEnumerator _ClimbJump(float speed)
        {
            float t = 0f;
            speedThisFrame.y = speedThisFrame.y + Mathf.Abs(speed) * 0.4f;
            float s;
            while (t < climbJumpDurition)
            {
                if ((colLef || colRig) && t > 0.1f)
                    break;
                t += Time.deltaTime;

                s = (climbJumpDurition - t) / climbJumpDurition * speed;
                if (speedThisFrame.x * speed < 0)
                    s += (t / climbJumpDurition + climbJumpWalkInfluence) * speedThisFrame.x;
                else
                    s += (t / climbJumpDurition) * 0.6f * speedThisFrame.x;
                speedThisFrame.x = s;
                yield return null;
            }
        }
        void CalculateSprint()
        {
            if (!activeSprint)
                return;
            if (playerInput.Sprint)
            {
                StartCoroutine(_Sprint(sprintVelocityGain * speedThisFrame.x));
                //Debug.Log($"{speedThisFrame}");
            }
        }
        IEnumerator _Sprint(float speed)
        {
            float t = 0;
            //yield return null;
            float p = rb.gravityScale;
            rb.gravityScale = 0;
            while (t < sprintDurition)
            {
                speedThisFrame.y = 0;
                speedThisFrame.x = speed;
                t += Time.deltaTime;
                yield return null;
            }
            rb.gravityScale = p;
        }
        private void drawRayRang(RayRange rayRange)
        {
            Gizmos.color = Color.red;
            foreach (var pos in lerpPoint(rayRange, detectionNums))
            {
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
