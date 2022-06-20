using UnityEngine;
using System;
using System.Collections;

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
        private PlayerAttribute playerAttribute;
        private SpriteRenderer spriteRenderer;
        private void Start()
        {
            blood = maxBlood;
            this.gameObject.tag = "Player";
            playerAttribute = GetComponent<PlayerAttribute>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            damageAction += causeDamage;
        }

        public void causeDamage(int hurtValue)
        {
            Debug.Assert(hurtValue > 0);
            int preBlood = blood;
            blood -= hurtValue;
            StartCoroutine(damageEffect());
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
        IEnumerator damageEffect()
        {
            var _col = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = _col;
        }
        public void addBlood(int addValue)
        {
            Debug.Assert(addValue > 0);
            blood = Mathf.Min(maxBlood, blood + addValue);
        }
        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {

        }
        /// <summary>
        /// Sent when another object enters a trigger collider attached to this
        /// object (2D physics only).
        /// </summary>
        /// <param name="other">The other Collider2D involved in this collision.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Ground")
            {
                if (playerAttribute.playerController.Velocity.y >= playerAttribute.playerController.maxFallSpeed)
                {
                    causeDamage(1);
                }
            }
        }
    }
}