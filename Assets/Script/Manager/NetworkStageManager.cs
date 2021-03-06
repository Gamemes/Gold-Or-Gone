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
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            stageManager = GetComponent<StageManager>();
            Debug.Assert(stageManager != null);
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
            stageManager.stageInfo.ShowInfo($"Change God to {player}");
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
            Debug.Log($"net work change grivate size {val}");
            RpcAddGrivate(val);
        }
        [ClientRpc]
        private void RpcAddGrivate(float val)
        {
            stageManager.gravitySize += val;
        }
        [Command(requiresAuthority = false)]
        public void CmdReGame()
        {
            // foreach (var player in stageManager.stagePlayers)
            // {
            //     player.transform.position = NetworkManager.singleton.GetStartPosition().position;
            // }
            RpcReGame();
        }
        [ClientRpc]
        private void RpcReGame()
        {
            stageManager.onReGame?.Invoke();
        }
        [Command(requiresAuthority = false)]
        public void CmdSpawn(GameObject gameObject, Transform parent)
        {
            var res = Instantiate(gameObject, parent);
            NetworkServer.Spawn(res);
        }
    }
}
