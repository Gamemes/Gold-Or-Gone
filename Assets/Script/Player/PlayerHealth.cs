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
                if (value == blood)
                    return;
                if (value < 0 || value > maxBlood)
                    return;
                if (value < blood)
                    StartCoroutine(DamageEffect());
                if (value == 0)
                {
                    Manager.MyGameManager.ShowInfoInCurrentStage($"{playerAttribute.playerName} died");
                    onPlayerDead?.Invoke();
                }
                _blood = value;
                onPlayerBloodChange?.Invoke(value);
                Debug.Log($"{gameObject} change blood {value}");
            }
        }
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
            damageAction += CauseDamage;
            Manager.MyGameManager.CurrentStageManager().onGameStart += () => { onPlayerBloodChange?.Invoke(blood); };
            Manager.MyGameManager.CurrentStageManager().onGameStart += () => onEneryChange?.Invoke(energy);
        }

        public void CauseDamage(int hurtValue)
        {
            Debug.Assert(hurtValue > 0);
            if (Manager.MyGameManager.CurrentStageManager().isdebug)
                return;
            blood -= hurtValue;
            blood = Mathf.Max(0, blood);
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