using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UIStageInfo : MonoBehaviour
{
    private Animator animator;
    private Text infoText;
    public Image backImg;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        infoText = GetComponentInChildren<Text>();
        Debug.Assert(infoText != null);
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void ShowInfo(string info)
    {
        int lines = info.countChar('\n') + 1;
        var size = backImg.rectTransform.sizeDelta;
        backImg.rectTransform.sizeDelta = new Vector2(size.x, Mathf.Max(141, lines * 47));
        infoText.text = info;
        this.animator.Play("showInfo");
    }
}
