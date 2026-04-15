using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputSettings;
using Image = UnityEngine.UI.Image;


public class UIControler : MonoBehaviour
{
    //[Header("重要：此脚本需被拖入最开始的场景的空GameObject中，主地图中不能存在")]
    //public GameObject Tips;

    #region 字段

    [Header("拖入对应知识文本框")]
    public TMP_Text infoText;

    [Header("知识触发文本")]
    public string[] bridge1Text = new string[9];
    public string[] bridge2Text = new string[9];
    public string[] bridge3Text = new string[9];

    [Header("知识触发图片")]
    public Sprite[] bridge1Image = new Sprite[9];
    public Sprite[] bridge2Image = new Sprite[9];
    public Sprite[] bridge3Image = new Sprite[9];

    private static AudioSource[] musicList;
    private static int musicNum; // 全部音乐音效的数量
    public static bool isMainMusicOn = true;//控制主音乐是否开启
    public static bool isOtherMusicOn = true; //控制其他音乐是否开启
    public static float MainMusicVolume = 0.5f;//音量大小，和设置里的滑动条绑定
    public static UnityEngine.UI.Slider slider;//设置中的音量大小滑动条



    private string[] bridgePanelNames =
    {
        "Bridge1Panel",
        "Bridge2Panel",
        "Bridge3Panel"
    };

    // 小鱼干收集情况
    public static int CollectScore;
    public static bool[] bridge1fishes = { false, false, false, false, false, false, false, false, false };
    public static bool[] bridge2fishes = { false, false, false, false, false, false, false, false, false };
    public static bool[] bridge3fishes = { false, false, false, false, false, false, false, false, false };




    #endregion

    #region 基本函数
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

    #endregion

    //开始游戏界面“开始游戏”按钮专用函数
    public static void StartGame()
    {
        //SceneManager.LoadScene("合并成功-晋祠");
        SceneFade.Instance.LoadScene("SwitchScene");
    }

    #region 选关/桥界面

    public static void EnterYuZhaoFeiLiang()
    {
        SceneFade.Instance.LoadScene("合并成功-晋祠");
    }

    public static void EnterZhaoZhouQiao()
    {
        //SceneFade.Instance.LoadScene("合并成功-晋祠");

    }

    public static void EnterGuangJiQiao()
    {
        //SceneFade.Instance.LoadScene("合并成功-晋祠");
    }

