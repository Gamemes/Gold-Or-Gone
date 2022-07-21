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
            }
        }
        private int _tacitValue = 0;
        private void Start()
        {
            gameEvents.AddRange(GameObject.FindObjectsOfType<GameEvent>());
            Debug.Log($"find {gameEvents.Count} events");
        }
        public void InvokeGameEvent(GameEvent gameEvent)
        {
            if (hasEvent)
                return;
            gameEvent.enabled = true;
            this.currentGameEvent = gameEvent;
            onGameEventInvoke?.Invoke(gameEvent);
        }
        public void StopCurrentEvent()
        {
            if (!hasEvent)
                return;
            currentGameEvent.enabled = false;
            onGameEventEnd?.Invoke(currentGameEvent);
            this.currentGameEvent = null;
        }
        private void OnGUI()
        {
            if (GUI.Button(new Rect(20, 40, 80, 20), "开启计时77秒"))
            {
                eventTimer.startTiming(36, () =>
                {
                    Debug.Log($"time up");
                });
            }
        }
    }
}