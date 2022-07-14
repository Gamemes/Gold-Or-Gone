using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Player
{
    public class FrameInput : MonoBehaviour
    {
        public float Horizontal;
        public float Vertical;
        public bool Jump;
        public bool HoldJump;
        public bool Rotate;
        public bool Climb;
        public bool Sprint;
        public PlayerInput _input;
        private void Awake()
        {
            _input = new PlayerInput();
            _input.Player.Enable();
        }
        private void OnEnable()
        {
            if (_input != null)
                _input.Player.Enable();
        }
        private void OnDisable()
        {
            _input.Player.Disable();
            _input.God.Disable();
        }
        public void setDevice(InputDevice inputDevice)
        {
            _input.devices = new InputDevice[] { inputDevice };

        }
        public void Update()
        {
            if (_input.Player.enabled)
            {
                var vec = _input.Player.Move.ReadValue<Vector2>();
                Horizontal = vec.x;
                Vertical = vec.y;
                Jump = _input.Player.Jump.WasPressedThisFrame();
                HoldJump = _input.Player.Jump.IsPressed();
                Climb = _input.Player.Climb.IsPressed();
                Sprint = _input.Player.Sprint.WasPressedThisFrame();
            }
            else if (_input.God.enabled)
            {
                Rotate = _input.God.Rotate.WasPressedThisFrame();
            }
        }
    }
}
