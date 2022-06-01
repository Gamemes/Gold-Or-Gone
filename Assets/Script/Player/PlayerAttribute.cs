using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAttribute : MonoBehaviour
    {
        // Start is called before the first frame update
        public IController controller { get; private set; }
        public PlayerHealth playerHealth { get; private set; }
        void Start()
        {
            controller = GetComponent<IController>();
            playerHealth = GetComponent<PlayerHealth>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}