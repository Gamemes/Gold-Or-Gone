using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUI;

namespace Player
{
    public class PlayerUIManager : MonoBehaviour
    {
        [Range(0, 1)]
        public int playerIndex = 0;
        public UIPlayerHeadName playerHeadName;
        public UIBlood uIBlood;
        public UIEnergy uIEnergy;
        private void Start()
        {
            playerHeadName = GetComponentInChildren<UIPlayerHeadName>();
            uIBlood = GetComponentInChildren<UIBlood>();
            uIEnergy = GetComponentInChildren<UIEnergy>();
            init(playerIndex);
        }
        public void init(int idx)
        {
            var stage = Manager.StageManager.CurrentStageManager();
            if (stage.stagePlayers.Count <= playerIndex)
            {
                Debug.LogWarning("错误的 playerIndex, 场景玩家数量不足");
                return;
            }
            else
            {
                var player = stage.stagePlayerAttributes[stage.stagePlayers[playerIndex]];
                playerHeadName.init(player);
                uIBlood.init(player);
                uIEnergy.init(player);
            }
        }
    }

}
