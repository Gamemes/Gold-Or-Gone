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
        private float adjustSpeed = 0f;
        private PlayerInput.GodActions godinput;
        private Manager.StageManager stageManager;
        [Tooltip("重力改变的持续时间")]
        public float gravityChangeDuration = 2f;
        [Tooltip("重力大小改变冷却")]
        public float gravityChangeCoolTime = 5f;
        [Tooltip("重力旋转的冷却")]
        public float gravityRotateCoolTime = 5f;
        public float lastGrivateChangeTime = 0f;
        public float lastGravityRotateTime = 0f;
        private void Start()
        {
            playerInput = GetComponent<FrameInput>();
            godinput = playerInput._input.God;
            stageManager = Manager.MyGameManager.instance.currentStage;
        }
        // Update is called once per frame
        void Update()
        {
            //playerInput.update();
            if (playerInput.Rotate && lastGravityRotateTime > gravityRotateCoolTime)
            {
                int dir = (int)godinput.RotateDir.ReadValue<float>();
                Manager.MyGameManager.instance.currentStage.RotateGravityDuration(dir * 90f, (float)90 / rotateSpeed);
                lastGravityRotateTime = 0f;
            }
            if (godinput.GrivateUp.WasPressedThisFrame() && lastGrivateChangeTime > gravityChangeCoolTime)
            {
                stageManager.AddGrivate(stageManager.gravitySize / 2, gravityChangeDuration);
                lastGrivateChangeTime = 0f;
            }
            else if (godinput.GrivateDown.WasPressedThisFrame() && lastGrivateChangeTime > gravityChangeCoolTime)
            {
                stageManager.AddGrivate(-stageManager.gravitySize / 2, gravityChangeDuration);
                lastGrivateChangeTime = 0f;
            }
            lastGravityRotateTime += Time.deltaTime;
            lastGrivateChangeTime += Time.deltaTime;

        }
    }

}
