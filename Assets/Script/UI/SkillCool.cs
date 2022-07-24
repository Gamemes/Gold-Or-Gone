using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class SkillCool : MonoBehaviour
    {
        public Image image;
        public Image mask;
        /// <summary>
        /// 需要一个函数返回0-1的小数
        /// </summary>
        public Func<float> processFunc;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (processFunc != null)
            {
                var rect = image.rectTransform.sizeDelta;
                mask.rectTransform.sizeDelta = new Vector2(rect.x * processFunc.Invoke(), rect.y);
            }
        }
    }

}

