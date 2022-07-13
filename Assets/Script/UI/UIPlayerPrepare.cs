using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class UIPlayerPrepare : MonoBehaviour
    {
        // Start is called before the first frame update
        public Image playerImage;
        public Image checkImage;
        public Text playerName;
        public Sprite onSprite;
        public Sprite offSprite;
        public bool ready = false;
        private Player.PlayerAttribute target;
        void Awake()
        {
            this.checkImage.sprite = ready ? onSprite : offSprite;
        }

        // Update is called once per frame
        void Update()
        {
            if (target == null)
                return;
            if (target.frameInput.Jump)
            {
                ready = !ready;
                this.checkImage.sprite = ready ? onSprite : offSprite;
            }

        }
        public void setPlayer(Player.PlayerAttribute playerAttribute)
        {
            this.target = playerAttribute;
            this.playerImage.sprite = playerAttribute.spriteRenderer.sprite;
            this.playerName.text = playerAttribute.playerName;
            playerAttribute.playerController.enabled = false;
        }
        public void StartGame()
        {
            this.target.playerController.enabled = true;
        }
        /// <summary>
        /// Callback to draw gizmos that are pickable and always drawn.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (this.checkImage && this.onSprite && this.offSprite)
                this.checkImage.sprite = ready ? onSprite : offSprite;
        }
    }
}
