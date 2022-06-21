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
            Manager.MyGameManager.instance.stageManager.onGravityRotated += this.rotate;
        }
        void rotate(float dangle)
        {
            angle += dangle;
        }
        void Update()
        {
            if (angle != 0f)
            {
                int dir = angle > 0 ? 1 : -1;
                float dangle = rotateSpeed * Time.deltaTime;
                dangle = dir * Mathf.Min(Mathf.Abs(dangle), Mathf.Abs(angle));
                if (!playerController.hitGround(transform.position, Quaternion.Euler(0, 0, transform.eulerAngles.z + dangle)))
                {
                    transform.Rotate(new Vector3(0, 0, dangle));
                    angle -= dangle;
                }
            }
        }
    }
}