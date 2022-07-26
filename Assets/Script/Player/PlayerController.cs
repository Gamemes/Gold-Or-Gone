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
        public int faceDir
        {
            get => transform.localScale.x > 0 ? 1 : -1;
        }
        #endregion


        #region 碰撞
        [Header("碰撞")]
        public Bounds playerSize;
        public LayerMask groundLayer;
        RayRange _coldown, _colLef, _colRig, _colUp;
        public float detectionLength = 0.5f;
        public int detectionNums = 10;
        #endregion
        Rigidbody2D rb;
        /// <summary>
        /// 玩家的速度, 以重力方向为y轴负方向的坐标系.
        /// </summary>
        public Vector2 speedThisFrame = new Vector2();
        public Vector2 speedPreFrame = new Vector2();
        /// <summary>
        /// 启用玩家本地控制, 如果来自远程则取消
        /// </summary>
        public bool _activate = true;
        void _active() => _activate = true;
        int jumpTime = 0;
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerInput = GetComponent<FrameInput>();
            //Invoke(nameof(_active), 0.5f);
        }
        private void FixedUpdate()
        {
            //rb.velocity = Quaternion.Euler(0, 0, Manager.MyGameManager.instance.stageManager.gravityAngle) * speedThisFrame;
        }
        void Update()
        {
            collisionCheck();
            updateState();
            if (!_activate)
                return;
            speedPreFrame = speedThisFrame;
            //playerInput.update();
            CalculateGravity();
            CalculateMove();
            CalculateJump();
            CalculateClimb();
            CalculateSprint();
        }
        /// <summary>
        /// LateUpdate is called every frame, if the Behaviour is enabled.
        /// It is called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
            if (!_activate)
                return;
            MovePlayer();
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
            _coldown = new RayRange(minpos, new Vector2(maxpos.x, minpos.y), Vector2.down, transform.position, angle);
            _colLef = new RayRange(minpos, new Vector2(minpos.x, maxpos.y), Vector2.left, transform.position, angle);
            _colRig = new RayRange(new Vector2(maxpos.x, minpos.y), maxpos, Vector2.right, transform.position, angle);
            _colUp = new RayRange(new Vector2(minpos.x, maxpos.y), maxpos, Vector2.up, transform.position, angle);
        }
        private IEnumerable<Vector2> lerpPoint(RayRange ran, int nums)
        {
            for (var i = 0; i <= nums; i++)
            {
                yield return Vector2.Lerp(ran.Start, ran.End, (float)i / nums);
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
        #region 行走参数
        [Header("行走参数")]
        public float walkSpeed = 11f;
        public float acceleration = 90f;
        public float deAcceleration = 60f;
        void CalculateMove()
        {
            if (playerInput.Horizontal != 0f)
            {
                // if (!sprintThisFrame)
                //     speedThisFrame.x = playerInput.Horizontal * walkSpeed;
                speedThisFrame.x += playerInput.Horizontal * Time.deltaTime * acceleration;
                speedThisFrame.x = Mathf.Clamp(speedThisFrame.x, -walkSpeed, walkSpeed);
            }
            else
            {
                speedThisFrame.x = Mathf.MoveTowards(speedThisFrame.x, 0, deAcceleration * Time.deltaTime);
            }
            if (speedThisFrame.x * transform.localScale.x < 0)
            {
                Vector3 tscale = transform.localScale;
                transform.localScale = new Vector3(-1 * tscale.x, tscale.y, tscale.z);
            }
        }
        #endregion
        #region 跳跃
        [Header("跳跃参数")]
        public float fallMultiplier = 4f;

        public float lowJumpMultiplier = 10f;
        [Range(1, 100)]
        public float jumpSpeed = 24f;
        public float maxFallSpeed = 25f;
        public float glidingFallSpeed = 5f;
        public int maxJumpTime = 2;
        // public float jumpHoverTime = 0.1f;
        // private bool isHover = false;
        [Tooltip("开启滑翔")]
        public bool activeGliding = false;

        void CalculateJump()
        {
            if (playerInput.Jump && jumpTime < maxJumpTime)
            {
                JumpingThisFrame = true;
                float k = 1f - ((float)jumpTime / (maxJumpTime * 2));
                speedThisFrame.y = jumpSpeed * k;
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
            float tmaxfallspeed = maxFallSpeed;
            if (playerInput.HoldJump && activeGliding)
                tmaxfallspeed = glidingFallSpeed;
            speedThisFrame.y = Mathf.Max(speedThisFrame.y, -tmaxfallspeed);
        }
        #endregion
        #region 爬墙
        [Header("爬墙")]
        public bool activeClimb = true;
        [Tooltip("在墙上停留的时间")]
        public float maxClimbTime = 4f;
        float climbTime = 0f;
        [Tooltip("在墙上上下移动的速度")]
        public float climbMoveSpeed = 4f;
        [Tooltip("在墙上跳跃的锤子速度")]
        public float climbJumpSpeed = 10f;
        [Tooltip("在墙上跳跃的水平速度")]
        public float climbJumpHorSpeed = 20f;
        [Tooltip("在墙上跳跃的持续时间")]
        public float climbJumpDurition = 0.3f;
        [Tooltip("水平移动对墙跳的横向影响")]
        public float climbJumpWalkInfluence = 1f;
        [Tooltip("墙跳次数")]
        public int maxClimbJumpTime = 3;
        private int climbJumpTime = 0;
        void CalculateClimb()
        {
            if (!activeClimb)
                return;
            if ((colLef || colRig) && playerInput.Climb)
            {
                climbTime += Time.deltaTime;
                if (climbTime < maxClimbTime)
                {
                    if (playerInput.Jump && climbJumpTime < maxClimbJumpTime)
                    {
                        climbJumpTime++;
                        JumpingThisFrame = true;
                        if (colLef)
                        {
                            StartCoroutine(_ClimbJump(climbJumpHorSpeed));
                        }
                        else
                        {
                            StartCoroutine(_ClimbJump(-climbJumpHorSpeed));
                        }
                    }
                    else
                    {
                        speedThisFrame.y = 0f;//Mathf.Max(playerInput.Vertical * climbMoveSpeed, speedThisFrame.y);
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
            {
                climbTime = 0f;
                climbJumpTime = 0;
            }
        }
        IEnumerator _ClimbJump(float speed)
        {
            float t = 0f;
            speedThisFrame.y = climbJumpSpeed;
            float s, k;
            while (t < climbJumpDurition)
            {
                if ((colLef || colRig) && t > 0.1f)
                    break;
                t += Time.deltaTime;
                k = (t / climbJumpDurition);
                s = speed * (1 - k);
                if (playerInput.Horizontal * speed < 0)
                    s += k * climbJumpWalkInfluence * walkSpeed * playerInput.Horizontal;
                else
                    s += k * climbJumpWalkInfluence * walkSpeed * playerInput.Horizontal;
                speedThisFrame.x = s;
                yield return null;
            }
        }
        #endregion
        #region 冲刺
        [Header("冲刺")]
        public bool activeSprint = true;
        [Tooltip("玩家获得的速度加强")]
        public float sprintVelocityGain = 20f;
        public float sprintDurition = 0.2f;
        public bool sprintThisFrame = false;
        private bool canSprint = true;
        public float sprintCoolTime = 3f;
        public float fromLastSprintTime { get; private set; } = 0f;
        void CalculateSprint()
        {
            if (!activeSprint)
                return;
            if (playerInput.Sprint && fromLastSprintTime > sprintCoolTime)
            {
                StartCoroutine(_Sprint(new Vector2(playerInput.Horizontal, playerInput.Vertical) * sprintVelocityGain * walkSpeed));
                fromLastSprintTime = 0f;
            }
            fromLastSprintTime += Time.deltaTime;
        }
        IEnumerator _Sprint(Vector2 speed)
        {
            if (sprintThisFrame || !canSprint)
                yield break;
            canSprint = false;
            sprintThisFrame = true;
            float t = 0;
            while (t < sprintDurition)
            {
                speedThisFrame.y = speed.y;
                speedThisFrame.x = speed.x;
                t += Time.deltaTime;
                yield return null;
            }
            speedThisFrame.y = 0f;
            sprintThisFrame = false;
            while (!colDow) { yield return null; }
            canSprint = true;
        }
        #endregion
        #region 重力
        [Header("重力")]
        public bool activeGrivate = true;
        public float gravityScale = 1.0f;
        public int _freeColliderIterations = 10;

        void CalculateGravity()
        {
            if (!activeGrivate)
                return;
            speedThisFrame.y -= Manager.MyGameManager.instance.currentStage.gravity.sqrMagnitude * gravityScale * Time.deltaTime;

        }
        bool BoundCast(Bounds a, Bounds b)
        {
            var hit = Physics2D.Linecast(a.min, b.min, groundLayer);
            if (hit)
                return true;
            hit = Physics2D.Linecast(a.max, b.max, groundLayer);
            if (hit)
                return true;
            hit = Physics2D.Linecast(new Vector2(a.min.x, a.max.y), new Vector2(b.min.x, b.max.y), groundLayer);
            if (hit)
                return true;
            hit = Physics2D.Linecast(new Vector2(a.max.x, a.min.y), new Vector2(b.max.x, b.min.y), groundLayer);
            if (hit)
                return true;
            return false;
        }
        #endregion
        /// <summary>
        /// 返回玩家在pos位置quaternion旋转的情况下是否会碰撞到墙壁
        /// </summary>
        /// <param name="pos">目标位置</param>
        /// <param name="quaternion">目标旋转</param>
        /// <returns></returns>
        public bool hitGround(Vector3 pos, Quaternion quaternion)
        {
            var offset = quaternion * playerSize.center;
            var fpos = pos + offset;
            var hit = Physics2D.OverlapBox(fpos, playerSize.size, quaternion.eulerAngles.z, groundLayer);
            return hit != null;
        }
        void MovePlayer()
        {
            if (Time.deltaTime > 0.1f)
                return;
            var offset = Quaternion.Euler(0, 0, transform.eulerAngles.z) * playerSize.center;
            var pos = transform.position + offset;
            if (colLef && speedThisFrame.x < 0)
                speedThisFrame.x = 0;
            if (colRig && speedThisFrame.x > 0)
                speedThisFrame.x = 0;
            if (colUp && speedThisFrame.y > 0)
                speedThisFrame.y = 0;
            if (colDow && speedThisFrame.y < 0)
                speedThisFrame.y = 0;
            var rawMovement = Quaternion.Euler(0, 0, transform.eulerAngles.z) * speedThisFrame;
            var move = new Vector3(rawMovement.x, rawMovement.y) * Time.deltaTime;
            var furPos = pos + move;
            var hit = Physics2D.OverlapBox(furPos, playerSize.size, transform.eulerAngles.z, groundLayer);
            //&& !BoundCast(new Bounds(pos, playerSize.size), new Bounds(furPos, playerSize.size))
            if (!hit)
            {
                transform.position += move;
                return;
            }
            var moveto = transform.position;
            for (int i = 0; i <= _freeColliderIterations; ++i)
            {
                var t = (float)i / _freeColliderIterations;
                var postry = Vector2.Lerp(pos, furPos, t);
                if (Physics2D.OverlapBox(postry, playerSize.size, transform.eulerAngles.z, groundLayer))
                {
                    transform.position = moveto - offset;
                    return;
                }
                moveto = postry;
            }

        }
        private void drawRayRang(RayRange rayRange)
        {
            Gizmos.color = Color.red;
            foreach (var pos in lerpPoint(rayRange, detectionNums))
            {
                Gizmos.DrawLine(pos, pos + rayRange.Dir * detectionLength);
            }
        }
        private void DrawWriteRect(Vector2 center, Vector2 size, Quaternion rotation)
        {
            var halfx = size.x / 2;
            var halfy = size.y / 2;
            var movedis = new Vector3(center.x, center.y, 0);
            //4个顶点坐标
            var leftDownPos = rotation * new Vector2(-halfx, -halfy) + movedis;
            var rightDownPos = rotation * new Vector2(halfx, -halfy) + movedis;
            var leftUpPos = rotation * new Vector2(-halfx, halfy) + movedis;
            var rightUpPos = rotation * new Vector2(halfx, halfy) + movedis;
            //4条线段
            Gizmos.DrawLine(leftDownPos, rightDownPos);
            Gizmos.DrawLine(leftDownPos, leftUpPos);
            Gizmos.DrawLine(leftUpPos, rightUpPos);
            Gizmos.DrawLine(rightUpPos, rightDownPos);
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            //Gizmos.DrawWireCube(transform.position + playerSize.center, playerSize.size);
            var offset = Quaternion.Euler(0, 0, transform.eulerAngles.z) * playerSize.center;
            DrawWriteRect(transform.position + offset, playerSize.size, Quaternion.Euler(0, 0, transform.eulerAngles.z));
            CalculateRayRanged();
            drawRayRang(_coldown);
            drawRayRang(_colLef);
            drawRayRang(_colRig);
            drawRayRang(_colUp);
        }
        public void ReSetController()
        {
            activeClimb = false;
            activeSprint = false;
            activeGliding = false;
            maxJumpTime = 1;
        }
        public void OnEnable()
        {
            playerInput._input.Player.Enable();
            _activate = true;
            rb.bodyType = RigidbodyType2D.Kinematic;
            Manager.StageManager.CurrentStageManager().stageCamera.Follow = this.transform;
        }
        public void OnDisable()
        {
            _activate = false;
        }
    }

}
