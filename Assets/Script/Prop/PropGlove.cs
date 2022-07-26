using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Prop
{
    public class PropGlove : PropBase
    {
        public override void onPlayerEnter(PlayerAttribute playerAttribute)
        {
            base.onPlayerEnter(playerAttribute);
            playerAttribute.playerController.activeClimb = true;
        }
    }
}
