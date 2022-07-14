using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Player
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
        }
        private PlayerAttribute targetCompent;
        [Tooltip("True来开启同步")]
        public bool activeSync = true;
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            Manager.MyGameManager.CurrentStageManager().stageCamera.Follow = this.transform;
        }
        private void Start()
        {
            targetCompent = GetComponent<PlayerAttribute>();
            targetCompent.isLocalPlayer = isLocalPlayer;
            Debug.Assert(targetCompent != null);
            //如果不是本地玩家则取消本地输入
            if (!isLocalPlayer)
            {
                targetCompent.playerController._activate = false;
                targetCompent.frameInput.enabled = false;
            }
            else
            {
                targetCompent.playerController._activate = true;
            }
        }
        // Update is called once per frame
        void Update()
        {
            if (isLocalPlayer)
            {
                CmdSyncAttribute(new PlayerSyncAttribute(targetCompent.playerHealth.blood, targetCompent.playerHealth.energy));
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
            if (targetCompent)
            {
                //Debug.Log($"sync energy {energy} {netId} {gameObject.name}");
                targetCompent.playerHealth.energy = playerSync.energy;
                targetCompent.playerHealth.blood = playerSync.blood;

            }
        }
    }

}
