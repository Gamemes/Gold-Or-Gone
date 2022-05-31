using System;
using UnityEngine;

namespace Trap
{
    /// <summary>
    /// 陷阱基类
    /// </summary>
    public class BasicTrap : MonoBehaviour
    {
        // [Tooltip("造成多少伤害")]
        // public int causeDamage = 1;
        public virtual void Start()
        {
        }
        /// <summary>
        /// 玩家走进陷阱触发
        /// </summary>
        /// <param name="playerAttribute">走进的玩家属性</param>
        public virtual void onPlayerEnter(Player.PlayerAttribute playerAttribute)
        {

        }
        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag.CompareTo("Player") == 0)
            {
                var attribute = other.GetComponent<Player.PlayerAttribute>();
                if (attribute == null)
                {
                    var err = new UnityException($"{other.gameObject} has no PlayerAttribute");
                    throw err;
                }
                onPlayerEnter(attribute);
            }
        }
    }
}