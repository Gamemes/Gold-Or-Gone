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
        private bool isStart = false;
        void Awake()
        {
            playerPrepares = new List<UIPlayerPrepare>();
            Debug.Assert(playerPrepareUI != null);
        }
        private void Start()
        {
            Manager.MyGameManager.CurrentStageManager().onAddPlayer += this.OnAddPlayer;
        }
        void OnAddPlayer(GameObject player)
        {
            GameObject playerui = Instantiate(playerPrepareUI, this.transform);
            playerui.transform.localPosition = new Vector3(-300 + 200 * this.playerPrepares.Count, 0, 0);
            var tplayer = playerui.GetComponent<UIPlayerPrepare>();
            tplayer.setPlayer(player.GetComponent<Player.PlayerAttribute>());
            playerPrepares.Add(tplayer);
        }
        // Update is called once per frame
        void Update()
        {
            if (playerPrepares.Count >= 2 && !isStart)
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
                    Manager.MyGameManager.CurrentStageManager().StartGame();
                    Destroy(this.gameObject);
                    isStart = true;
                }
            }
        }
    }
}
