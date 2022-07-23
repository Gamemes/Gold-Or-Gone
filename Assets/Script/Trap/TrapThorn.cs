using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Trap
{
    /// <summary>
    /// 地刺陷阱
    /// </summary>
    public class TrapThorn : TrapBase
    {
        [Range(1, 10)]
        public int damage = 1;            //伤害数值（在外面手动输入）
        public float maxStayTime = 3f;
        private float stayTime = 0f;
        private void Awake()
        {
            if (damage == 0)
            {
                damage = 1;                     //默认初始化
            }
        }
        public override void onPlayerEnter(Player.PlayerAttribute playerAttribute)
        {
            stayTime = 0f;
            playerAttribute.playerHealth.CauseDamage(damage);      //对玩家造成伤害
        }
        public override void onPlayerStay(PlayerAttribute playerAttribute)
        {
            stayTime += Time.deltaTime;
            if (stayTime >= maxStayTime)
            {
                playerAttribute.playerHealth.CauseDamage(damage);
                stayTime = 0;
            }
        }
    }


}