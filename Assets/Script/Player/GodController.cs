using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class GodController : MonoBehaviour, IGodController
    {
        public FrameInput playerInput { get; private set; }
        public bool RotatingThisFrame { get; private set; } = false;
        public float rotateSpeed = 10f;
        private PlayerInput.GodActions godinput;
        private Manager.StageManager stageManager;
        [Tooltip("重力改变的持续时间")]
        public float gravityChangeDuration = 2f;
        [Tooltip("重力大小改变冷却")]
        public float gravityChangeCoolTime = 5f;
        [Tooltip("重力旋转的冷却")]
        public float gravityRotateCoolTime = 5f;
        public float fromLastGrivateChangeTime = 100f;
        public float fromLastGravityRotateTime = 100f;
        public bool activeFlip = false;
        private PlayerAttribute playerAttribute;
        private void Awake()
        {
            playerInput = GetComponent<FrameInput>();
            playerAttribute = GetComponent<PlayerAttribute>();
            godinput = playerInput._input.God;
            stageManager = Manager.MyGameManager.instance.currentStage;
        }
        void Update()
        {
            if (playerInput.Rotate && fromLastGravityRotateTime > gravityRotateCoolTime)
            {
                int dir = (int)godinput.RotateDir.ReadValue<float>();
                Manager.MyGameManager.instance.currentStage.RotateGravityDuration(dir * 90f, (float)90 / rotateSpeed);
                fromLastGravityRotateTime = 0f;
            }
            if (activeFlip && godinput.Flip.WasPerformedThisFrame() && fromLastGravityRotateTime > gravityRotateCoolTime)
            {
                int dir = UnityEngine.Random.Range(1, 10) % 2 == 0 ? 1 : -1;
                Manager.MyGameManager.instance.currentStage.RotateGravityDuration(dir * 180f, (float)90 / rotateSpeed);
                fromLastGravityRotateTime = 0f;
            }
            if (godinput.GrivateUp.WasPressedThisFrame() && fromLastGrivateChangeTime > gravityChangeCoolTime)
            {
                stageManager.AddGrivate(stageManager.gravitySize / 2, gravityChangeDuration);
                fromLastGrivateChangeTime = 0f;
            }
            else if (godinput.GrivateDown.WasPressedThisFrame() && fromLastGrivateChangeTime > gravityChangeCoolTime)
            {
                stageManager.AddGrivate(-stageManager.gravitySize / 2, gravityChangeDuration);
                fromLastGrivateChangeTime = 0f;
            }
            fromLastGravityRotateTime += Time.deltaTime;
            fromLastGrivateChangeTime += Time.deltaTime;

        }
        public void OnEnable()
        {
            playerInput._input.God.Enable();
            playerAttribute.playerHealth.energy = 0;
            playerAttribute.rb.bodyType = RigidbodyType2D.Static;
            playerAttribute.playerAnimation.changeState(PlayerAnimationController.PlayerState.Idle);
        }
        public void OnDisable()
        {
        }
    }

}
