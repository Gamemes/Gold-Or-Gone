using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class TacitUI : MonoBehaviour
    {
        private GameEvent.GameEventManager gameEventManager => Manager.StageManager.CurrentStageManager().gameEventManager;
        public int startShowVal = 40;
        public Image barImg;
        private Vector2 _rect;
        private void Start()
        {
            _rect = barImg.rectTransform.sizeDelta;
            gameEventManager.onTacitValueChange += (val) =>
            {
                showVal(val);
            };
            showVal(gameEventManager.tacitValue);
        }
        public void showVal(int val)
        {
            if (val < startShowVal)
            {
                barImg.gameObject.SetActive(false);
            }
            else
            {
                barImg.gameObject.SetActive(true);
                barImg.rectTransform.sizeDelta = new Vector2(_rect.x * val / 100f, _rect.y);
            }

        }
    }
}

