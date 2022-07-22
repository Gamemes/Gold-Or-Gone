using System;
using Player;
using UnityEngine;

namespace Trap
{
    /// <summary>
    /// 陷阱基类
    /// </summary>
    public class TrapBase : MonoBehaviour
    {
        /// <summary>
        /// 玩家走进陷阱触发
        /// </summary>
        /// <param name="playerAttribute">走进的玩家属性</param>
        public virtual void onPlayerEnter(Player.PlayerAttribute playerAttribute)
        {

        }
        /// <summary>
        /// 玩家一直呆在范围内
        /// </summary>
        /// <param name="playerAttribute"></param>
        public virtual void onPlayerStay(Player.PlayerAttribute playerAttribute)
        {

        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
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
        protected virtual void OnTriggerStay2D(Collider2D other)
        {
            if (other.tag.CompareTo("Player") == 0)
            {
                var attribute = Manager.StageManager.CurrentStageManager().stagePlayerAttributes[other.gameObject];
                if (attribute == null)
                {
                    var err = new UnityException($"{other.gameObject} has no PlayerAttribute");
                    throw err;
                }
                onPlayerStay(attribute);
            }
        }
    }
}