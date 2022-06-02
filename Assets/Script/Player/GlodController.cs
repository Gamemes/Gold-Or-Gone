using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class GlodController : MonoBehaviour, IGoldController
    {
        public FrameInput playerInput { get; private set; }
        public bool RotatingThisFrame { get; private set; } = false;
        public Transform player;
        public float rotateSpeed = 10f;
        private void Awake()
        {
            playerInput = new FrameInput();
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
        }
        IEnumerator rotate(float angle = 90f)
        {
            float _ang = 0f;
            while (_ang < angle)
            {
                RotatingThisFrame = true;
                transform.RotateAround(player.position, Vector3.forward, rotateSpeed * Time.deltaTime);
                _ang += rotateSpeed * Time.deltaTime;
                yield return null;
            }
            transform.RotateAround(player.position, Vector3.forward, angle - _ang);
            RotatingThisFrame = false;
        }
    }

}
