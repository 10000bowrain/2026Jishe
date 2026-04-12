using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputSettings;


public class UIControler : MonoBehaviour
{
    //[Header("重要：此脚本需被拖入最开始的场景的空GameObject中，主地图中不能存在")]
    //public GameObject Tips;

    #region 音乐音效相关字段声明

    private static AudioSource[] musicList;
    private static int musicNum; // 全部音乐音效的数量
    public static bool isMainMusicOn = true;//控制主音乐是否开启
    public static bool isOtherMusicOn = true; //控制其他音乐是否开启
    public static float MainMusicVolume = 0.5f;//音量大小，和设置里的滑动条绑定
    public static UnityEngine.UI.Slider slider;//设置中的音量大小滑动条

    #endregion

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




    //开始游戏界面“开始游戏”按钮专用函数
    public static void StartGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    #region 设置界面

    //打开设置界面
    public static void SettingOpen()
    {
        //musicList[0].Play();
        Show(GameObject.Find("SettingPanel"));
        Time.timeScale = 0;
    }

    //退出设置界面
    public static void SettingClose()
    {
        Hide(GameObject.Find("SettingPanel"));
        Time.timeScale = 1;
    }

    // 返回开始界面（主界面）
    public static void BackTOMainMenu()
    {
        SceneManager.LoadScene("StartGame");
    }

    public static void ExitGame()
    {
        Application.Quit();
    }

    // 控制主音乐开关
    public static void SwtichMainMusic()
    {
        isMainMusicOn = !isMainMusicOn;
        if (!isMainMusicOn)
        {
            musicList[0].volume = 0f;
        }
        else
        {
            musicList[0].volume = MainMusicVolume;
        }
    }

    // 控制其他音效开关
    public static void SwitchOtherMusic()
    {
        isOtherMusicOn = !isOtherMusicOn;
        if (!isOtherMusicOn)
        {
            for (int i = 0; i < musicNum; i++)
            {
                //if(i != 0)
                //musicList[i].volume = 0f;
            }
        }
        else
        {
            for (int i = 0; i < musicNum; i++)
            {
                //if(i != 0)
                //musicList[i].volume = MainMusicVolume;
            }

        }
    }
    
    // 设置音量大小
    public static void ChangeMusicVolume()
    {
        slider = GameObject.Find("Slider").GetComponent<UnityEngine.UI.Slider>();
        MainMusicVolume = slider.value;
        for (int i=0;i<musicNum;i++)
        {
            musicList[i].volume = MainMusicVolume;
        }
        //Debug.Log($"当前音量：{MainMusicVolume}");
    }

    #endregion

    #region 知识界面

    //打开知识界面
    public static void KnowledgeOpen()
    {
        // 播放点击知识按钮时的音效，把0换成对应的数字
        //musicList[0].Play();
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

    // 进入三个个小游戏
    public static void StartGame1()
    {
        SceneManager.LoadScene("第一个游戏的Scene名字");
    }

    public static void StartGame2()
    {
        SceneManager.LoadScene("第二个游戏的Scene名字");
    }

    public static void StartGame3()
    {
        SceneManager.LoadScene("第三个游戏的Scene名字");
    }

    #endregion

    #endregion

    #region 成就/小鱼干界面

    //打开小鱼干界面
    public static void FishOpen()
    {
        //musicList[0].Play();
        Show(GameObject.Find("FishPanel"));
        Time.timeScale = 0;
    }

    //退出小鱼干界面
    public static void FishClose()
    {
        Hide(GameObject.Find("FishPanel"));
        Time.timeScale = 1;
    }

    #endregion

    void Start()
    {
        musicList = FindObjectOfType<MusicControler>().musicList;
        musicNum = musicList.Length;
        //Debug.Log($"isFirstEnterGame={isFirstEnterGame}");
        // 隐藏对话UI
        //if(gameObject.scene.name == "MainGame" && !isFirstEnterGame)

        // 隐藏设置、知识和成就界面
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