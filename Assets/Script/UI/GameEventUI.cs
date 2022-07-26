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
        public AnimationCurve enterAnimation;
        public float enterDuration = 2f;
        private void Start()
        {
            Debug.Assert(detailInfo != null);
            Debug.Assert(specialInfo != null);

        }
        IEnumerator _EnterAnim(bool reverse = false)
        {
            float time = 0;
            while (time < enterDuration)
            {
                float k = reverse ? enterAnimation.Evaluate(time / enterDuration) : enterAnimation.Evaluate(1f - time / enterDuration);
                this.infoObj.transform.localPosition = new Vector2(0, 144 * k);
                time += Time.deltaTime;
                yield return null;
            }
            if (reverse)
                infoObj.SetActive(false);
            yield break;
        }
        public void ShowDetail(string name, string info, float duration = 10f)
        {
            infoObj.SetActive(true);
            nameInfo.text = name;
            detailInfo.text = info;
            StartCoroutine(_EnterAnim());
            if (duration > 0f)
                this.DelayInvoke(() => { infoObj.SetActive(false); }, duration);
        }
        public void CloseDetail()
        {
            StartCoroutine(_EnterAnim(true));
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
