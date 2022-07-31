using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

/// <summary>
/// UI面板脚本
/// </summary>
public class UIPanel : MonoBehaviour
{
    public CanvasGroup panelImage;
    public AnimationCurve showCurve;            //弹出动画曲线
    public AnimationCurve hideCurve;            //关闭动画曲线
    public float animationSpeed;                //动画速率
    [SerializeField] private bool isShow = false;       //判定是否已经显示UI
    [SerializeField] public GameObject UIChild;                  //子对象管理
    [SerializeField] public GameObject UIChildBase;                  //子对象管理.base
    [SerializeField] public GameObject UIChildSetting;                  //子对象管理.setting
    private void Awake()
    {
        panelImage = gameObject.GetComponent<CanvasGroup>();      //获取图片组件
        UIChild = GameObject.Find("ESCCanvas/Panel/UI");
        UIChildBase = GameObject.Find("ESCCanvas/Panel/UI/Base");
        UIChildSetting = GameObject.Find("ESCCanvas/Panel/UI/Setting");
        UIChild.SetActive(false);
    }
    IEnumerator ShowPanel()
    {
        float timer = 0;                //初始化计时器
        while (panelImage.alpha < 0.7)
        {
            panelImage.alpha = showCurve.Evaluate(timer);
            timer += Time.deltaTime * animationSpeed;
            yield return null;
        }
    }

    IEnumerator HidePanel()
    {
        float timer = 0;                //初始化计时器
        while (panelImage.alpha > 0)
        {
            panelImage.alpha = hideCurve.Evaluate(timer);
            timer += Time.deltaTime * animationSpeed;
            if (panelImage.alpha == 0)
            {
                UIChild.SetActive(false);
            }
            yield return null;
        }
    }
    public void OnESC(InputAction.CallbackContext context)
    {
        Debug.Log($"test");
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                if (context.interaction is PressInteraction)
                {
                    if (isShow == false)
                    {
                        isShow = true;
                        UIChild.SetActive(true);
                        StopAllCoroutines();            //停止当前动画
                        StartCoroutine(ShowPanel());    //开始弹出动画
                    }
                    else if (isShow == true)
                    {
                        isShow = false;
                        StopAllCoroutines();            //停止当前动画
                        StartCoroutine(HidePanel());    //开始关闭动画
                    }
                    UIChildBase.SetActive(true);
                    UIChildSetting.SetActive(false);
                }
                break;
            case InputActionPhase.Started:
                break;
            case InputActionPhase.Canceled:
                break;
        }
    }

    public void BaseBack()      //返回按钮
    {
        if (isShow == true)
        {
            StopAllCoroutines();            //停止当前动画
            StartCoroutine(HidePanel());    //开始关闭动画
            isShow = false;
        }
    }

    public void Setting()       //设置按钮
    {
        UIChildBase.SetActive(false);
        UIChildSetting.SetActive(true);
    }

    public void SettingBack()   //设置里的返回按钮
    {
        UIChildBase.SetActive(true);
        UIChildSetting.SetActive(false);
    }

    public void MainMenu()      //退出按钮
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

}
