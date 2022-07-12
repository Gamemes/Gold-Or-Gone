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
        void Start()
        {
            follow = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            this.transform.position = follow.transform.position + offset;
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
