using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace Manager
{
    public class MyNetworkManager : NetworkManager
    {
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Transform startPos = GetStartPosition();
            GameObject player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);

            // instantiating a "Player" prefab gives it the name "Player(clone)"
            // => appending the connectionId is WAY more useful for debugging!
            player.name = $"{playerPrefab.name} [connId={conn.connectionId}] [randID={UnityEngine.Random.Range(1000, 9999)}]";
            NetworkServer.AddPlayerForConnection(conn, player);
            //触发玩家加入事件.
            MyGameManager.CurrentStageManager().onAddPlayer?.Invoke(player);
        }
        public override void Start()
        {
            base.Start();

        }
    }

}
