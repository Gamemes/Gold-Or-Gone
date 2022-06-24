using Player;
using UnityEngine;

namespace Prop
{
    public class AddEnergy : PropBase
    {
        public override void onPlayerEnter(PlayerAttribute playerAttribute)
        {

            playerAttribute.energy++;
            Debug.Log($"{playerAttribute.playerName} {playerAttribute.energy}");
        }
    }
}