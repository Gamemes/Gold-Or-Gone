using System;
using Player;
using UnityEngine;

namespace Trap
{
    /// <summary>
    /// 陷阱基类
    /// </summary>
    public class TrapBase : Prop.PropBase
    {
        public virtual void Start()
        {
        }
        /// <summary>
        /// 玩家走进陷阱触发
        /// </summary>
        /// <param name="playerAttribute"></param>
        public override void onPlayerEnter(PlayerAttribute playerAttribute)
        {
            Debug.Log($"{playerAttribute.gameObject.name} trap in {gameObject.name}");
        }
    }
}