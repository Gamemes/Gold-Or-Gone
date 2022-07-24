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
        private UIPlayerHeadName playerHeadName;
        private UIBlood uIBlood;
        private UIEnergy uIEnergy;
        public SkillCool sprintSkill;
        public SkillCool rotateSkill;
        public SkillCool gravitySkill;
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
            var player = stage.stagePlayerAttributes[stage.stagePlayers[playerIndex]];
            playerHeadName.init(player);
            uIBlood.init(player);
            uIEnergy.init(player);
            sprintSkill.processFunc = () =>
            {
                return Mathf.Clamp(1.0f - player.playerController.fromLastSprintTime / player.playerController.sprintCoolTime, 0f, 1.0f);
            };
            rotateSkill.processFunc = () =>
            {
                return Mathf.Clamp(1.0f - player.godController.fromLastGravityRotateTime / player.godController.gravityRotateCoolTime, 0f, 1f);
            };
            gravitySkill.processFunc = () =>
            {
                return Mathf.Clamp(1.0f - player.godController.fromLastGrivateChangeTime / player.godController.gravityChangeCoolTime, 0f, 1f);
            };
            player.onPlayerChangeToGod += () =>
            {
                sprintSkill.gameObject.SetActive(false);
                gravitySkill.gameObject.SetActive(true);
                rotateSkill.gameObject.SetActive(true);
            };
            player.onPlayerChangeToHuman += () =>
            {
                sprintSkill.gameObject.SetActive(true);
                gravitySkill.gameObject.SetActive(false);
                rotateSkill.gameObject.SetActive(false);
            };
        }
    }

}
