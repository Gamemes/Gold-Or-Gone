using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Trap;
using Player;

namespace GameEvent
{
    public class DodgeballEvent : GameEvent
    {
        public RangeTrigger triggerPrefab;
        public List<RangeTrigger> triggerList;
        public float triggerMoveSpeed = 15f;
        public int triggerNums = 10;
        private PlayerAttribute humanTarget;
        public int collidedNums { get; private set; } = 0;
        public int collideNumTarget = 4;
        private Vector3 followPos = Vector3.zero;
        private float updatePosTime = 100f;
        public float delayUpdatePos = 0.2f;
        public string specialIInfo => $"已经触碰了:\n{collidedNums}/{collideNumTarget}";
        protected override void InitEvent()
        {
            triggerList = new List<RangeTrigger>();
            for (var i = 0; i < triggerNums; i++)
            {
                var trigger = Instantiate<RangeTrigger>(triggerPrefab, this.transform);
                trigger.gameObject.SetActive(false);
                trigger.enterAction = (player) =>
                {
                    trigger.gameObject.SetActive(false);
                    collidedNums++;
                    gameEventManager.gameEventUI.ShowSpecialInfo(specialIInfo);
                };
                trigger.everyFrameAction = () =>
                {
                    if (!humanTarget)
                        return;
                    trigger.transform.position = Vector3.MoveTowards(trigger.transform.position, followPos, triggerMoveSpeed * Time.deltaTime);
                };
                triggerList.Add(trigger);
            }
        }
        private Vector2 GetPos()
        {
            Vector3 res = new Vector3(0, 0, 0);
            do
            {
                res.Set(UnityEngine.Random.Range(-150, 150), UnityEngine.Random.Range(-150, 150), 0);
            } while ((humanTarget.transform.position - res).sqrMagnitude < 500);
            return res;
        }
        protected override void EnterEvent()
        {
            collidedNums = 0;
            humanTarget = stageManager.stagePlayerAttributes.Values.Single((player) => player.isHuman);
            foreach (var trigger in this.triggerList)
            {
                trigger.transform.position = GetPos();
                trigger.gameObject.SetActive(true);
            }
            gameEventManager.gameEventUI.ShowSpecialInfo(specialIInfo);
        }

        protected override void ReleaseEvent()
        {
            foreach (var trigger in this.triggerList)
            {
                trigger.gameObject.SetActive(false);
            }
        }

        protected override EventResult Judge()
        {
            if (collidedNums >= collideNumTarget)
            {
                Debug.Log($"God win");
                if (stageManager.GodPlayer)
                    stageManager.stagePlayerAttributes[stageManager.GodPlayer].godController.activeFlip = true;
                return EventResult.god;
            }
            else if (gameEventManager.eventTimer.countSeconds <= 0f)
            {
                Debug.Log("Human win");
                humanTarget.playerController.maxClimbJumpTime += 1;
                return EventResult.human;
            }
            return EventResult.none;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            if (updatePosTime > delayUpdatePos)
            {
                updatePosTime = 0f;
                followPos = humanTarget.transform.position;
            }
            updatePosTime += Time.deltaTime;
            if (collidedNums >= collideNumTarget)
            {
                this.enabled = false;
            }
        }
    }
}