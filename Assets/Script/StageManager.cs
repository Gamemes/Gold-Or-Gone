using System;
using System.Collections;
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
                //Debug.Log(value);
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
        /// <summary>
        /// 旋转重力, 直接旋转, 没有过程
        /// </summary>
        /// <param name="angle">度数</param>
        private void rotate_Gravity(float angle)
        {
            gravity = Quaternion.Euler(0, 0, angle) * gravity;
            onGravityRotated?.Invoke(angle);
            gravityAngle += angle;
        }
        IEnumerator _rotateGravityDuration(float angle, float duration)
        {
            float _ang = 0;
            float dangle, t = 0f, x;
            while (t < duration)
            {
                t += Time.deltaTime;
                x = (float)t / duration;
                dangle = Mathf.Pow(x, 3) * 4 * Time.deltaTime * angle;
                dangle = Mathf.Min(dangle, angle - _ang);
                rotate_Gravity(dangle);
                _ang += dangle;
                Debug.Log($"{t} {x} {dangle} {_ang}");
                yield return null;
            }
            yield return null;
        }
        public void rotateGravityDuration(float angle, float duration = 1f)
        {
            StartCoroutine(_rotateGravityDuration(angle, duration));
        }
    }
}