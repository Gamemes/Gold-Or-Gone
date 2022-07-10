using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerUIManager : MonoBehaviour
    {
        public Vector3 offset = new(0, 0, 0);
        public PlayerAttribute targetPlayer;
        public Transform playerTransform;
        // Start is called before the first frame update
        void Start()
        {
            Debug.Assert(targetPlayer != null);
        }

        private void Update()
        {
            if (playerTransform)
            {
                this.transform.rotation = playerTransform.rotation;
                this.transform.position = playerTransform.position + offset;

            }
        }
        private void OnDrawGizmos()
        {
            if (playerTransform)
            {
                this.transform.rotation = playerTransform.rotation;
                this.transform.position = playerTransform.position + offset;
            }
        }
    }

}
