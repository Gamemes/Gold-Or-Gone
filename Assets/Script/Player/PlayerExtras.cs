using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class FrameInput
    {
        public float Horizontal;
        public float Vertical;
        public bool Jump;
        public bool HoldJump;
        public bool Rotate;
        public bool Climb;
        public PlayerInput _input;
        public FrameInput()
        {
            _input = new PlayerInput();
            _input.Player.Enable();
        }
        public void update()
        {
            Horizontal = _input.Player.Move.ReadValue<float>();
            //Vertical = Input.GetAxis("Vertical");
            Jump = _input.Player.Jump.WasPressedThisFrame();
            HoldJump = _input.Player.Jump.IsPressed();
            Rotate = false;//Input.GetKeyDown(KeyCode.Return);
            Climb = _input.Player.Climb.IsPressed();
            //Debug.Log(this.ToString());
        }
        public override string ToString()
        {
            return $"Horizontal: {Horizontal} Jump: {Jump}";
        }
    }
    public interface IController
    {
        public FrameInput playerInput { get; }
    }
    public interface IPlayerController : IController
    {
        public Vector2 Velocity { get; }
        /// <summary>
        /// 这一帧是否跳跃
        /// </summary>
        public bool JumpingThisFrame { get; }
        /// <summary>
        /// 这一帧是否着陆
        /// </summary>
        public bool LandingThisFrame { get; }
        /// <summary>
        /// 是否在地面上
        /// </summary>
        public bool Grounded { get; }
        public bool colDow { get; }
        public bool colUp { get; }
        public bool colRig { get; }
        public bool colLef { get; }
    }
    public interface IGoldController : IController
    {
        public bool RotatingThisFrame { get; }
    }
}