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
        public enum EventResult
        {
            //人类胜利, 上帝胜利, 双方胜利, 双方全输
            human, god, both, none
        }
        /// <summary>
        /// 事件类型
        /// </summary>
        public EventType eventType = EventType.combat;
        public string eventName = "";
        [TextArea(5, 20)]
        public string detailInfo = "";
        [TextArea]
        public string godWinInfo = "";
        [TextArea]
        public string humanWinInfo = "";
        public bool invokeOnAwake = false;
        [Header("持续时间")]
        public float duration = 90f;
        protected Manager.StageManager stageManager => Manager.StageManager.CurrentStageManager();
        protected GameEventManager gameEventManager => stageManager.gameEventManager;
        protected GameUI.UITimer eventTimer => gameEventManager.eventTimer;
        protected virtual void Awake()
        {
            if (!invokeOnAwake)
                this.enabled = false;
            InitEvent();
        }
        protected virtual void OnEnable()
        {
            EnterEvent();
            gameEventManager.eventTimer.startTiming(duration, this.TimeUp);
        }
        protected virtual void InitEvent()
        {

        }
        protected virtual void EnterEvent()
        {

        }
        protected virtual void ReleaseEvent()
        {

        }
        protected virtual EventResult Judge()
        {
            return EventResult.none;
        }
        protected virtual void TimeUp()
        {
            this.enabled = false;
        }
        protected virtual void OnDisable()
        {
            if (!gameEventManager.hasEvent)
                return;
            gameEventManager.gameEventUI.CloseSpecialInfo();
            gameEventManager.StopCurrentEvent();
            ReleaseEvent();
            switch (this.Judge())
            {
                case EventResult.human:
                    Debug.Log($"human win");
                    stageManager.stageInfo.ShowInfo($"Human胜利\n{humanWinInfo}");
                    break;
                case EventResult.god:
                    Debug.Log($"god win");
                    stageManager.stageInfo.ShowInfo($"上帝胜利\n{godWinInfo}");
                    break;
                case EventResult.both:
                    Debug.Log("双方胜利");
                    stageManager.stageInfo.ShowInfo($"玩家胜利\n{humanWinInfo}\n{godWinInfo}");
                    break;
                default:
                    Debug.Log($"玩家失败");
                    break;
            }
            gameEventManager.eventTimer.stopTiming(false);
        }
    }
}