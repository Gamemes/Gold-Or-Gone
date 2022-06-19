using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        // Start is called before the first frame update
        enum PlayerState
        {
            Idle, Run, Death, FlyUp, FlyDown
        }
        public Animator animator;
        private PlayerAttribute playerAttribute;
        private PlayerState currentState = PlayerState.Idle;
        void Start()
        {
            animator = GetComponent<Animator>();
            playerAttribute = GetComponent<PlayerAttribute>();
        }
        void changeState(PlayerState state)
        {
            if (currentState == state) return;
            Debug.Log($"change to {state.ToString()}");
            currentState = state;
            animator.Play(state.ToString());
        }
        // Update is called once per frame
        void Update()
        {
            var velocity = playerAttribute.playerController.Velocity;
            if (velocity.y > .3f)
            {
                changeState(PlayerState.FlyUp);
            }
            else if (velocity.y < -.3f)
            {
                changeState(PlayerState.FlyDown);
            }
            else if (velocity.x == .0f && velocity.y == .0f)
            {
                changeState(PlayerState.Idle);
            }
            else if (velocity.x != .0f)
            {
                changeState(PlayerState.Run);
            }
        }
    }
}
