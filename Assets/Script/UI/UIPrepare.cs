using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utils;
using TMPro;

namespace GameUI
{
    public class UIPrepare : MonoBehaviour
    {
        // Start is called before the first frame update
        public TextMeshProUGUI specialText;
        public GameObject playerPrepareUI;
        public List<UIPlayerPrepare> playerPrepares;
        public GameObject story;
        private bool isStart = false;
        void Awake()
        {
            playerPrepares = new List<UIPlayerPrepare>();
            Debug.Assert(playerPrepareUI != null);
        }
        private void Start()
        {
            this.init();
            //StartCoroutine(Utils.Utils.DelayInvoke(() => { }, 0.5f));
        }
        public void init()
        {
            Manager.MyGameManager.CurrentStageManager().onAddPlayer += this.OnAddPlayer;
            foreach (var player in Manager.StageManager.CurrentStageManager().stagePlayers)
            {
                OnAddPlayer(player);
            }
        }
        void OnAddPlayer(GameObject player)
        {
            GameObject playerui = Instantiate(playerPrepareUI, this.transform);
            int _l = this.playerPrepares.Count == 0 ? -1 : 1;
            playerui.transform.localPosition = new Vector3(_l * 50, 0, 0);
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
                    playerPrepares.ForEach((ui) => ui.activeBar = true);
                    specialText.text = "快速点击按键争夺God:";
                    this.DelayInvoke(() =>
                    {
                        playerPrepares.ForEach((ui) => ui.activeBar = false);
                        playerPrepares.Sort((a, b) => { return b.barImage.rectTransform.rect.width.CompareTo(a.barImage.rectTransform.rect.width); });
                        var god = playerPrepares[0];
                        var human = playerPrepares[1];
                        Debug.Log(god.target.playerName);
                        god.transform.localPosition = new Vector3(260, 0, 0);
                        human.transform.localPosition = new Vector3(-260, 0, 0);
                        story.SetActive(true);
                        this.DelayInvoke(() =>
                        {
                            Manager.MyGameManager.CurrentStageManager().StartGame(god.target.gameObject);
                            Destroy(this.gameObject);
                        }, 3);
                    }, 3);

                    isStart = true;
                }
            }
        }
    }
}
