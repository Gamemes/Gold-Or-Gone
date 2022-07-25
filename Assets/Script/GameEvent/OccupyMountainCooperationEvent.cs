using UnityEngine;

namespace GameEvent
{
    public class OccupyMountainCooperationEvent : OccupyMountainEvent
    {
        public float rotateInterval = 3f;
        private float fromLastRotate = 1f;
        public int tacitAdd = 30;
        protected override void Update()
        {
            base.Update();
            fromLastRotate += Time.deltaTime;
            if (fromLastRotate > rotateInterval)
            {
                int dir = UnityEngine.Random.Range(1, 10) % 2 == 0 ? -1 : 1;
                int angle = UnityEngine.Random.Range(1, 3) * 90;
                stageManager.RotateGravityDuration(dir * angle, 1f);
                fromLastRotate = 0f;
            }
        }
        protected override EventResult Judge()
        {
            if (playerStayTime >= playerStayTimeTarget)
            {
                gameEventManager.tacitValue += tacitAdd;
                return EventResult.both;
            }
            return EventResult.none;
        }
    }
}