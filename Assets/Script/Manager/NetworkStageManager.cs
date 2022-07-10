using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Manager
{
    /// <summary>
    /// 本地Stage的在线扩展. 
    /// </summary>
    public class NetworkStageManager : NetworkBehaviour
    {
        private StageManager stageManager;
        public GameObject playerUI;
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            stageManager = GetComponent<StageManager>();
            Debug.Assert(stageManager != null);
            Debug.Assert(playerUI != null);
            stageManager.onAddPlayer += this.AddPlayer;
        }
        [Command(requiresAuthority = false)]
        public void CmdChangeGodPlayer(GameObject player)
        {
            RpcChangeGodPlayer(player);
        }
        [ClientRpc]
        private void RpcChangeGodPlayer(GameObject player)
        {
            stageManager.GodPlayer = player;
        }
        [Command(requiresAuthority = false)]
        public void CmdRotateGravityDuration(float angle, float duration)
        {
            RpcRotateGravityDuration(angle, duration);
        }
        [ClientRpc]
        private void RpcRotateGravityDuration(float angle, float duration)
        {
            StartCoroutine(stageManager._rotateGravityDuration(angle, duration));
        }
        [Command(requiresAuthority = false)]
        public void CmdAddGrivate(float val)
        {
            RpcAddGrivate(val);
        }
        [ClientRpc]
        private void RpcAddGrivate(float val)
        {
            stageManager.gravitySize += val;
        }
        private void AddPlayer(GameObject player)
        {
            //出于一些问题, UI并不能和玩家一同生成, 我们需要手动生产UI绑定到玩家;
            if (playerUI == null)
                return;
            var pui = Instantiate(playerUI);
            var uimanager = pui.GetComponent<Player.PlayerUIManager>();
            uimanager.targetPlayer = player.GetComponent<Player.PlayerAttribute>();
            uimanager.playerTransform = player.transform;
            pui.name = $"{player.name} UI(auto create by stageManager)";
        }
    }
}
