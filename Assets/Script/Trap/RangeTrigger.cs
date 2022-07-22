using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;


namespace Trap
{
    public class RangeTrigger : TrapBase
    {
        public Action<PlayerAttribute> stayAction;
        public Action<PlayerAttribute> enterAction;
        public override void onPlayerEnter(PlayerAttribute playerAttribute)
        {
            enterAction?.Invoke(playerAttribute);
        }
        public override void onPlayerStay(PlayerAttribute playerAttribute)
        {
            stayAction?.Invoke(playerAttribute);
        }
    }
}
