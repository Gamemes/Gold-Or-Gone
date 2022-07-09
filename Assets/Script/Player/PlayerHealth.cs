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
        /// 当玩家生命值变化, 传输玩家生命值作为参数
        /// </summary>
        public Action<int> onPlayerBloodChange;
        /// <summary>
        /// 当玩家死亡
        /// </summary>
        public Action onPlayerDead;
        private PlayerAttribute playerAttribute;
        private SpriteRenderer spriteRenderer;
        private Color _color;
        private void Start()
        {
            blood = maxBlood;
            this.gameObject.tag = "Player";
            playerAttribute = GetComponent<PlayerAttribute>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            _color = spriteRenderer.color;
            damageAction += CauseDamage;
            onPlayerBloodChange?.Invoke(maxBlood);
        }

        public void CauseDamage(int hurtValue)
        {
            Debug.Assert(hurtValue > 0);
            int preBlood = blood;
            blood -= hurtValue;
            StartCoroutine(DamageEffect());
            blood = Mathf.Max(0, blood);
            if (blood <= 0)
            {
                Manager.MyGameManager.ShowInfoInCurrentStage($"{playerAttribute.playerName} died");
                onPlayerDead?.Invoke();
            }
            if (blood < preBlood)
            {
                Debug.Log($"cause damage {hurtValue}, blood now : {blood}");
                onPlayerBloodChange?.Invoke(blood);
            }
        }
        IEnumerator DamageEffect()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = _color;
        }
        public void addBlood(int addValue)
        {
            Debug.Assert(addValue > 0);
            int pre = blood;
            blood = Mathf.Min(maxBlood, blood + addValue);
            if (pre != blood)
                onPlayerBloodChange?.Invoke(blood);
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
                    CauseDamage(1);
                }
            }
        }
    }
}