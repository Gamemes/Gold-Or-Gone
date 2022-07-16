using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Player.Network
{
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(NetworkAnimator))]
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkPlayerAttribute : NetworkBehaviour
    {
        // Start is called before the first frame update
        public struct PlayerSyncAttribute
        {
            public int blood;
            public int energy;
            public PlayerSyncAttribute(int blood, int energy)
            {
                this.blood = blood;
                this.energy = energy;
            }
            public void update(PlayerHealth health)
            {
                this.blood = health.blood;
                this.energy = health.energy;
            }
        }
        private PlayerAttribute targetCompent;
        [Tooltip("True来开启同步")]
        public bool activeSync = true;
        private float timeFromLastSync = 100f;
        private PlayerSyncAttribute preAttribute;
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            Manager.MyGameManager.CurrentStageManager().stageCamera.Follow = this.transform;
        }
        private void Start()
        {
            targetCompent = GetComponent<PlayerAttribute>();
            preAttribute = new PlayerSyncAttribute(targetCompent.playerHealth.blood, targetCompent.playerHealth.energy);
            targetCompent.isLocalPlayer = isLocalPlayer;
            Debug.Assert(targetCompent != null);
            //如果不是本地玩家则取消本地输入
            if (!isLocalPlayer)
            {
                targetCompent.playerController._activate = false;
                targetCompent.frameInput.enabled = false;
                gameObject.tag = "Player Network";
            }
            else
            {
                targetCompent.playerController._activate = true;
            }
        }
        bool needSync()
        {
            bool need = false;
            if (targetCompent.playerHealth.blood != preAttribute.blood)
            {
                need = true;
            }
            if (targetCompent.playerHealth.energy != preAttribute.energy)
            {
                need = true;
            }

            //每10s也要同步一次
            timeFromLastSync += Time.deltaTime;
            if (timeFromLastSync > 5f)
            {
                need = true;
                timeFromLastSync = 0f;
            }
            if (need)
                preAttribute.update(targetCompent.playerHealth);
            return need;
        }
        void Update()
        {
            if (isLocalPlayer && needSync())
            {
                CmdSyncAttribute(preAttribute);
            }
            else
            {
                targetCompent.playerHealth.energy = preAttribute.energy;
                targetCompent.playerHealth.blood = preAttribute.blood;
            }
        }
        [Command]
        public void CmdSyncAttribute(PlayerSyncAttribute playerSync)
        {
            this.RpcSyncAttribute(playerSync);
        }
        [ClientRpc]
        public void RpcSyncAttribute(PlayerSyncAttribute playerSync)
        {
            if (!isLocalPlayer)
            {
                preAttribute = playerSync;
            }
        }
    }

}
