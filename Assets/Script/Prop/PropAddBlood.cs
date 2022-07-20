using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prop
{
    /// <summary>
    /// 加血道具
    /// </summary>
    public class PropAddBlood : PropBase
    {
        [SerializeField] int addBloodVal;           //要加血的数值（在外边手动输入）
        private void Awake()
        {
            if (addBloodVal == 0)
            {
                addBloodVal = 1;                    //默认初始化
            }
        }
        public override void onPlayerEnter(Player.PlayerAttribute playerAttribute)
        {
            if (playerAttribute.playerHealth.blood < playerAttribute.playerHealth.MaxBlood)
            {
                playerAttribute.playerHealth.addBlood(addBloodVal);
            }
        }
    }
}
