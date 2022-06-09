using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class GlodController : MonoBehaviour, IGoldController
    {
        public FrameInput playerInput { get; private set; }
        public bool RotatingThisFrame { get; private set; } = false;
        public float rotateSpeed = 10f;
        private void Awake()
        {
            playerInput = GetComponent<FrameInput>();
        }
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            Manager.MyGameManager.instance.stageManager.changeGloadPlayer(this.gameObject);
        }
        // Update is called once per frame
        void Update()
        {
            playerInput.update();
            if (playerInput.Rotate)
            {
                Manager.MyGameManager.instance.stageManager.rotateGravityDuration(90, (float)90 / rotateSpeed);
                //StartCoroutine(rotate());
            }
            if (playerInput.Horizontal != 0f)
            {
                Manager.MyGameManager.instance.stageManager.rotate_Gravity(playerInput.Horizontal * Time.deltaTime);
            }
        }
    }

}
