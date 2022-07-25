using UnityEngine;

namespace GameEvent
{
    public class LoseBloodEvent : GameEvent
    {
        //已经造成的伤害
        private int damage = 0;
        [Header("目标伤害")]
        public int damageTarget = 2;
        [Header("玩家加速")]
        public float speedUp = 10f;
        [Header("重力旋转冷却减少")]
        public float coolDown = 0.5f;
        protected override void EnterEvent()
        {
            damage = 0;
            foreach (var item in stageManager.stagePlayerAttributes.Values)
            {
                item.playerHealth.damageAction += this.onPlayerDamage;
            }
            //对玩家施加增益
            stageManager.stagePlayerAttributes[stageManager.GodPlayer].godController.gravityRotateCoolTime -= coolDown;
            foreach (var item in stageManager.stagePlayers)
            {
                if (item != stageManager.GodPlayer)
                {
                    stageManager.stagePlayerAttributes[item].playerController.walkSpeed += speedUp;
                }
            }
        }
        protected override void ReleaseEvent()
        {
            foreach (var item in stageManager.stagePlayers)
            {
                if (item != stageManager.GodPlayer)
                {
                    stageManager.stagePlayerAttributes[item].playerController.walkSpeed -= speedUp;
                }
            }
            if (stageManager.GodPlayer)
                stageManager.stagePlayerAttributes[stageManager.GodPlayer].godController.gravityRotateCoolTime += coolDown;
            foreach (var item in stageManager.stagePlayerAttributes.Values)
            {
                item.playerHealth.damageAction -= this.onPlayerDamage;
            }
        }
        protected override EventResult Judge()
        {
            if (damage >= damageTarget)
            {
                //god win
                stageManager.stagePlayerAttributes[stageManager.GodPlayer].godController.gravityRotateCoolTime -= coolDown;
                return EventResult.god;
            }
            else if (gameEventManager.eventTimer.countSeconds <= 0)
            {
                //human win
                foreach (var item in stageManager.stagePlayers)
                {
                    if (item != stageManager.GodPlayer)
                    {
                        var health = stageManager.stagePlayerAttributes[item].playerHealth;
                        health.MaxBlood += 2;
                        health.blood += 2;
                    }
                }
                return EventResult.human;
            }
            return EventResult.none;
        }
        private void onPlayerDamage(int causeDamage)
        {
            damage += causeDamage;
        }
        private void Update()
        {
            if (damage >= damageTarget)
            {
                this.enabled = false;
            }
        }
    }
}