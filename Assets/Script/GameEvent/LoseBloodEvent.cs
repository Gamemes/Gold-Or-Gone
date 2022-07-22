using UnityEngine;

namespace GameEvent
{
    public class LoseBloodEvent : GameEvent
    {
        [Header("持续时间(s)")]
        public float duration = 60f;
        //已经造成的伤害
        private int damage = 0;
        [Header("目标伤害")]
        public int damageTarget = 2;
        [Header("玩家加速")]
        public float speedUp = 10f;
        [Header("重力旋转冷却减少")]
        public float coolDown = 0.5f;
        private void OnEnable()
        {
            damage = 0;
            gameEventManager.eventTimer.startTiming(duration, this.onTimeUp);
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
        private void onTimeUp()
        {
            this.enabled = false;
        }
        protected override void OnDisable()
        {
            if (!gameEventManager.hasEvent)
                return;
            base.OnDisable();
            //取消增益
            foreach (var item in stageManager.stagePlayers)
            {
                if (item != stageManager.GodPlayer)
                {
                    stageManager.stagePlayerAttributes[item].playerController.walkSpeed -= speedUp;
                }
            }
            stageManager.stagePlayerAttributes[stageManager.GodPlayer].godController.gravityRotateCoolTime += coolDown;
            //如果达到目标
            if (damage >= damageTarget)
            {
                Debug.Log($"god win");
                stageManager.stagePlayerAttributes[stageManager.GodPlayer].godController.gravityRotateCoolTime -= coolDown;
            }
            else if (gameEventManager.eventTimer.countSeconds <= 0)
            {
                Debug.Log($"human win");
                foreach (var item in stageManager.stagePlayers)
                {
                    if (item != stageManager.GodPlayer)
                    {
                        var health = stageManager.stagePlayerAttributes[item].playerHealth;
                        health.MaxBlood += 2;
                        health.blood += 2;
                    }
                }
            }
            foreach (var item in stageManager.stagePlayerAttributes.Values)
            {
                item.playerHealth.damageAction -= this.onPlayerDamage;
            }
            gameEventManager.eventTimer.stopTiming(false);
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