using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameUI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UIPlayerHeadName : MonoBehaviour
    {
        // Start is called before the first frame update
        public TextMeshProUGUI textMesh;
        public bool initd = false;
        void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }
        public void init(Player.PlayerAttribute playerAttribute)
        {
            if (playerAttribute == null)
                return;
            textMesh.SetText(playerAttribute.playerName);
            initd = true;
        }

    }
}
