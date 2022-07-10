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
            Debug.Assert(targetCompent != null);
            //如果不是本地玩家则取消本地输入
            if (!isLocalPlayer)
            {
                targetCompent.playerController._activate = false;
            }
            else
            {
                targetCompent.playerController._activate = true;
            }
        }
        // Update is called once per frame
        void Update()
        {

        }
    }

}
