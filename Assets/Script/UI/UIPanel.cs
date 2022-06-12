using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI面板脚本
/// </summary>
public class UIPanel : MonoBehaviour
{
    public Image panelImage;
    public AnimationCurve showCurve;            //弹出动画曲线
    public AnimationCurve hideCurve;            //关闭动画曲线
    public float animationSpeed;                //动画速率
    [SerializeField] private bool isShow = false;       //判定是否已经显示UI

    private void Awake()
    {
        panelImage = gameObject.GetComponent<Image>();      //获取图片组件
    }
    IEnumerator ShowPanel()
    {
        float timer = 0;                //初始化计时器
        while (panelImage.color.a < 1)
        {
            panelImage.color = new Vector4(1, 1, 1, showCurve.Evaluate(timer));
            timer += Time.deltaTime * animationSpeed;
            yield return null;
        }
    }

    IEnumerator HidePanel()
    {
        float timer = 0;                //初始化计时器
        while (panelImage.color.a > 0) 
        {
            panelImage.color = new Vector4(1, 1, 1, hideCurve.Evaluate(timer));
            timer += Time.deltaTime * animationSpeed;
            yield return null;
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if(isShow == false)
        //    {
        //        StopAllCoroutines();            //停止当前动画
        //        StartCoroutine(ShowPanel());    //开始弹出动画
        //    }
        //    else if (isShow == true)
        //    {
        //        StopAllCoroutines();            //停止当前动画
        //        StartCoroutine(HidePanel());    //开始关闭动画
        //    }
        //}
    }
}
