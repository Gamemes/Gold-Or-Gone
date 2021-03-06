using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

namespace Player
{
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
    public interface IGodController : IController
    {
        public bool RotatingThisFrame { get; }
    }
}