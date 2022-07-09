using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameUI
{
    public class UIPlayerHeadName : MonoBehaviour
    {
        // Start is called before the first frame update
        public TextMeshProUGUI textMesh;
        void Start()
        {
            var playerAttribute = GetComponentInParent<Player.PlayerUIManager>().targetPlayer;
            Debug.Assert(playerAttribute != null);
            textMesh = GetComponent<TextMeshProUGUI>();
            Debug.Assert(textMesh != null);
            Debug.Assert(playerAttribute.playerName != null);
            textMesh.SetText(playerAttribute.playerName);
        }
    }
}
