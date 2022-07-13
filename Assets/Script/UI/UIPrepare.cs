using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameUI
{
    public class UIPrepare : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject playerPrepareUI;
        public List<UIPlayerPrepare> playerPrepares;
        void Awake()
        {
            playerPrepares = new List<UIPlayerPrepare>();
            Manager.MyGameManager.CurrentStageManager().onAddPlayer += this.OnAddPlayer;
            Debug.Assert(playerPrepareUI != null);
        }
        void OnAddPlayer(GameObject player)
        {
            GameObject playerui = Instantiate(playerPrepareUI, Vector3.zero, Quaternion.identity, this.transform);
            playerui.transform.localPosition = new Vector3(-300 + 200 * this.playerPrepares.Count, 0, 0);
            var tplayer = playerui.GetComponent<UIPlayerPrepare>();
            tplayer.setPlayer(player.GetComponent<Player.PlayerAttribute>());
            playerPrepares.Add(tplayer);
        }
        // Update is called once per frame
        void Update()
        {
            if (playerPrepares.Count >= 2)
            {
                bool ready = true;
                foreach (var item in playerPrepares)
                {
                    ready = ready & item.ready;
                }
                if (ready)
                {
                    //所有人都准备完毕.
                    Debug.Log($"game start");
                }
            }
        }
    }
}
