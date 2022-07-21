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
    }
}