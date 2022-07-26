using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Prop
{
    public class PropShoes : PropBase
    {
        public override void onPlayerEnter(PlayerAttribute playerAttribute)
        {
            base.onPlayerEnter(playerAttribute);
            playerAttribute.playerController.activeSprint = true;
        }
    }
}
