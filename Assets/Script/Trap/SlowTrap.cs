using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Trap
{
    /// <summary>
    /// 减速脚本
    /// </summary>
    public class SlowTrap : TrapBase
    {
        [SerializeField] bool isSpeedDown;      //玩家是否已经被减速
        [SerializeField] float speedDownVal;    //行走减速程度（数值越小，减速越强）
        [SerializeField] float jumpDownaVal;    //跳跃减速程度（数值越小，减速越强）

        float thiswalkspeed;                    //进入方块时的速度（恢复速度时候用得到）
        float thisjumpspeed;                    //进入方块时的跳跃速度（恢复速度时候用得到）
        private void Awake()                    //初始化，随便给个值
        {
            isSpeedDown = false;
            if(speedDownVal == 0f)
                speedDownVal = 0.5f;
            if (jumpDownaVal == 0f)
                jumpDownaVal = 0.5f;

            thiswalkspeed = 10f;
            thisjumpspeed = 25f;
        }
        public override void onPlayerEnter(Player.PlayerAttribute playerAttribute)          //进入减速方块时
        {
            thiswalkspeed = playerAttribute.playerController.walkSpeed;             //记录本次进入方块的行走速度
            thisjumpspeed = playerAttribute.playerController.jumpSpeed;             //记录本次进入方块的跳跃速度
            if (isSpeedDown == false)
            {
                isSpeedDown = true;
                Debug.Log("Player has speed down");
                playerAttribute.playerController.walkSpeed = playerAttribute.playerController.walkSpeed * speedDownVal;             //行走减速
                playerAttribute.playerController.jumpSpeed = playerAttribute.playerController.jumpSpeed * jumpDownaVal;             //跳跃减速
            }
        }
        private void OnTriggerExit2D(Collider2D other)              //离开减速方块时
        {

            if(other.tag == "Player")
            {
                var attribute = other.GetComponent<Player.PlayerAttribute>();
                if (attribute == null)
                {
                    var err = new UnityException($"{other.gameObject} has no PlayerAttribute");
                    throw err;
                }

                isSpeedDown = false;
                Debug.Log("Player has recover");
                attribute.playerController.walkSpeed = thiswalkspeed;               //恢复原本行走速度
                attribute.playerController.jumpSpeed = thisjumpspeed;               //恢复原本跳跃速度
            }
        }
    }
}

