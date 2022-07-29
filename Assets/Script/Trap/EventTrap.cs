using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Trap
{
    public class EventTrap : TrapBase
    {
        public GameEvent.GameEvent targetEvent;
        public float invokeAfterTime = 3f;
        private float staytime = 0;
        private bool invoked = false;

        public override void onPlayerEnter(PlayerAttribute playerAttribute)
        {
            base.onPlayerEnter(playerAttribute);
            staytime = 0f;
        }
        public override void onPlayerStay(PlayerAttribute playerAttribute)
        {
            base.onPlayerStay(playerAttribute);
            if (invoked || targetEvent is null)
                return;
            staytime += Time.deltaTime;
            if (staytime >= invokeAfterTime)
            {
                GameEvent.GameEventManager.currentGameEventManager.InvokeGameEvent(targetEvent);
                invoked = true;
            }
        }
        public override void onPlayerExit(PlayerAttribute playerAttribute)
        {
            base.onPlayerExit(playerAttribute);
            invoked = false;
        }
    }
}
