using System.Linq;
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
        protected ParticleSystem touchParticle;
        protected bool active = true;
        public virtual void Start()
        {
            Manager.StageManager.CurrentStageManager().onReGame += () =>
            {
                Enable();
            };
            touchParticle = (from par in GetComponentsInChildren<ParticleSystem>() where par.name == "touch" select par).FirstOrDefault();
            if (touchParticle != null)
                Debug.Log($"{gameObject.name} get touchParticle");
        }
        /// <summary>
        /// 玩家走进触发
        /// </summary>
        /// <param name="playerAttribute">走进的玩家属性</param>
        public virtual void onPlayerEnter(Player.PlayerAttribute playerAttribute)
        {

        }
        private void Enable()
        {
            active = true;
            this.gameObject.SetActive(true);
        }
        private void Disable()
        {
            active = false;
            StartCoroutine(Utils.Utils.DelayInvoke(() => { this.gameObject.SetActive(false); }, 1f));
        }
        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!active)
                return;
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
                if (touchParticle != null)
                {
                    touchParticle.Play();
                }
                if (autoDelete)
                {
                    Disable();
                }
            }
            else if (other.tag.CompareTo("Player Network") == 0)
            {
                if (autoDelete)
                    Disable();
            }
        }
    }
}
