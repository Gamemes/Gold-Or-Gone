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
        public Image deviceImage;
        public Image barImage;
        public Text playerName;
        public Sprite onSprite;
        public Sprite offSprite;
        public Sprite keybordSprite;
        public Sprite xboxSprite;
        public bool ready = false;
        [HideInInspector] public Player.PlayerAttribute target;
        [HideInInspector] public bool activeBar;
        private bool active = true;
        void Awake()
        {
            this.checkImage.sprite = ready ? onSprite : offSprite;
        }

        // Update is called once per frame
        void Update()
        {
            if (!activeBar && active)
            {
                if (target == null)
                    return;
                if (target.frameInput.specialKey != ready)
                {
                    ready = target.frameInput.specialKey;
                    this.checkImage.sprite = ready ? onSprite : offSprite;
                }
            }
            else if (activeBar)
            {
                if (target.frameInput.Jump)
                {
                    var rect = barImage.rectTransform.sizeDelta;
                    barImage.rectTransform.sizeDelta = new Vector2(rect.x + rect.x * 0.1f + 1, rect.y);
                    rect = barImage.rectTransform.sizeDelta;
                    barImage.color = Color.HSVToRGB(rect.x / 90f, 0.5f, 0.5f);
                    Debug.Log($"{target.playerName} press {barImage.rectTransform.sizeDelta}");
                }
                active = false;
            }
        }
        public void setPlayer(Player.PlayerAttribute playerAttribute)
        {
            this.target = playerAttribute;
            this.playerImage.sprite = playerAttribute.spriteRenderer.sprite;
            this.playerName.text = playerAttribute.playerName;
            playerAttribute.playerController.enabled = false;
            playerAttribute.frameInput.specialKeyFunc = () => { return playerAttribute.frameInput.Jump; };
            if (deviceImage != null)
            {
                deviceImage.sprite = playerAttribute.frameInput.device is UnityEngine.InputSystem.Keyboard ? keybordSprite : xboxSprite;
            }
        }
        private void OnDrawGizmos()
        {
            if (this.checkImage && this.onSprite && this.offSprite)
                this.checkImage.sprite = ready ? onSprite : offSprite;
        }

    }
}
