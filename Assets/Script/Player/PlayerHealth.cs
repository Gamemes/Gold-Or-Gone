using UnityEngine;
using System;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField]
        private int maxBlood = 3;
        public int MaxBlood { get => maxBlood; }
        public int blood { get; private set; }
        /// <summary>
        /// 对玩家造成伤害
        /// </summary>
        public Action<int> damageAction;
        /// <summary>
        /// 当玩家受伤(生命值减少), 传输玩家生命值作为参数
        /// </summary>
        public Action<int> onPlayerHurt;
        /// <summary>
        /// 当玩家死亡
        /// </summary>
        public Action onPlayerDead;
        private void Start()
        {
            blood = maxBlood;
            this.gameObject.tag = "Player";
            damageAction += causeDamage;
        }

        public void causeDamage(int hurtValue)
        {
            Debug.Assert(hurtValue > 0);
            int preBlood = blood;
            blood -= hurtValue;
            blood = Mathf.Max(0, blood);
            if (blood <= 0)
            {
                onPlayerDead?.Invoke();
            }
            if (blood < preBlood)
            {
                Debug.Log($"cause damage {hurtValue}, blood now : {blood}");
                onPlayerHurt?.Invoke(blood);
            }
        }
        public void addBlood(int addValue)
        {
            Debug.Assert(addValue > 0);
            blood = Mathf.Min(maxBlood, blood + addValue);
        }

    }
}