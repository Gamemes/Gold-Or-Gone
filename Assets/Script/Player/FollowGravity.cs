using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player
{
    public class FollowGravity : MonoBehaviour
    {
        public float rotateSpeed = 180f;
        private PlayerController playerController;
        private float angle = 0f;
        void Start()
        {
            playerController = GetComponent<PlayerController>();
            Manager.MyGameManager.instance.currentStage.onGravityRotateCompleted += this.rotate;
        }
        void rotate(float dangle)
        {
            StartCoroutine(_startRotate(dangle));
            //angle += dangle;
        }
        void Update()
        {
            // if (angle != 0f && !playerController.hitGround(transform.position, Quaternion.Euler(0, 0, Manager.MyGameManager.instance.stageManager.gravityAngle)))
            // {
            //     StartCoroutine(_startRotate(angle));
            //     angle = 0f;
            // }
        }
        IEnumerator _startRotate(float _angle)
        {
            Debug.Log($"start rotate {_angle}");
            while (playerController.hitGround(transform.position, Quaternion.Euler(0, 0, Manager.MyGameManager.instance.currentStage.gravityAngle)))
            {
                yield return null;
            }
            int dir = _angle > 0 ? 1 : -1;
            while (_angle != 0)
            {
                float dangle = rotateSpeed * Time.deltaTime;
                dangle = dir * Mathf.Min(Mathf.Abs(dangle), Mathf.Abs(_angle));
                transform.Rotate(new Vector3(0, 0, dangle));
                _angle -= dangle;
                yield return null;
            }
        }
    }
}