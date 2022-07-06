using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIStart : MonoBehaviour
{
    public void UIButtonStart()            //开始菜单
    {
        
    }

    public void UIButtonSetting()          //设置菜单
    {

    }

    public void StartGame()                //开始游戏
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void UIButtonQuit()             //退出游戏
    {
        Debug.Log("Game is Quit!");
        Application.Quit();
    }

}
