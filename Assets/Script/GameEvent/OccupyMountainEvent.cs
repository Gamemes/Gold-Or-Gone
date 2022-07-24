using UnityEngine;
using System.Collections.Generic;

namespace GameEvent
{
    using Trap;
    public class OccupyMountainEvent : GameEvent
    {
        public float duration = 180;
        public RangeTrigger rangeTrigger;
        public int triggerNums;
        private List<RangeTrigger> triggerList;
        [SerializeField] private PolygonCollider2D polygonCollider;
        public float playerStayTime { get; private set; } = 0f;
        public float playerStayTimeTarget = 30f;
        private bool isPlayerStay = false;
        public Vector2 uiPos;
        protected override void Awake()
        {
            base.Awake();
            triggerList = new List<RangeTrigger>();
            for (var i = 0; i < triggerNums; i++)
            {
                var trigger = Instantiate<RangeTrigger>(rangeTrigger, this.transform);
                trigger.gameObject.SetActive(false);
                trigger.stayAction = (playerAtt) =>
                {
                    this.isPlayerStay = true;
                };
                triggerList.Add(trigger);
            }
        }
        protected Vector2 GetPos()
        {
            Vector2 res = new Vector2(0, 0);
            do
            {
                res.Set(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
            } while (!polygonCollider.OverlapPoint(res));
            return res;
        }
        private void OnEnable()
        {
            //init
            playerStayTime = 0;
            foreach (var trigger in triggerList)
            {
                trigger.transform.position = GetPos();
                trigger.gameObject.SetActive(true);
            }
            gameEventManager.eventTimer.startTiming(duration, this.TimeUp);
            foreach (var trap in GameObject.FindObjectsOfType<Trap.TrapThorn>())
            {
                trap.activeDamage = false;
            }
        }
        protected override void OnDisable()
        {
            if (!gameEventManager.hasEvent)
            {
                return;
            }
            //收尾
            base.OnDisable();
            foreach (var trigger in triggerList)
            {
                trigger.gameObject.SetActive(false);
            }
            foreach (var trap in GameObject.FindObjectsOfType<Trap.TrapThorn>(true))
            {
                trap.activeDamage = true;
            }
            //玩家获胜
            if (playerStayTime >= playerStayTimeTarget)
            {
                Debug.Log($"human win");
                foreach (var player in stageManager.stagePlayerAttributes.Values)
                {
                    if (player.isHuman)
                    {
                        player.playerController.walkSpeed += 4f;
                        //三分钟后取消增强
                        gameEventManager.DelayInvoke(() =>
                        {
                            player.playerController.walkSpeed -= 4f;
                        }, 180);
                    }
                }
            }
            else if (gameEventManager.eventTimer.countSeconds <= 0)
            {
                Debug.Log($"god win");
                foreach (var trap in GameObject.FindObjectsOfType<Trap.TrapThorn>())
                {
                    trap.damage += 1;
                    //三分钟后取消增强
                    gameEventManager.DelayInvoke(() =>
                    {
                        trap.damage -= 1;
                    }, 180);
                }
            }
            gameEventManager.eventTimer.stopTiming(false);
        }
        protected void Update()
        {
            if (isPlayerStay)
            {
                playerStayTime += Time.deltaTime;
            }
            isPlayerStay = false;
            if (playerStayTime >= playerStayTimeTarget)
            {
                this.enabled = false;
            }
        }
        protected void TimeUp()
        {
            this.enabled = false;
        }
        protected void OnGUI()
        {
            if (!gameEventManager.debug)
                return;
            GUI.Label(new Rect(uiPos, new Vector2(100, 20)), $"{playerStayTime}");
        }

    }
}