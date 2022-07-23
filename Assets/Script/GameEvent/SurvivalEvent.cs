using UnityEngine;
using System.Linq;

namespace GameEvent
{
    public class SurvivalEvent : GameEvent
    {
        [Header("间隔时间")]
        public float rotateInterval = 3f;
        private float fromLastRotate = 0f;
        [Header("玩家收到伤害小于这个数就胜利")]
        public int damageTarget = 3;
        private int causedDamage = 0;
        public float duration = 90;
        private Player.PlayerAttribute human;
        private void OnEnable()
        {
            gameEventManager.eventTimer.startTiming(duration, () =>
            {
                this.enabled = false;
            });
            causedDamage = 0;
            human = stageManager.stagePlayerAttributes.Values.Single((player) => player.isHuman);
            if (human == null)
            {
                Debug.LogError($"未能获取到人类玩家");
                this.enabled = false;
                return;
            }
            human.playerHealth.damageAction += this.onPlayerDamage;

        }
        void onPlayerDamage(int val)
        {
            causedDamage += val;
        }
        protected override void OnDisable()
        {
            if (!gameEventManager.hasEvent)
                return;
            base.OnDisable();
            if (causedDamage < damageTarget && eventTimer.countSeconds <= 0f)
            {
                gameEventManager.tacitValue += 20;
            }
            if (human)
            {
                human.playerHealth.damageAction -= this.onPlayerDamage;
            }
            eventTimer.stopTiming(false);
        }
        private void Update()
        {
            if (causedDamage >= damageTarget)
            {
                this.enabled = false;
            }
            if (fromLastRotate > rotateInterval)
            {
                int dir = UnityEngine.Random.Range(1, 10) % 2 == 0 ? -1 : 1;
                int angle = UnityEngine.Random.Range(1, 3) * 90;
                stageManager.RotateGravityDuration(dir * angle, 1f);
                fromLastRotate = 0f;
            }
            fromLastRotate += Time.deltaTime;
        }

    }
}