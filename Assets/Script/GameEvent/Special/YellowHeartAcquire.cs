using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameEvent.Special
{
    public class YellowHeartAcquire : GameEvent
    {
        public List<GameObject> walls;
        private List<Prop.PropTacit> hearts;
        private int heartPicked = 0;
        private int playerHurt = 0;
        private Player.PlayerAttribute human;
        public bool hasInvoked = false;
        protected override void InitEvent()
        {
            base.InitEvent();
            stageManager.onReGame += () => this.gameObject.SetActive(true);
            if (walls is null)
                walls = new List<GameObject>();
            walls.ForEach((wall) => wall.gameObject.SetActive(gameEventManager.hasEvent));
            hearts = GetComponentsInChildren<Prop.PropTacit>().ToList();
            hearts.ForEach((heart) =>
            {
                heart.playerEnterAction += (player) => { this.heartPicked += 1; };
            });
            gameEventManager.onGameEventInvoke += (ev) =>
            {
                walls.ForEach((wall) => { wall.gameObject.SetActive(true); });
            };
            gameEventManager.onGameEventEnd += (ev) =>
            {
                walls.ForEach((wall) => { wall.gameObject.SetActive(false); });
            };
        }
        void onPlayerHurt(int val)
        {
            playerHurt += val;
        }
        protected override void EnterEvent()
        {
            base.EnterEvent();
            hearts.ForEach((heart) => heart.Enable());
            human = stageManager.stagePlayerAttributes.Values.Single((player) => player.isHuman);
            human.playerHealth.damageAction += this.onPlayerHurt;
        }
        protected override EventResult Judge()
        {
            human.transform.position = Vector3.zero;
            if (playerHurt >= 3)
            {
                if (stageManager.GodPlayer != null)
                    stageManager.stagePlayerAttributes[stageManager.GodPlayer].godController.gravityChangeCoolTime -= 1;
                return EventResult.god;
            }
            else if (heartPicked >= this.hearts.Count)
            {
                gameEventManager.tacitValue += 10;
                return EventResult.both;
            }
            return EventResult.none;
        }
        protected override void ReleaseEvent()
        {
            base.ReleaseEvent();
            if (human != null)
                human.playerHealth.damageAction -= this.onPlayerHurt;
            this.gameObject.SetActive(false);
        }
        private void Update()
        {
            if (heartPicked >= this.hearts.Count)
            {
                this.enabled = false;
            }
            if (playerHurt >= 3)
            {
                this.enabled = false;
            }

        }
    }
}
