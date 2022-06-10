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
        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Awake()
        {
            _input = new PlayerInput();
            _input.Player.Enable();
        }
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            if (_input != null)
                _input.Player.Enable();
        }
        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            _input.Player.Disable();
        }
        public void setDevice(InputDevice inputDevice)
        {
            _input.devices = new InputDevice[] { inputDevice };

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
            Sprint = _input.Player.Sprint.WasPressedThisFrame();
        }
        public override string ToString()
        {
            return $"Horizontal: {Horizontal} Jump: {Jump}";
        }
    }
}
