using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    /// <summary>
    /// 场景管理, 包括这个场景的一切, 比如玩家, 重力
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        // Start is called before the first frame update
        public Vector2 gravity
        {
            get
            {
                return Physics2D.gravity;
            }
            set
            {
                gravityDirection = value.normalized;
                Physics2D.gravity = value;
            }
        }
        private float _gravityAngle = 0f;
        public float gravityAngle
        {
            get
            {
                return _gravityAngle;
            }
            private set
            {
                _gravityAngle = value;

            }
        }
        public Vector2 gravityDirection { get; private set; }
        public Action<float> onGravityRotated;
        void Awake()
        {
            MyGameManager.instance.setStageManager(this);
            gravityDirection = gravity.normalized;
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator _rotateGravity(float angle, float rotateSpeed)
        {
            float _ang = 0;
            float dangle;
            while (_ang < angle)
            {
                dangle = rotateSpeed * Time.deltaTime;
                //dangle = dangle * Mathf.Pow((float)_ang / angle, 2) * 3;
                gravity = Quaternion.Euler(0, 0, dangle) * gravity;
                onGravityRotated?.Invoke(dangle);
                gravityAngle += dangle;
                _ang += dangle;
                yield return null;
            }
            dangle = angle - _ang;
            gravity = Quaternion.Euler(0, 0, dangle) * gravity;
            onGravityRotated?.Invoke(dangle);
            gravityAngle += dangle;
            yield return null;
        }

        public void rotateGravity(float angle, float rotateSpeed = 40)
        {
            StartCoroutine(_rotateGravity(angle, rotateSpeed));
        }
    }
}