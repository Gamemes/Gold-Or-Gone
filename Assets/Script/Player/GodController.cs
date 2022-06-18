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
        private void Start()
        {
            playerInput = GetComponent<FrameInput>();
            godinput = playerInput._input.God;
            stageManager = Manager.MyGameManager.instance.stageManager;
        }
        // Update is called once per frame
        void Update()
        {
            playerInput.update();
            if (playerInput.Rotate)
            {
                Manager.MyGameManager.instance.stageManager.rotateGravityDuration(godinput.RotateDir.ReadValue<float>() * 90f, (float)90 / rotateSpeed);
                //StartCoroutine(rotate());
            }
            if (godinput.GrivateUp.IsPressed())
            {
                adjustSpeed += Time.deltaTime;
                stageManager.addGrivate(Time.deltaTime);
            }
            else if (godinput.GrivateDown.IsPressed())
            {
                adjustSpeed += Time.deltaTime;
                stageManager.addGrivate(-Time.deltaTime);
            }
            else
            {
                adjustSpeed = 0;
            }
            // if (playerInput.Horizontal != 0f)
            // {
            //     Manager.MyGameManager.instance.stageManager.rotate_Gravity(playerInput.Horizontal * Time.deltaTime * rotateAdjustSpeed);
            // }
        }
    }

}
