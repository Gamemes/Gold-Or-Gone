using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trap
{
    /// <summary>
    /// 地刺陷阱
    /// </summary>
    public class TrapThorn : TrapBase
    {
        [SerializeField] int damage;            //伤害数值（在外面手动输入）
        private void Awake()
        {
            if (damage == 0)
            {
                damage = 1;                     //默认初始化
            }
        }
        public override void onPlayerEnter(Player.PlayerAttribute playerAttribute)
        {
            playerAttribute.playerHealth.damageAction(damage);      //对玩家造成伤害
        }
    }


}