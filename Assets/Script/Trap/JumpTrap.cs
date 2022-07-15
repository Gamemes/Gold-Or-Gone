using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Trap
{
    public class JumpTrap : TrapBase
    {
        public override void onPlayerEnter(PlayerAttribute playerAttribute)
        {
            Debug.Log($"jump block enter");
            var playercontroller = playerAttribute.playerController;
            StartCoroutine(jump(playercontroller));
        }
        IEnumerator jump(Player.PlayerController playerController)
        {
            yield return null;
            Debug.Log($"jump block");
            if (playerController.colDow)
            {
                playerController.speedThisFrame.y += 2 * playerController.jumpSpeed;
            }
        }
    }
}
