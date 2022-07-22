using UnityEngine;
using System;
using System.Collections;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField]
        private int maxBlood = 3;
        public int MaxBlood
        {
            get => maxBlood;
            set
            {
                value = Mathf.Clamp(value, 1, 10);
                maxBlood = value;
            }
        }
        /// <summary>
        /// 免伤
        /// </summary>
        public bool injuryFree = false;
        private int _blood;
        /// <summary>
        /// 玩家的当前血量.
        /// 不要直接调用setter函数.
        /// </summary>
        /// <value>血量</value>
        public int blood
        {
            get => _blood;
            set
            {
                value = Mathf.Clamp(value, 0, maxBlood);
                if (value < blood)
                {
                    if (injuryFree)
                        return;
                    StartCoroutine(DamageEffect());
                    damageAction?.Invoke(blood - value);
                }
                if (value > blood)
                {
                    healAction?.Invoke(value - blood);
                }
                if (value == 0)
                {
                    Manager.MyGameManager.ShowInfoInCurrentStage($"{playerAttribute.playerName} died");
                    onPlayerDead?.Invoke();
                }
                _blood = value;
                onPlayerBloodChange?.Invoke(value);
            }
        }
        /// <summary>
        /// 对玩家造成伤害, 传入造成的伤害
        /// </summary>
        public Action<int> damageAction;
        public Action<int> healAction;
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
        #region 能量
        public Action<int> onEneryChange;
        /// <summary>
        /// 能量值, 达3的时候变成上帝
        /// </summary>
        public int energy
        {
            get => _energy;
            set
            {
                if (value == energy)
                    return;
                if (value == 3)
                {
                    Manager.MyGameManager.instance.currentStage.ChangeGloadPlayer(gameObject);
                }
                _energy = value % 3;
                onEneryChange?.Invoke(_energy);
                Debug.Log($"{gameObject} change energy {value}");
            }
        }
        private int _energy = 0;
        #endregion
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            playerAttribute = GetComponent<PlayerAttribute>();
            Debug.Assert(playerAttribute != null);
        }
        private void Start()
        {

            blood = maxBlood;
            this.gameObject.tag = "Player";
            spriteRenderer = GetComponent<SpriteRenderer>();
            _color = spriteRenderer.color;
            //damageAction += CauseDamage;
            // Manager.MyGameManager.CurrentStageManager().onGameStart += () => { onPlayerBloodChange?.Invoke(blood); };
            // Manager.MyGameManager.CurrentStageManager().onGameStart += () => onEneryChange?.Invoke(energy);
        }

        public void CauseDamage(int hurtValue)
        {
            Debug.Assert(hurtValue > 0);
            blood -= hurtValue;
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
            blood = Mathf.Min(maxBlood, blood + addValue);
        }
        public void ReSetHealth()
        {
            maxBlood = 3;
            this.blood = maxBlood;
            this.energy = 0;
        }
        private void Update()
        {

        }
        private void OnTriggerEnter2D(Collider2D other)
        {

        }
    }
}