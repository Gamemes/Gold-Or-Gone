using Player;
using UnityEngine;

namespace Prop
{
    public class AddEnergy : PropBase
    {
        public override void onPlayerEnter(PlayerAttribute playerAttribute)
        {

            playerAttribute.energy++;
            Debug.Log($"add energy {playerAttribute.playerName} {playerAttribute.energy}");
        }
    }
}