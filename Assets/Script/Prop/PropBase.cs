using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prop
{
    /// <summary>
    /// 可拾取道具基类.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class PropBase : MonoBehaviour
    {
        protected bool autoDelete = true;
        public virtual void Start()
        {
            Manager.StageManager.CurrentStageManager().onReGame += () => { this.gameObject.SetActive(true); };
        }
        /// <summary>
        /// 玩家走进触发
        /// </summary>
        /// <param name="playerAttribute">走进的玩家属性</param>
        public virtual void onPlayerEnter(Player.PlayerAttribute playerAttribute)
        {

        }
        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"player enter {this.gameObject.name}");
            if (other.tag.CompareTo("Player") == 0)
            {
                var attribute = other.GetComponent<Player.PlayerAttribute>();
                if (attribute == null)
                {
                    var err = new UnityException($"{other.gameObject} has no PlayerAttribute");
                    throw err;
                }
                onPlayerEnter(attribute);
                if (autoDelete)
                {
                    this.gameObject.SetActive(false);
                }
            }
            else if (other.tag.CompareTo("Player Network") == 0)
            {
                if (autoDelete)
                    this.gameObject.SetActive(false);
            }
        }
    }
}
