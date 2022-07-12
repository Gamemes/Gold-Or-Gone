using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        // Start is called before the first frame update
        public enum PlayerState
        {
            Idle, Run, Death, FlyUp, FlyDown, Climb, Sprint
        }
        public Animator animator;
        private PlayerAttribute playerAttribute;
        private PlayerState currentState = PlayerState.Idle;
        void Start()
        {
            animator = GetComponent<Animator>();
            playerAttribute = GetComponent<PlayerAttribute>();
        }
        public void changeState(PlayerState state)
        {
            if (currentState == state) return;
            currentState = state;
            //Debug.Log($"change state to {state.ToString()}");
            animator.Play(state.ToString());
        }
        // Update is called once per frame
        void Update()
        {
            var velocity = playerAttribute.playerController.Velocity;
            if (playerAttribute.playerController.Climbing)
            {
                changeState(PlayerState.Climb);
            }
            else if (velocity.y > .3f)
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
