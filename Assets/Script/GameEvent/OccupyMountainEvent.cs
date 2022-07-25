using UnityEngine;
using System.Collections.Generic;

namespace GameEvent
{
    using Trap;
    public class OccupyMountainEvent : GameEvent
    {
        public RangeTrigger rangeTrigger;
        public int triggerNums;
        private List<RangeTrigger> triggerList;
        [SerializeField] private PolygonCollider2D polygonCollider;
        public float playerStayTime { get; private set; } = 0f;
        public float playerStayTimeTarget = 30f;
        private bool isPlayerStay = false;
        public Vector2 uiPos;
        protected override void InitEvent()
        {
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
        protected override void EnterEvent()
        {
            //init
            playerStayTime = 0;
            foreach (var trigger in triggerList)
            {
                trigger.transform.position = GetPos();
                trigger.gameObject.SetActive(true);
            }
            foreach (var trap in GameObject.FindObjectsOfType<Trap.TrapThorn>())
            {
                trap.activeDamage = false;
            }
        }
        protected override void ReleaseEvent()
        {
            foreach (var trigger in triggerList)
            {
                trigger.gameObject.SetActive(false);
            }
            foreach (var trap in GameObject.FindObjectsOfType<Trap.TrapThorn>(true))
            {
                trap.activeDamage = true;
            }
        }
        protected override EventResult Judge()
        {
            if (playerStayTime >= playerStayTimeTarget)
            {
                //human win
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
                return EventResult.human;
            }
            else if (gameEventManager.eventTimer.countSeconds <= 0)
            {
                //god win
                foreach (var trap in GameObject.FindObjectsOfType<Trap.TrapThorn>())
                {
                    trap.damage += 1;
                    //三分钟后取消增强
                    gameEventManager.DelayInvoke(() =>
                    {
                        trap.damage -= 1;
                    }, 180);
                }
                return EventResult.god;
            }
            return EventResult.none;
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
        protected void OnGUI()
        {
            if (!gameEventManager.debug)
                return;
            GUI.Label(new Rect(uiPos, new Vector2(100, 20)), $"{playerStayTime}");
        }

    }
}