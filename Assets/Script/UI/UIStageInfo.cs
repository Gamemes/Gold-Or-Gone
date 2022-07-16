using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStageInfo : MonoBehaviour
{
    private Animator animator;
    private Text infoText;
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
        infoText.text = info;
        this.animator.Play("showInfo");
    }
}
