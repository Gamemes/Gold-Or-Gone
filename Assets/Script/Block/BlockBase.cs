using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Block
{
    /// <summary>
    /// 方块基类
    /// </summary>
    public class BlockBase : MonoBehaviour
    {
        /// <summary>
        /// 玩家接触方块触发
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
