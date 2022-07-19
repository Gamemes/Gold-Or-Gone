using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{

    public class FollowCamera : MonoBehaviour
    {
        // Start is called before the first frame update
        private Camera follow;
        public Vector3 offset = new(0, 0, 10);
        public float diff = 1;
        private Vector3 cpos;
        void Start()
        {
            follow = Camera.main;
            this.transform.position = follow.transform.position + offset;
            cpos = follow.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            var step = follow.transform.position - cpos;
            this.transform.position += step * diff;
            cpos = follow.transform.position;
        }
        /// <summary>
        /// Callback to draw gizmos that are pickable and always drawn.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!follow)
                follow = Camera.main;
            this.transform.position = follow.transform.position + offset;
        }
    }
}
