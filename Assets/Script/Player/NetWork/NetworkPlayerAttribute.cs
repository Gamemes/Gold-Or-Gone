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
        public override void OnStartClient()
        {
            base.OnStartClient();
            targetCompent = GetComponent<PlayerAttribute>();
            Debug.Assert(targetCompent != null);
            //如果不是本地玩家则取消本地输入
            if (!isClient)
            {
                targetCompent.playerController._activate = false;
            }
        }

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
