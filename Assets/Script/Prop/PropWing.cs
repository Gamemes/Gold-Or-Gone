using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Prop
{
    public class PropWing : PropBase
    {
        public override void onPlayerEnter(PlayerAttribute playerAttribute)
        {
            base.onPlayerEnter(playerAttribute);
            playerAttribute.playerController.activeGliding = true;
        }
    }
}
