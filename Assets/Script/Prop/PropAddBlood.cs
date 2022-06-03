using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prop
{
    /// <summary>
    /// ¼ÓÑªµÀ¾ß
    /// </summary>
    public class PropAddBlood : PropBase
    {
        public override void onPlayerEnter(Player.PlayerAttribute playerAttribute)
        {
            if(playerAttribute.playerHealth.blood < playerAttribute.playerHealth.MaxBlood)
            {
                playerAttribute.playerHealth.addBlood(1);
                Debug.Log("Player blood is +1");
                Destroy(gameObject);
            }
        }
    }
}
