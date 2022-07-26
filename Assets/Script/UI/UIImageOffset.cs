using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class UIImageOffset : MonoBehaviour
    {
        public Image image;
        public Vector2 offsetPre;
        private Vector2 offset = Vector2.zero;
        private void Update()
        {
            if (image == null)
                return;
            offset += offsetPre * Time.deltaTime;
            image.material.SetTextureOffset("_MainTex", offset);
        }
    }
}
