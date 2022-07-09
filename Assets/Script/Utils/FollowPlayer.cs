using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class FollowPlayer : MonoBehaviour
    {
        public Vector3 offset = new(0, 0, 0);
        public Transform follow;
        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            if (follow)
                this.transform.position = follow.position + offset;
        }
        /// <summary>
        /// Callback to draw gizmos that are pickable and always drawn.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (follow)
                this.transform.position = follow.position + offset;
        }
    }
}
