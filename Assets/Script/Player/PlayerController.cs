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


        #region 爬墙
        [Header("爬墙")]
        public bool activeClimb = true;
        public float maxClimbTime = 4f;
        float climbTime = 0f;
        public float climbMoveSpeed = 4f;
        public float climbJumpSpeed = 10f;
        public float climbJumpHorSpeed = 20f;
        public float climbJumpDurition = 0.3f;
        public float climbJumpWalkInfluence = 1f;
        #endregion

        Rigidbody2D rb;
        Vector2 speedThisFrame = new Vector2();
        Vector2 speedPreFrame = new Vector2();
        bool _activate = false;
        void _active() => _activate = true;
        int jumpTime = 0;
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerInput = GetComponent<FrameInput>();
            Invoke(nameof(_active), 1f);
        }
        private void FixedUpdate()
        {
            //rb.velocity = Quaternion.Euler(0, 0, Manager.MyGameManager.instance.stageManager.gravityAngle) * speedThisFrame;
        }
        void Update()
        {
            if (!_activate)
                return;
            speedPreFrame = speedThisFrame;//Quaternion.Euler(0, 0, -Manager.MyGameManager.instance.stageManager.gravityAngle) * rb.velocity;
            //Debug.Log(speedPreFrame);
            //speedThisFrame.Set(0, 0);
            playerInput.update();
            collisionCheck();
            updateState();
            CalculateGravity();
            CalculateMove();
            CalculateJump();
            CalculateClimb();
            CalculateSprint();
            //rb.velocity = Quaternion.Euler(0, 0, Manager.MyGameManager.instance.stageManager.gravityAngle) * speedThisFrame;
            Velocity = speedThisFrame;
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
        void CalculateMove()
        {
            if (playerInput.Horizontal != 0f)
            {
                if (!sprintThisFrame)
                    speedThisFrame.x = playerInput.Horizontal * walkSpeed;
            }
            else
            {
                speedThisFrame.x = Mathf.MoveTowards(speedThisFrame.x, 0, 120 * Time.deltaTime);
            }
            if (speedThisFrame.x * transform.localScale.x < 0)
            {
                Vector3 tscale = transform.localScale;
                transform.localScale = new Vector3(-1 * tscale.x, tscale.y, tscale.z);
            }
        }
        #region 跳跃参数
        [Header("跳跃参数")]
        public float fallMultiplier = 4f;

        public float lowJumpMultiplier = 10f;
        [Range(1, 100)]
        public float jumpSpeed = 24f;
        public float maxFallSpeed = 25f;
        public int maxJumpTime = 2;
        public float jumpHoverTime = 0.1f;
        private bool isHover = false;
        #endregion
        void CalculateJump()
        {
            //speedThisFrame.y = speedPreFrame.y;
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
            if (!colDow && speedThisFrame.y >= .0f && speedThisFrame.y <= 1f && !isHover)
            {
                //Debug.Log($"hover");
                //StartCoroutine(JumpHover(jumpHoverTime));
            }
            // 下落最大速度
            speedThisFrame.y = Mathf.Max(speedThisFrame.y, -maxFallSpeed);
        }
        IEnumerator JumpHover(float time)
        {
            if (isHover)
                yield break;

            float t = 0f;
            isHover = true;
            while (t < time)
            {
                if (playerInput.Jump)
                    break;
                speedThisFrame.y = Mathf.MoveTowards(speedThisFrame.y, 0, 10 * Time.deltaTime);
                speedThisFrame.x *= 0.2f;
                t += Time.deltaTime;
                yield return 0;
            }
            isHover = false;
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
                            StartCoroutine(_ClimbJump(climbJumpHorSpeed));
                        }
                        else
                        {
                            StartCoroutine(_ClimbJump(-climbJumpHorSpeed));
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
            speedThisFrame.y = climbJumpSpeed;
            float s;
            while (t < climbJumpDurition)
            {
                if ((colLef || colRig) && t > 0.1f)
                    break;
                t += Time.deltaTime;
                s = speed * (1 - (t / climbJumpDurition));
                if (speedThisFrame.x * speed < 0)
                    s += (t / climbJumpDurition + climbJumpWalkInfluence) * speedThisFrame.x;
                else
                    s += (t / climbJumpDurition) * 0.6f * speedThisFrame.x;
                speedThisFrame.x = s;
                yield return null;
            }
            //speedThisFrame.y = 0;
            //speedThisFrame.x = 0;
        }
        #region 冲刺
        [Header("冲刺")]
        public bool activeSprint = true;
        [Tooltip("玩家获得的速度加强")]
        public float sprintVelocityGain = 20f;
        public float sprintDurition = 0.2f;
        public bool sprintThisFrame = false;
        private bool canSprint = true;
        #endregion
        void CalculateSprint()
        {
            if (!activeSprint)
                return;
            if (playerInput.Sprint)
            {
                StartCoroutine(_Sprint(new Vector2(playerInput.Horizontal, playerInput.Vertical) * sprintVelocityGain * walkSpeed));
                //Debug.Log($"{speedThisFrame}");
            }
        }
        IEnumerator _Sprint(Vector2 speed)
        {
            Debug.Log($"Sprint :{speed}");
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
        #region 重力
        [Header("重力")]
        public bool activeGrivate = true;
        public float gravityScale = 1.0f;
        public int _freeColliderIterations = 10;
        #endregion
        void CalculateGravity()
        {
            if (!activeGrivate)
                return;
            if (colDow)
            {
                speedThisFrame.y = 0;
            }
            else
            {
                if (!sprintThisFrame && !isHover)
                    speedThisFrame.y -= Manager.MyGameManager.instance.stageManager.gravity.sqrMagnitude * gravityScale * Time.deltaTime;
            }
        }
        void MovePlayer()
        {
            if (Time.deltaTime > 0.1f)
                return;
            var pos = transform.position;
            if (colLef && speedThisFrame.x < 0)
                speedThisFrame.x = 0;
            if (colRig && speedPreFrame.x > 0)
                speedThisFrame.x = 0;
            if (colUp && speedThisFrame.y > 0)
                speedThisFrame.y = 0;
            if (colDow && speedThisFrame.y < 0)
                speedThisFrame.y = 0;
            var rawMovement = Quaternion.Euler(0, 0, Manager.MyGameManager.instance.stageManager.gravityAngle) * speedThisFrame;
            var move = new Vector3(rawMovement.x, rawMovement.y) * Time.deltaTime;
            var furPos = pos + move;
            //var hit = Physics2D.Linecast(pos, furPos, groundLayer);
            var hit = Physics2D.OverlapBox(furPos, playerSize.size, Manager.MyGameManager.instance.stageManager.gravityAngle, groundLayer);
            if (!hit)
            {
                transform.position += move;
                return;
            }
            var moveto = transform.position;
            for (int i = 1; i <= _freeColliderIterations; ++i)
            {
                var t = (float)i / _freeColliderIterations;
                var postry = Vector2.Lerp(pos, furPos, t);
                if (Physics2D.OverlapBox(postry, playerSize.size, Manager.MyGameManager.instance.stageManager.gravityAngle, groundLayer))
                {
                    transform.position = moveto;
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
