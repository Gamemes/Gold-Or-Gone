using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prop
{
    /// <summary>
    /// 加速道具
    /// </summary>
    public class PropSpeedUp : PropBase
    {
        [SerializeField] float speedUpVal;           //加速的大小（在外边手动输入）
        [SerializeField] float speedUpTime;           //加速的时间（在外边手动输入）
        public override void onPlayerEnter(Player.PlayerAttribute playerAttribute)
        {
            Debug.Log("Player speed is up");
            Destroy(gameObject);
        }
        
    }

}
