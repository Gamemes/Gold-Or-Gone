using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEvent
{
    public class GameEventManager : MonoBehaviour
    {
        public List<GameEvent> gameEvents;
        /// <summary>
        /// 当游戏事件触发
        /// </summary>
        public Action<GameEvent> onGameEventInvoke;
        /// <summary>
        /// 当事件结束
        /// </summary>
        public Action<GameEvent> onGameEventEnd;
        /// <summary>
        /// 当前的游戏事件, null代表无事件
        /// </summary>
        public GameEvent currentGameEvent { get; private set; } = null;
        public bool hasEvent => currentGameEvent != null;
        public GameUI.UITimer eventTimer;
        public Action<int> onTacitValueChange;
        private Manager.StageManager stageManager => Manager.StageManager.CurrentStageManager();
        /// <summary>
        /// 默契值
        /// </summary>
        public int tacitValue
        {
            get => _tacitValue;
            set
            {
                value = Mathf.Clamp(value, 0, 100);
                if (value != _tacitValue)
                {
                    onTacitValueChange?.Invoke(value);
                    _tacitValue = value;
                }
                if (value == 100)
                {
                    stageManager.stageInfo.ShowInfo("恭喜达成合作通关成就!\n (虽然也没什么奖励..");
                    stageManager.GameOver();
                }
            }
        }
        private int _tacitValue = 0;
        public bool debug = true;
        [Header("触发间隔")]
        public float eventInvokeInterval = 180;
        public float fromLastInvoke = 60;
        public bool activeAutoInvoke = false;
        /// <summary>
        /// 下一个事件, 会在事件开始时重置.
        /// </summary>
        public GameEvent nextEvent = null;
        public GameUI.GameEventUI gameEventUI { get; private set; }
        public static GameEventManager currentGameEventManager => Manager.StageManager.currentStageManager.gameEventManager;
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            gameEventUI = GetComponentInChildren<GameUI.GameEventUI>();
        }
        private void Start()
        {
            gameEvents.AddRange(GetComponentsInChildren<GameEvent>());
            Prop.PropBase.onPropPicked += this.ReSetPickProp;
            stageManager.onGameStart += () => { activeAutoInvoke = true; };
            stageManager.onGameOver += () => this.StopCurrentEvent();
            stageManager.onReGame += () => { this.fromLastInvoke = this.eventInvokeInterval - 40f; };
            Debug.Log($"find {gameEvents.Count} events");
        }
        public void InvokeGameEvent(GameEvent gameEvent, bool force = false)
        {
            if (hasEvent)
            {
                if (!force)
                    return;
                StopCurrentEvent();
            }
            gameEvent.enabled = true;
            this.currentGameEvent = gameEvent;
            onGameEventInvoke?.Invoke(gameEvent);
            Debug.Log($"{gameEvent.eventName} start");
            stageManager.stageInfo.ShowInfo($"{gameEvent.eventName} 事件开始!", Color.red, Color.black);
        }
        public void InvokeGameEvent(int eventIdx)
        {
            if (eventIdx >= gameEvents.Count)
                return;
            InvokeGameEvent(this.gameEvents[eventIdx]);
        }
        public void StopCurrentEvent()
        {
            if (!hasEvent)
                return;
            currentGameEvent.enabled = false;
            onGameEventEnd?.Invoke(currentGameEvent);
            this.currentGameEvent = null;
        }
        public void DelayInvoke(Action func, float seconds)
        {
            StartCoroutine(Utils.Utils.DelayInvoke(func, seconds));
        }

        private void ReSetPickProp(Prop.PropBase prop, GameObject gameObject)
        {
            fromLastInvoke = Mathf.Max(fromLastInvoke, eventInvokeInterval - 60f);
        }

        private void Update()
        {
            if (!activeAutoInvoke)
                return;
            if (!hasEvent)
                fromLastInvoke += Time.deltaTime;
            if (fromLastInvoke >= eventInvokeInterval - 10f)
            {
                nextEvent = gameEvents[UnityEngine.Random.Range(0, gameEvents.Count - 1)];
                gameEventUI.ShowDetail(nextEvent.eventName, nextEvent.detailInfo, -1);
                stageManager.stageInfo.ShowInfo($"10s后开启下一个事件\n{nextEvent.eventName}");
                this.DelayInvoke(() =>
                {
                    InvokeGameEvent(nextEvent);
                    nextEvent = null;
                }, 10f);
                fromLastInvoke = 0f;
            }
        }
        private void OnDestroy()
        {
            Prop.PropBase.onPropPicked -= this.ReSetPickProp;
        }
        private string text = "0";
        private void OnGUI()
        {
            if (!this.debug)
                return;
            text = GUI.TextField(new Rect(20, 90, 100, 20), text);
            int idx = 0;
            if (text.Length > 0)
                int.TryParse(text, out idx);
            if (GUI.Button(new Rect(20, 60, 160, 20), "start event"))
            {
                InvokeGameEvent(gameEvents[idx]);
            }
            if (GUI.Button(new Rect(20, 120, 160, 20), "stop event"))
            {
                StopCurrentEvent();
            }
            if (GUI.Button(new Rect(20, 160, 160, 20), "tacit add"))
            {
                tacitValue += 10;
            }
        }
    }
}