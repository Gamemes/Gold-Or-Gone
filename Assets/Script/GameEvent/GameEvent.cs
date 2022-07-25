using System;
using UnityEngine;

namespace GameEvent
{
    public class GameEvent : MonoBehaviour
    {
        public enum EventType
        {
            //合作 | 对抗
            cooperation, combat
        }
        /// <summary>
        /// 事件类型
        /// </summary>
        public EventType eventType = EventType.combat;
        public string eventName = "";
        [TextArea(5, 20)]
        public string detailInfo = "";
        public bool invokeOnAwake = false;
        protected Manager.StageManager stageManager => Manager.StageManager.CurrentStageManager();
        protected GameEventManager gameEventManager => stageManager.gameEventManager;
        protected GameUI.UITimer eventTimer => gameEventManager.eventTimer;
        protected virtual void Awake()
        {
            if (!invokeOnAwake)
                this.enabled = false;
        }
        protected virtual void OnDisable()
        {
            gameEventManager.gameEventUI.CloseSpecialInfo();
            gameEventManager.StopCurrentEvent();
        }
    }
}