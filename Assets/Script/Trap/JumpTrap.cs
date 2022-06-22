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
            Debug.Log($"jump block");
            var playercontroller = playerAttribute.playerController;
            StartCoroutine(jump(playercontroller));
            //playercontroller.speedThisFrame.y += 2 * playercontroller.jumpSpeed;

        }
        IEnumerator jump(Player.PlayerController playerController)
        {
            yield return null;
            if (playerController.colDow)
            {
                Debug.Log($"jump");
                playerController.speedThisFrame.y += 2 * playerController.jumpSpeed;
            }
        }
    }
}
