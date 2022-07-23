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
    private Color _textColor;
    private Color _backColor;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        infoText = GetComponentInChildren<Text>();
        Debug.Assert(infoText != null);
        _textColor = infoText.color;
        _backColor = backImg.color;
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
        infoText.color = _textColor;
        backImg.color = _backColor;
        this.animator.Play("showInfo");
    }
    public void ShowInfo(string info, Color textColor, Color backColor)
    {
        int lines = info.countChar('\n') + 1;
        var size = backImg.rectTransform.sizeDelta;
        backImg.rectTransform.sizeDelta = new Vector2(size.x, Mathf.Max(141, lines * 47));
        infoText.text = info;
        backColor.a = _backColor.a;
        infoText.color = textColor;
        backImg.color = backColor;
        this.animator.Play("showInfo");
    }
}
