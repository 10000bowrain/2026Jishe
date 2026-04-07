using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputSettings;

public class ThreeUIControler : MonoBehaviour
{

    #region 通用函数
    //隐藏ui，在弹出设置面板，知识面板时用到
    private static void Hide(GameObject target)
    {
        target.GetComponent<CanvasGroup>().alpha = 0;
        target.GetComponent<CanvasGroup>().interactable = false;
        target.GetComponent<CanvasGroup>().blocksRaycasts = false;
        //target.SetActive(false);
    }

    //显示ui，在弹出设置面板，知识面板时用到
    private static void Show(GameObject target)
    {
        target.GetComponent<CanvasGroup>().alpha = 1;
        target.GetComponent<CanvasGroup>().interactable = true;
        target.GetComponent<CanvasGroup>().blocksRaycasts = true;
        //target.SetActive(true);
        //target.enabled = true;
    }

    public static void HideAll()
    {
        if (GameObject.Find("SettingPanel") != null)
        {
            SettingClose();
        }
        if (GameObject.Find("KnowledgePanel") != null)
        {
            KnowledgeClose();
        }
        if (GameObject.Find("FishPanel") != null)
        {
            FishClose();
        }
    }

    //public static void ShowAll()
    //{
    //    if (GameObject.Find("SettingPanel") != null)
    //    {
    //        SettingClose();
    //    }
    //    if (GameObject.Find("KnowledgePanel") != null)
    //    {
    //        knowledgeClose();
    //    }
    //    if (GameObject.Find("FishPanel") != null)
    //    {
    //        FishClose();
    //    }
    //}

    //退出游戏时调用的函数

    #endregion

    public static void ExitGame()
    {
        Application.Quit();
    }

    //开始游戏界面“开始游戏”按钮专用函数
    public static void StartGame()
    {
        //SceneManager.LoadScene("EndAnimation");
    }

    #region 设置界面

    //打开设置界面
    public static void SettingOpen()
    {
        Show(GameObject.Find("SettingPanel"));
        Time.timeScale = 0;
    }

    //退出设置界面
    public static void SettingClose()
    {
        Hide(GameObject.Find("SettingPanel"));
        Time.timeScale = 1;
    }
    #endregion

    #region 知识界面

    //打开知识界面
    public static void KnowledgeOpen()
    {
        Show(GameObject.Find("KnowledgePanel"));
        Hide(GameObject.Find("Knowledge1Panel"));
        Hide(GameObject.Find("Knowledge2Panel"));
        Hide(GameObject.Find("Knowledge3Panel"));
        //Show(GameObject.Find("KnowledgeMainPanel"));
        Time.timeScale = 0;
    }

    //退出知识界面
    public static void KnowledgeClose()
    {
        Hide(GameObject.Find("KnowledgePanel"));
        Time.timeScale = 1;
    }
    #region 三个知识界面
    // 打开知识1界面
    public static void Knowledge1Open()
    {
        Hide(GameObject.Find("KnowledgeMainPanel"));
        Show(GameObject.Find("Knowledge1Panel"));
    }

    // 关闭知识1界面
    public static void Knowledge1Close()
    {
        Hide(GameObject.Find("Knowledge1Panel"));
        Show(GameObject.Find("KnowledgeMainPanel"));
        Debug.Log("agsaUh");
    }

    // 打开知识2界面
    public static void Knowledge2Open()
    {
        Hide(GameObject.Find("KnowledgeMainPanel"));
        Show(GameObject.Find("Knowledge2Panel"));
    }

    // 关闭知识2界面
    public static void Knowledge2Close()
    {
        Hide(GameObject.Find("Knowledge2Panel"));
        Show(GameObject.Find("KnowledgeMainPanel"));
    }

    // 打开知识3界面
    public static void Knowledge3Open()
    {
        Hide(GameObject.Find("KnowledgeMainPanel"));
        Show(GameObject.Find("Knowledge3Panel"));
    }

    // 关闭知识3界面
    public static void Knowledge3Close()
    {
        Hide(GameObject.Find("Knowledge3Panel"));
        Show(GameObject.Find("KnowledgeMainPanel"));
    }

    public static void StartGame1()
    {

    }

    public static void StartGame2()
    {

    }

    public static void StartGame3()
    {

    }

    #endregion
    
    #endregion

    //打开小鱼干界面
    public static void FishOpen()
    {
        Show(GameObject.Find("FishPanel"));
        Time.timeScale = 0;
    }

    //退出小鱼干界面
    public static void FishClose()
    {
        Hide(GameObject.Find("FishPanel"));
        Time.timeScale = 1;
    }



    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("SettingPanel") != null)
        {
            SettingClose();
        }
        if (GameObject.Find("KnowledgePanel") != null)
        {
            KnowledgeClose();
        }
        if (GameObject.Find("FishPanel") != null)
        {
            FishClose();
        }
    }

}
