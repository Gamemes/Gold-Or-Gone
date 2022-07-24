using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Utils;

namespace GameUI
{
    public class GameEventUI : MonoBehaviour
    {
        public GameObject infoObj;
        public GameObject specialObj;
        public TextMeshProUGUI nameInfo;
        public TextMeshProUGUI detailInfo;
        public TextMeshProUGUI specialInfo;
        private void Start()
        {
            Debug.Assert(detailInfo != null);
            Debug.Assert(specialInfo != null);

        }
        public void ShowDetail(string name, string info, float duration = 10f)
        {
            infoObj.SetActive(true);
            nameInfo.text = name;
            detailInfo.text = info;
            this.DelayInvoke(() => { infoObj.SetActive(false); }, duration);
        }
        public void ShowSpecialInfo(string info)
        {
            if (!specialObj.activeSelf)
                specialObj.SetActive(true);
            specialInfo.text = info;
        }
        public void CloseSpecialInfo()
        {
            specialObj.SetActive(false);
        }
    }
}