    #endregion

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
        SceneFade.Instance.LoadScene("StartGame");
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
        for (int i = 0; i < musicNum; i++)
        {
            musicList[i].volume = MainMusicVolume;
        }
        //Debug.Log($"当前音量：{MainMusicVolume}");
    }

    #endregion

    //#region （已废弃）知识界面

    ////打开知识界面
    //public static void KnowledgeOpen()
    //{
    //    // 播放点击知识按钮时的音效，把0换成对应的数字
    //    //musicList[0].Play();
    //    Show(GameObject.Find("KnowledgePanel"));
    //    Hide(GameObject.Find("Knowledge1Panel"));
    //    Hide(GameObject.Find("Knowledge2Panel"));
    //    Hide(GameObject.Find("Knowledge3Panel"));
    //    //Show(GameObject.Find("KnowledgeMainPanel"));
    //    Time.timeScale = 0;
    //}

    ////退出知识界面
    //public static void KnowledgeClose()
    //{
    //    Hide(GameObject.Find("KnowledgePanel"));
    //    Time.timeScale = 1;
    //}
    //#region 三个知识界面
    //// 打开知识1界面
    //public static void Knowledge1Open()
    //{
    //    Hide(GameObject.Find("KnowledgeMainPanel"));
    //    Show(GameObject.Find("Knowledge1Panel"));
    //}

    //// 关闭知识1界面
    //public static void Knowledge1Close()
    //{
    //    Hide(GameObject.Find("Knowledge1Panel"));
    //    Show(GameObject.Find("KnowledgeMainPanel"));
    //    Debug.Log("agsaUh");
    //}

    //// 打开知识2界面
    //public static void Knowledge2Open()
    //{
    //    Hide(GameObject.Find("KnowledgeMainPanel"));
    //    Show(GameObject.Find("Knowledge2Panel"));
    //}

    //// 关闭知识2界面
    //public static void Knowledge2Close()
    //{
    //    Hide(GameObject.Find("Knowledge2Panel"));
    //    Show(GameObject.Find("KnowledgeMainPanel"));
    //}

    //// 打开知识3界面
    //public static void Knowledge3Open()
    //{
    //    Hide(GameObject.Find("KnowledgeMainPanel"));
    //    Show(GameObject.Find("Knowledge3Panel"));
    //}

    //// 关闭知识3界面
    //public static void Knowledge3Close()
    //{
    //    Hide(GameObject.Find("Knowledge3Panel"));
    //    Show(GameObject.Find("KnowledgeMainPanel"));
    //}

    //// 进入三个个小游戏
    //public static void StartGame1()
    //{
    //    SceneFade.Instance.LoadScene("第一个游戏的Scene名字");
    //}

    //public static void StartGame2()
    //{
    //    SceneFade.Instance.LoadScene("第二个游戏的Scene名字");
    //}

    //public static void StartGame3()
    //{
    //    SceneFade.Instance.LoadScene("第三个游戏的Scene名字");
    //}

    //#endregion

    //#endregion

    #region 成就/小鱼干界面

    //打开小鱼干界面
    public static void FishOpen()
    {
        //musicList[0].Play();
        Show(GameObject.Find("FishPanel"));
        Hide(GameObject.Find("Bridge1Panel"));
        Hide(GameObject.Find("Bridge2Panel"));
        Hide(GameObject.Find("Bridge3Panel"));
        Time.timeScale = 0;
    }

    //退出小鱼干界面
    public static void FishClose()
    {
        Hide(GameObject.Find("FishPanel"));
        Time.timeScale = 1;
    }

    public static void Bridge1FishOpen()
    {
        Show(GameObject.Find("Bridge1Panel"));
        Hide(GameObject.Find("Bridge2Panel"));
        Hide(GameObject.Find("Bridge3Panel"));
    }
    
    public static void Bridge2FishOpen()
    {
        Show(GameObject.Find("Bridge2Panel"));
        Hide(GameObject.Find("Bridge1Panel"));
        Hide(GameObject.Find("Bridge3Panel"));
    }
    
    public static void Bridge3FishOpen()
    {
        Show(GameObject.Find("Bridge3Panel"));
        Hide(GameObject.Find("Bridge2Panel"));
        Hide(GameObject.Find("Bridge1Panel"));
    }

    // 自动查找所有桥面板下的 Fish1~Fish9 按钮并注册点击
    public void RegisterAllFishButtons()
    {
        // 获取FishPanel的Transform
        Transform fishPanelTrans = GameObject.Find("FishPanel")?.transform;

        // 如果根节点都找不到，直接退出
        if (fishPanelTrans == null)
        {
            Debug.LogError("未找到名为 FishPanel 的物体！");
            return;
        }

        for (int bridgeIndex = 0; bridgeIndex < 3; bridgeIndex++)
        {
            string panelName = bridgePanelNames[bridgeIndex];
            Transform panelTrans = fishPanelTrans.Find(panelName);

            // 修正点：使用 continue 跳过不存在的面板，而不是终止整个循环
            if (panelTrans == null)
            {
                Debug.LogWarning("未找到面板：" + panelName);
                continue;
            }

            // 遍历 Fish1 到 Fish9
            for (int fishNum = 1; fishNum <= 9; fishNum++)
            {
                Transform btnTrans = panelTrans.Find("Fish" + fishNum);

                // 判空后再获取组件
                if (btnTrans == null) continue;

                UnityEngine.UI.Button btn = btnTrans.GetComponent<UnityEngine.UI.Button>();
                if (btn != null)
                {
                    // 核心修正：正确的订阅事件方式
                    int b = bridgeIndex + 1;
                    int f = fishNum;

                    // 亮暗显示鱼干是否集齐
                    ColorBlock colorBlock = btn.colors;
                    if (b == 1)
                    {
                        if (bridge1fishes[f-1])
                        {
                            colorBlock.normalColor = new Color(r: 1f, g: 1f, b: 1f, a: 1f);
                        }
                        else
                        {
                            colorBlock.normalColor = new Color(r: 0.5f, g: 0.5f, b: 0.5f, a: 1f);
                        }
                    }
                    else if(b == 2)
                    {
                        if (bridge2fishes[f - 1])
                        {
                            colorBlock.normalColor = new Color(r: 1f, g: 1f, b: 1f, a: 1f);
                        }
                        else
                        {
                            colorBlock.normalColor = new Color(r: 0.5f, g: 0.5f, b: 0.5f, a: 1f);
                        }
                    }
                    else if(b == 3)
                    {
                        if (bridge3fishes[f - 1])
                        {
                            colorBlock.normalColor = new Color(r: 1f, g: 1f, b: 1f, a: 1f);
                        }
                        else
                        {
                            colorBlock.normalColor = new Color(r: 0.5f, g: 0.5f, b: 0.5f, a: 1f);
                        }
                    }
                    btn.colors = colorBlock;

                    // 修正点：必须是 onClick.AddListener
                    btn.onClick.AddListener(() =>
                    {
                        OnFishButtonClicked(b, f);
                    });
                }
            }
        }
    }

    // 点击回调
    public void OnFishButtonClicked(int bridgeNo, int fishNo)
    {
        // 这里可以自定义显示文本
        string text = $"Bridge {bridgeNo} - Fish {fishNo} 被点击";
        infoText.text = text;

        // 你也可以在这里加逻辑：
        // if(bridgeNo == 1 && fishNo == 3) { ... }
    }

    void OnDestroy()
    {
        // 可选：清理监听（更安全）
        //UnRegisterAllFishButtons();
    }

    void UnRegisterAllFishButtons()
    {
        for (int bridgeIndex = 0; bridgeIndex < 3; bridgeIndex++)
        {
            string panelName = bridgePanelNames[bridgeIndex];
            Transform panelTrans = FindObjectOfType<Canvas>().transform.Find(panelName);
            if (panelTrans == null) continue;

            for (int fishNum = 1; fishNum <= 9; fishNum++)
            {
                Transform btnTrans = panelTrans.Find("Fish" + fishNum);
                if (btnTrans == null) continue;

                UnityEngine.UI.Button btn = btnTrans.GetComponent<UnityEngine.UI.Button>();
                if (btn != null)
                {
                    int b = bridgeIndex + 1;
                    int f = fishNum;
                    btn.onClick.RemoveAllListeners();
                }
            }
        }
    }

    #endregion

    #region 触发界面

    public void TriggerOpen(int bridgeNum, int fishNum)
    {
        Show(GameObject.Find("TriggersPanel"));
        Image triggerImage = GameObject.Find("TriggersPanel")?.transform.Find("KnowledgeImage").GetComponent<Image>();
        //Debug.Log($"{bridgeNum}, {fishNum}");
        Debug.Log($"bridge1Image[fishNum - 1] == null is {bridge1Image[fishNum - 1] == null}");
        //Debug.Log($"triggerImage == null is {triggerImage == null}");
        switch(bridgeNum)
        {
            case 1:
                triggerImage.sprite = bridge1Image[fishNum - 1];
                break;
            case 2:
                triggerImage.sprite = bridge2Image[fishNum - 1];
                break;
            case 3:
                triggerImage.sprite = bridge3Image[fishNum - 1];
                break;
        }
        Time.timeScale = 0;
    }

    public static void TriggerClose()
    {
        Hide(GameObject.Find("TriggersPanel"));
        Time.timeScale = 1;
    }

    #endregion

    #region 游戏前界面
    
    public static void YesGame()
    {
        Hide(GameObject.Find("EnterGamePanel"));
        //SceneFade.Instance.LoadScene("SwitchScene");
    }
    
    public static void NoGame()
    {
        SceneFade.Instance.LoadScene("SwitchScene");
    }



    #endregion

    void Start()
    {
        //前两行单独测试时注释掉
        //musicList = FindObjectOfType<MusicControler>().musicList;
        //musicNum = musicList.Length;

        //Debug.Log($"isFirstEnterGame={isFirstEnterGame}");
        // 隐藏对话UI
        //if(gameObject.scene.name == "MainGame" && !isFirstEnterGame)

        // 隐藏设置、知识和成就界面
        if (GameObject.Find("SettingPanel") != null)
        {
            SettingClose();
        }
        //if (GameObject.Find("KnowledgePanel") != null)
        //{
        //    KnowledgeClose();
        //}
        if (GameObject.Find("FishPanel") != null)
        {
            FishClose();
        }
        if (GameObject.Find("TriggersPanel") != null)
        {
            TriggerClose();
        }
        if(GameObject.Find("EnterGamePanel") != null)
        {
            Hide(GameObject.Find("EnterGamePanel"));
        }
        if (gameObject.scene.name == "SwitchScene" || gameObject.scene.name == "合并成功-晋祠")
        {
            RegisterAllFishButtons();
        }
        if (gameObject.scene.name == "Fish" || gameObject.scene.name == "Puzzle" || gameObject.scene.name == "Question")
        {

        }
    }

}