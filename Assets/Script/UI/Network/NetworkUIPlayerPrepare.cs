using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace GameUI.Network
{
    [RequireComponent(typeof(UIPlayerPrepare))]
    public class NetworkUIPlayerPrepare : NetworkBehaviour
    {
        private UIPlayerPrepare target = null;
        bool islocal = false;
        bool inited = false;
        private void Start()
        {
            target = GetComponent<UIPlayerPrepare>();

        }
        private void Update()
        {
            if (target.target != null && !inited)
            {
                Debug.Log($"{gameObject.name} inited");
                islocal = target.target.isLocalPlayer;
                if (!islocal)
                    target.enabled = false;
                inited = true;
            }
            if (inited && islocal)
                CmdSyncReady(target.ready);
        }
        [Command(requiresAuthority = false)]
        public void CmdSyncReady(bool ready)
        {
            RpcSyncReady(ready);
        }
        [ClientRpc]
        public void RpcSyncReady(bool ready)
        {
            Debug.Log($"{gameObject} {ready}");
            if (target && !islocal)
            {
                target.ready = ready;
            }
        }
    }
}