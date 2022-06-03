using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prop
{
    /// <summary>
    /// 可拾取道具基类.
    /// </summary>
    public class PropBase : MonoBehaviour
    {
        /// <summary>
        /// 玩家走进触发
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