using Player;
using UnityEngine;


namespace Prop
{
    public class PropTacit : PropBase
    {
        public int addTacit = 25;
        public override void onPlayerEnter(PlayerAttribute playerAttribute)
        {
            base.onPlayerEnter(playerAttribute);
            Manager.StageManager.CurrentStageManager().gameEventManager.tacitValue += addTacit;
        }
    }
}