using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

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
            //_input.devices = new InputDevice[] { Keyboard.all[0] };
        }
        public void setDevice(InputDevice inputDevice)
        {
            _input.devices = new InputDevice[] { inputDevice };

        }
        ~FrameInput()
        {
            _input.Player.Disable();
        }
        public void update()
        {
            var vec = _input.Player.Move.ReadValue<Vector2>();
            Horizontal = vec.x;
            Vertical = vec.y;
            Jump = _input.Player.Jump.WasPressedThisFrame();
            HoldJump = _input.Player.Jump.IsPressed();
            Rotate = _input.Player.Rotate.WasPressedThisFrame();
            Climb = _input.Player.Climb.IsPressed();
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