using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class FrameInput
    {
        public float Horizontal;
        public float Vertical;
        public bool Jump;
        public bool Rotate;
        public void update()
        {
            Horizontal = Input.GetAxis("Horizontal");
            Vertical = Input.GetAxis("Vertical");
            Jump = Input.GetButtonDown("Jump");
            Rotate = Input.GetKeyDown(KeyCode.Return);
        }
    }
    public interface IController
    {
        public FrameInput playerInput { get; }
    }
    public interface IPlayerController : IController
    {
        public Vector2 Velocity { get; }
        public bool JumpingThisFrame { get; }
        public bool LandingThisFrame { get; }
        public bool Grounded { get; }
    }
    public interface IGoldController : IController
    {
        public bool RotatingThisFrame { get; }
    }
}