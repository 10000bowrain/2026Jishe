using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using System.Data;
using System.Runtime.CompilerServices;


public class UIControler : MonoBehaviour
{
    // 单例（所有调用GameObject.Find("UIControler")的地方改为 UIControler.Instance）
    public static UIControler Instance { get; private set; }

    #region 字段

    [Header("拖入对应知识文本框")]
    public TMP_Text infoText;

    [Header("知识触发文本：填前9个")]
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
    public static float MainMusicVolume = MusicControler.DefaultVolume;//音量大小，和设置里的滑动条绑定
    public static UnityEngine.UI.Slider slider;//设置中的音量大小滑动条

    public static readonly string[] ThreeSceneName = { "", "1-晋祠鱼沼飞梁", "2-赵州桥", "3-广济桥" };

    private static string[] bridgePanelNames =
    {
        "Bridge1Panel",
        "Bridge2Panel",
        "Bridge3Panel"
    };

    // 小鱼干收集情况
    public static int fish1count = 0;
    public static int fish2count = 0;
    public static int fish3count = 0;
    public static int fishallcount = 0;
    public static bool[] bridge1fishes = { false, false, false, false, false, false, false, false, false };
    public static bool[] bridge2fishes = { false, false, false, false, false, false, false, false, false };
    public static bool[] bridge3fishes = { false, false, false, false, false, false, false, false, false };

    public static bool isBridge1Entered = false;
    public static bool isBridge2Entered = false;

    // 控制漫画开头是否已经播放过（只播一次）
    // 控制漫画开头是否已经播放过（只播一次）
    public static bool isCartoonPlayed = false;

    // 小游戏返回位置记录
    public static string returnSceneName = "";
    public static Vector3 returnPlayerPosition = Vector3.zero;

    // 小游戏鱼干pending追踪（进入小游戏前记录，通关/退出时判断是否给鱼干）
    public static int pendingFishBridge = 2;  // 哪个桥（1=晋祠，2=赵州桥，3=广济桥）
    public static int pendingFishIndex = -1;  // 哪个鱼干（0-8），-1表示无pending
    public static bool pendingFishAwarded = false; // 该pending鱼干是否已发放（防止重复）

    // 鱼干进度条（类似血条，从下往上填满）
    private static GameObject fishProgressBarObj;
    private static Image fishProgressFill;

    // 设置界面来源（用于判断是否显示"返回选关界面"按钮）
    public static bool isFromSwitchScene = false;


    #endregion

    #region 基本函数

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // 【关键修复】不要直接销毁新的UIControler，而是让它"接过"实例
            // 这样每个场景的UIControler都能正确初始化自己的场景数据
            // 销毁旧的持久化UIControler
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    //隐藏ui，在弹出设置面板，知识面板时用到
    public static void Hide(GameObject target)
    {
        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    //显示ui，在弹出设置面板，知识面板时用到
    public static void Show(GameObject target)
    {
        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    #endregion

    #region 鱼干进度条（类似血条）

    // 创建鱼干进度条UI（定位在屏幕右上角，与FishButton同区域显示）
    public static void CreateFishProgressBar()
    {
        // 关键：如果进度条已存在，检查它是否在当前活跃Canvas上
        // 防止旧场景残留的进度条在错误Canvas上导致不可见
        if (fishProgressBarObj != null)
        {
            Canvas currentCanvas = FindObjectOfType<Canvas>();
            if (currentCanvas != null && fishProgressBarObj.transform.parent == currentCanvas.transform)
            {
                // 进度条在当前活跃Canvas上，无需重建
                Debug.Log("[鱼干进度条] 已存在且在正确Canvas上，不重建");
                return;
            }
            else
            {
                // 进度条在错误的Canvas上，销毁重建
                Debug.Log("[鱼干进度条] 在错误Canvas上，销毁重建");
                Destroy(fishProgressBarObj);
                fishProgressBarObj = null;
                fishProgressFill = null;
            }
        }

        // 找到游戏场景的 Canvas（必须包含 FishButton）
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[鱼干进度条] 找不到任何Canvas！");
            return;
        }

        // 进一步确认：找包含FishButton的Canvas
        GameObject fishBtn = GameObject.Find("FishButton");
        if (fishBtn == null)
        {
            Debug.LogWarning("[鱼干进度条] 未找到FishButton，跳过创建（当前场景可能没有）");
            return;
        }

        // FishButton 的父级就是游戏场景的 Canvas
        Canvas gameCanvas = fishBtn.transform.parent.GetComponentInParent<Canvas>();
        if (gameCanvas != null)
        {
            canvas = gameCanvas;
            Debug.Log($"[鱼干进度条] 找到游戏Canvas: {canvas.name}");
        }
        else
        {
            Debug.LogWarning("[鱼干进度条] FishButton不在任何Canvas下，使用默认Canvas");
        }

        // 创建进度条容器（作为Canvas子物体）
        fishProgressBarObj = new GameObject("FishProgressBar");
        fishProgressBarObj.transform.SetParent(canvas.transform);

        RectTransform rect = fishProgressBarObj.AddComponent<RectTransform>();
        // 定位在屏幕右上角（0.13f, 0.9f），与FishButton同区域显示
        rect.anchorMin = new Vector2(0.13f, 0.88f);
        rect.anchorMax = new Vector2(0.13f, 0.88f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(16, 120);

        // 背景（深色半透明）
        GameObject bgObj = new GameObject("ProgressBG");
        bgObj.transform.SetParent(fishProgressBarObj.transform);
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.pivot = new Vector2(0.5f, 0.5f);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0.3f, 0.2f, 0.1f, 0.6f); // 深棕色背景
        bgImg.fillCenter = true;

        // 填充（从下往上，暖粉色/肉色）
        GameObject fillObj = new GameObject("ProgressFill");
        fillObj.transform.SetParent(fishProgressBarObj.transform);
        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0, 0);
        fillRect.anchorMax = new Vector2(1, 1);
        fillRect.sizeDelta = new Vector2(-4, -4); // 留边距
        fillRect.pivot = new Vector2(0.5f, 0.5f);
        fillRect.anchoredPosition = Vector2.zero;
        fishProgressFill = fillObj.AddComponent<Image>();
        fishProgressFill.type = UnityEngine.UI.Image.Type.Filled;
        fishProgressFill.fillMethod = UnityEngine.UI.Image.FillMethod.Vertical;
        fishProgressFill.fillOrigin = (int)UnityEngine.UI.Image.OriginVertical.Bottom;
        fishProgressFill.fillAmount = 0f;
        fishProgressFill.color = new Color(1f, 0.7f, 0.6f, 1f); // 固定1.0透明度，fillAmount控制填充

        Debug.Log("[鱼干进度条] 创建成功");
    }

    // 更新鱼干进度条（根据当前场景鱼干数）
    public static void UpdateFishProgressBar()
    {
        if (fishProgressBarObj == null || fishProgressFill == null) return;

        // 使用Instance单例获取场景名
        if (Instance == null) return;
        string sceneName = Instance.gameObject.scene.name;
        int currentCount = 0;
        int totalCount = 9;

        if (sceneName == ThreeSceneName[1])
        {
            currentCount = fish1count;
        }
        else if (sceneName == ThreeSceneName[2])
        {
            currentCount = fish2count;
        }
        else if (sceneName == ThreeSceneName[3])
        {
            currentCount = fish3count;
        }
        else if (sceneName == "SwitchScene选关界面")
        {
            currentCount = fishallcount;
            totalCount = 27;
        }
        else
        {
            return; // 非游戏场景不显示
        }

        float ratio = Mathf.Clamp01((float)currentCount / totalCount);
        fishProgressFill.fillAmount = ratio;
        // fillAmount 从 0→1 就是从下往上填充（FillMethod.Vertical + OriginVertical.Bottom）
        // 透明度固定为 1.0，不额外做 alpha 混合
    }

    // 隐藏鱼干进度条
    public static void HideFishProgressBar()
    {
        if (fishProgressBarObj != null)
            fishProgressBarObj.SetActive(false);
    }

    #endregion

    #region 开始界面函数
    //开始游戏界面"开始游戏"按钮专用函数
    public static void StartGame()
    {
        EnsureSceneFadeExists();
        if (!isCartoonPlayed)
        {
            // 第一次：播放漫画开头
            isCartoonPlayed = true;
            SceneTransition.Instance.LoadScene("Cartoon1Scene漫画1");
        }
        else
        {
            // 之后：直接进选关界面，同时预加载晋祠
            SceneTransition.Instance.PreloadJinCiScene();
            SceneTransition.Instance.LoadScene("SwitchScene选关界面");
        }
    }
    // 开始界面退出游戏函数
    public static void ExitGame()
    {
        Application.Quit();
    }

    public static void IntroductionOpen()
    {
        Show(GameObject.Find("GameIntroductionPanel"));
    }

    public static void IntroductionClose()
    {
        Hide(GameObject.Find("GameIntroductionPanel"));
    }

    #endregion

    // 确保SceneTransition实例存在（防止Cartoon等场景没有SceneTransition）
    private static void EnsureSceneFadeExists()
    {
        if (SceneTransition.Instance == null)
        {
            GameObject sf = new GameObject("SceneTransition");
            sf.AddComponent<SceneTransition>();
        }
    }

    // 选关界面"重看剧情"按钮调用这个
    public static void ReplayCartoon()
    {
        EnsureSceneFadeExists();
        SceneTransition.Instance.LoadScene("Cartoon1Scene漫画1");
    }

    // 鱼干收集计数
    public static void FishCounter()
    {
        fish1count = fish2count = fish3count = 0;
        foreach (bool val in bridge1fishes)
        {
            fish1count += val ? 1 : 0;
        }
        foreach (bool val in bridge2fishes)
        {
            fish2count += val ? 1 : 0;
        }
        foreach (bool val in bridge3fishes)
        {
            fish3count += val ? 1 : 0;
        }
        fishallcount = fish1count + fish2count + fish3count;
        // Debug.Log("fishCount = " + fishallcount);

        // 改为从当前活动场景获取名称，避免 GameObject.Find 找不到对象的问题
        string nowSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        GameObject fishBtn = GameObject.Find("FishButton");
        if (fishBtn == null) return;

        var btnComp = fishBtn.GetComponent<UnityEngine.UI.Button>();
        if (btnComp == null) return;
        var tmpText = btnComp.GetComponentInChildren<TMP_Text>();
        if (tmpText == null) return;

        if (nowSceneName == "SwitchScene选关界面")
        {
            Debug.Log("调用");
            tmpText.text = $"{fishallcount}/27";
        }
        else if (nowSceneName == ThreeSceneName[1])
        {
            tmpText.text = $"{fish1count}/9";
        }
        else if (nowSceneName == ThreeSceneName[2])
        {
            tmpText.text = $"{fish2count}/9";
        }
        else if (nowSceneName == ThreeSceneName[3])
        {
            tmpText.text = $"{fish3count}/9";
        }
        tmpText.ForceMeshUpdate();

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
                if (btnTrans == null)
                    continue;
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
                        if (bridge1fishes[f - 1])
                        {
                            colorBlock.normalColor = new Color(r: 1f, g: 1f, b: 1f, a: 1f);
                        }
                        else
                        {
                            colorBlock.normalColor = new Color(r: 0.5f, g: 0.5f, b: 0.5f, a: 1f);
                        }
                    }
                    else if (b == 2)
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
                    else if (b == 3)
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
                }
            }
        }

        //Debug.Log(GameObject.Find("FishButton").transform.GetComponent<UnityEngine.UI.Button>().GetComponentInChildren<TMP_Text>().text);

        // 同时更新鱼干进度条
        UpdateFishProgressBar();

    }

    #region 选关/桥界面

    public static void SwitchBGOpen()
    {
        Show(GameObject.Find("SwitchBGPanel"));
    }

    public static void SwitchBGClose()
    {
        Hide(GameObject.Find("SwitchBGPanel"));
    }

    public static void SwitchBridgeOpen()
    {
        GameObject panel = GameObject.Find("BasePanel");
        if (panel != null)
        {
            Show(panel);
            // 安全地清空提示文字（避免空引用）
            Transform tip = panel.transform.Find("SwitchBridgeTipText");
            if (tip != null)
            {
                TMP_Text tmp = tip.GetComponent<TMP_Text>();
                if (tmp != null) tmp.text = "";
            }

            // 安全地获取按钮颜色块
            Transform btn2T = panel.transform.Find("ZhaoZhouQiaoButton");
            Transform btn3T = panel.transform.Find("GuangJiQiaoButton");
            if (btn2T != null && btn3T != null)
            {
                UnityEngine.UI.Button btn2 = btn2T.GetComponent<UnityEngine.UI.Button>();
                UnityEngine.UI.Button btn3 = btn3T.GetComponent<UnityEngine.UI.Button>();
                if (btn2 != null && btn3 != null)
                {
                    ColorBlock colorBlock2 = btn2.colors;
                    ColorBlock colorBlock3 = btn3.colors;
                }
            }
        }

        FishCounter();
        Time.timeScale = 0;
    }

    public static void SwichBridgeClose()
    {
        Hide(GameObject.Find("SwitchBGPanel"));
        Time.timeScale = 1;
    }

    public static void EnterYuZhaoFeiLiang()
    {
        // 防止快速点击导致多次触发场景切换
        if (SceneTransition.Instance != null && SceneTransition.Instance.IsTransitioning()) return;

        isBridge1Entered = true;
        //LoaddingOpen();

        //// 预加载晋祠场景
        //if (SceneTransition.Instance != null)
        //    SceneTransition.Instance.PreloadJinCiScene();

        // 切换到晋祠场景
        SceneTransition.Instance.LoadScene(ThreeSceneName[1]);
    }

    public static void EnterZhaoZhouQiao()
    {
        // 防止快速点击导致多次触发场景切换
        //if (SceneTransition.Instance != null && SceneTransition.Instance.IsTransitioning()) return;
        //if (!isBridge1Entered)
        //{
        //    ShowSwitchBridgeToast("游览前一关解锁");
        //    return;
        //}
        //isBridge2Entered = true;
        ////LoaddingOpen();
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(ThreeSceneName[2]);
    }

    public static void EnterGuangJiQiao()
    {
        // 防止快速点击导致多次触发场景切换
        if (SceneTransition.Instance != null && SceneTransition.Instance.IsTransitioning()) return;

        if (!isBridge2Entered)
        {
            ShowSwitchBridgeToast("游览前一关解锁");
            return;
        }
        //LoaddingOpen();
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(ThreeSceneName[3]);
    }

    public static void SwitchSceneSettingClose()
    {
        SettingClose();
        SwitchBridgeOpen();
    }

    public static void SwitchSceneFishClose()
    {
        FishClose();
        SwitchBridgeOpen();
    }

    public static void BridgeNameOpen()
    {
        Show(GameObject.Find("BridgeNamePanel"));
    }

    public static void BridgeNameClose()
    {
        Hide(GameObject.Find("BridgeNamePanel"));
    }

    /// <summary>
    /// 弹出提示文字（弹入→停留→掉落消失的动效）
    /// </summary>
    public static void ShowSwitchBridgeToast(string message)
    {
        if (Instance != null)
            Instance.PlayToastAnimation(message);
    }

    /// <summary>
    /// Toast动画协程：弹入 → 停留1.5秒 → 向下掉落+淡出消失
    /// 协程期间禁用触发按钮，防止重复触发
    /// </summary>
    private void PlayToastAnimation(string message)
    {
        // 保留Toast动画，但不禁用按钮（允许重复点击）
        StartCoroutine(_PlayToastAnimation(message));
    }

    /// <summary>
    /// 禁用选关按钮（动画播放期间）
    /// </summary>
    private void DisableBridgeButtons()
    {
        GameObject panel = GameObject.Find("BasePanel");
        if (panel == null) return;
        Transform zhaoBtn = panel.transform.Find("ZhaoZhouQiaoButton");
        Transform guangBtn = panel.transform.Find("GuangJiQiaoButton");
        if (zhaoBtn != null) zhaoBtn.gameObject.SetActive(false);
        if (guangBtn != null) guangBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// 动画结束后重新启用选关按钮
    /// </summary>
    private void EnableBridgeButtons()
    {
        GameObject panel = GameObject.Find("BasePanel");
        if (panel == null) return;
        Transform zhaoBtn = panel.transform.Find("ZhaoZhouQiaoButton");
        Transform guangBtn = panel.transform.Find("GuangJiQiaoButton");
        if (zhaoBtn != null) zhaoBtn.gameObject.SetActive(true);
        if (guangBtn != null) guangBtn.gameObject.SetActive(true);
    }

    private IEnumerator _PlayToastAnimation(string message)
    {
        GameObject tipObj = GameObject.Find("SwitchBridgeTipText");
        if (tipObj == null)
        {
            yield break;
        }

        RectTransform rect = tipObj.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = tipObj.GetComponent<CanvasGroup>();
        if (canvasGroup == null) 
            canvasGroup = tipObj.AddComponent<CanvasGroup>();

        // 强制激活（确保SetActive(true)不会被后续干扰）
        if (!tipObj.activeSelf)
            tipObj.SetActive(true);

        // 初始状态：隐藏在上方，透明
        Vector2 startPos = new Vector2(0, 280);
        Vector2 normalPos = new Vector2(0, 190);
        rect.anchoredPosition = startPos;
        canvasGroup.alpha = 0f;
        rect.localScale = Vector3.one;

        // 第一段：弹入（0.25秒，弹性缩放）
        float popDur = 0.25f;
        float t = 0f;
        while (t < popDur)
        {
            t += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(t / popDur);
            float scale = progress < 0.6f
                ? progress / 0.6f * 1.15f
                : 1.15f + (1f - 1.15f) * (progress - 0.6f) / 0.4f;
            scale = Mathf.Clamp(scale, 0f, 1.15f);

            rect.localScale = new Vector3(scale, scale, 1f);
            canvasGroup.alpha = Mathf.Clamp01(progress * 2f);
            rect.anchoredPosition = Vector2.Lerp(startPos, normalPos, Mathf.Clamp01(progress * 1.5f));
            yield return null;
        }
        rect.localScale = Vector3.one;
        rect.anchoredPosition = normalPos;
        canvasGroup.alpha = 1f;

        // 第二段：停留1.5秒
        yield return new WaitForSecondsRealtime(1.5f);

        // 第三段：掉落+淡出（0.4秒）
        float dropDur = 0.4f;
        float t2 = 0f;
        while (t2 < dropDur)
        {
            t2 += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(t2 / dropDur);
            float eased = 1f - Mathf.Pow(1f - progress, 2f); // ease-out
            rect.anchoredPosition = Vector2.Lerp(normalPos, new Vector2(0, 80), eased);
            canvasGroup.alpha = 1f - eased;
            yield return null;
        }

        // 结束后隐藏（只设alpha，不SetActive(false)，避免重置组件状态）
        canvasGroup.alpha = 0f;
        rect.anchoredPosition = startPos;
        tipObj.SetActive(false);
    }

    public static void SwitchSceneOpen()
    {
        Show(GameObject.Find("BasePanel"));
        Show(GameObject.Find("SwitchBGPanel"));
        Show(GameObject.Find("BridgeNamePanel"));
        Time.timeScale = 0;
    }


    #endregion

    #region 设置界面

    //打开设置界面（根据来源决定按钮显示）
    public static void SettingOpen()
    {
        string nowSceneName = Instance.gameObject.scene.name;

        //if (GameObject.Find("SwitchBGPanel") != null)
        //{
        //    Hide(GameObject.Find("SwitchBGPanel"));
        //}

        Show(GameObject.Find("SettingPanel"));

        // 获取按钮引用
        GameObject backToMainBtn = GameObject.Find("BackTOMainMenuButton");
        GameObject backToSwitchBtn = GameObject.Find("BackToSwitchButton");

        // 根据来源场景决定按钮显示
        // 选关界面：两个都不显示
        if (nowSceneName == "SwitchScene选关界面")
        {
            if (backToSwitchBtn != null) Hide(backToSwitchBtn);
        }
        // 主界面：两个都不显示（已在主界面没必要返回）
        else if (nowSceneName == "0StartGame主界面")
        {
            if (backToMainBtn != null) Hide(backToMainBtn);
            if (backToSwitchBtn != null) Hide(backToSwitchBtn);
        }
        // 游戏场景（晋祠/赵州桥/广济桥）：两个都显示
        else if (nowSceneName == ThreeSceneName[1] ||
                 nowSceneName == ThreeSceneName[2] ||
                 nowSceneName == ThreeSceneName[3])
        {
        }
        else // 小游戏，都显示
        {

        }
        Time.timeScale = 0;
    }

    //退出设置界面
    public static void SettingClose()
    {
        string nowSceneName = Instance.gameObject.scene.name;
        if (nowSceneName == "SwitchScene选关界面")
        {
            Show(GameObject.Find("SwitchBGPanel"));
            Show(GameObject.Find("BasePanel"));
        }
        Hide(GameObject.Find("SettingPanel"));
        //if(Component.gameObject.scene.name)
        Time.timeScale = 1;
    }

    // 返回开始界面（主界面）
    public static void BackTOMainMenu()
    {
        // 防止快速点击导致多次触发场景切换
        if (SceneTransition.Instance != null && SceneTransition.Instance.IsTransitioning()) return;

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene("0StartGame主界面");
    }

    public static void BackTOSwitchBridge()
    {
        // 防止快速点击导致多次触发场景切换
        if (SceneTransition.Instance != null && SceneTransition.Instance.IsTransitioning()) return;

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene("SwitchScene选关界面");
    }

    // 控制主音乐开关
    public static void SwtichMainMusic()
    {
        isMainMusicOn = !isMainMusicOn;
        if (MusicControler.Instance != null)
        {
            MusicControler.Instance.SetMusicEnabled(isMainMusicOn);
        }
    }

    // 控制其他音效开关
    //public static void SwitchOtherMusic()
    //{
    //    isOtherMusicOn = !isOtherMusicOn;
    //    if (!isOtherMusicOn)
    //    {
    //        for (int i = 4; i < musicNum; i++)
    //        {
    //            //if(i != 0)
    //            //musicList[i].volume = 0f;
    //        }
    //    }
    //    else
    //    {
    //        for (int i = 4; i < musicNum; i++)
    //        {
    //            //if(i != 0)
    //            //musicList[i].volume = MainMusicVolume;
    //        }

    //    }
    //}

    // 设置音量大小
    
    public static void ChangeMusicVolume()
    {
        slider = GameObject.Find("MusicSlider").GetComponent<UnityEngine.UI.Slider>();
        MainMusicVolume = slider.value;
        if (MusicControler.Instance != null)
        {
            MusicControler.Instance.ApplyGlobalVolume(MainMusicVolume);
        }
        Debug.Log("音量已调整为: " + MainMusicVolume);
    }

    #endregion

    #region 等待界面

    //public static void LoaddingOpen()
    //{
    //    // LoaddingPanel只在LoaddingScene中存在，其他场景不处理
    //    // 加载进度条由SceneTransition在渐亮阶段自动创建和显示
    //    Time.timeScale = 1;
    //    // 强制隐藏SwitchScene的所有UI（防止加完场景时旧UI闪现）
    //    GameObject switchPanel = GameObject.Find("BasePanel");
    //    if (switchPanel != null)
    //        switchPanel.SetActive(false);
    //}

    //public static void LoaddingClose()
    //{
    //    // LoaddingPanel只在LoaddingScene中存在，其他场景不处理
    //    Time.timeScale = 1;
    //}

    #endregion

    #region 成就/小鱼干界面

    //打开小鱼干界面
    public static void FishOpen()
    {
        //musicList[0].Play();
        if (GameObject.Find("BasePanel") != null)
        {
            Hide(GameObject.Find("BasePanel"));
        }
        Show(GameObject.Find("FishPanel"));
        Hide(GameObject.Find("Bridge1Panel"));
        Hide(GameObject.Find("Bridge2Panel"));
        Hide(GameObject.Find("Bridge3Panel"));
        Time.timeScale = 0;
    }

    //退出小鱼干界面
    public static void FishClose()
    {
        // 使用Instance单例获取场景名，避免GameObject.Find的不稳定性
        if (Instance == null)
        {
            Debug.LogError("[FishClose] UIControler.Instance 为空！");
            return;
        }
        string nowSceneName = Instance.gameObject.scene.name;

        // 检查是否在游戏场景中（有FishPanel的场景）
        bool isGameScene = (nowSceneName == ThreeSceneName[1] ||
                           nowSceneName == ThreeSceneName[2] ||
                           nowSceneName == ThreeSceneName[3] ||
                           nowSceneName == "SwitchScene选关界面");

        if (isGameScene)
        {
            // 游戏场景：安全操作FishPanel
            GameObject fishPanel = GameObject.Find("FishPanel");
            if (fishPanel != null)
            {
                // 先显示FishPanel（这样子面板才能显示）
                Show(fishPanel);
                // 显示三个桥的子面板
                Show(GameObject.Find("Bridge1Panel"));
                Show(GameObject.Find("Bridge2Panel"));
                Show(GameObject.Find("Bridge3Panel"));
                // 隐藏FishPanel本身
                Hide(fishPanel);
            }
            else
            {
                Debug.LogWarning("[FishClose] 未找到FishPanel！");
            }

            // 如果是从选关界面进的，也要恢复选关面板
            GameObject switchPanel = GameObject.Find("BasePanel");
            if (switchPanel != null && nowSceneName == "SwitchScene选关界面")
            {
                Show(switchPanel);
            }
        }
        // 小游戏场景：什么都不做，防止报错
        Show(GameObject.Find("BasePanel"));
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
                if (btnTrans == null)
                    continue;
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
                        if (bridge1fishes[f - 1])
                        {
                            colorBlock.normalColor = new Color(r: 1f, g: 1f, b: 1f, a: 1f);
                        }
                        else
                        {
                            colorBlock.normalColor = new Color(r: 0.5f, g: 0.5f, b: 0.5f, a: 1f);
                        }
                    }
                    else if (b == 2)
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
                    else if (b == 3)
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
        if (infoText == null)
        {
            Debug.LogWarning("[OnFishButtonClicked] infoText 未绑定！");
            return;
        }

        string text = "";
        switch (bridgeNo)
        {
            case 1:
                if (fishNo >= 1 && fishNo <= bridge1Text.Length && bridge1Text[fishNo - 1] != null)
                    text = bridge1Text[fishNo - 1];
                break;
            case 2:
                if (fishNo >= 1 && fishNo <= bridge2Text.Length && bridge2Text[fishNo - 1] != null)
                    text = bridge2Text[fishNo - 1];
                break;
            case 3:
                if (fishNo >= 1 && fishNo <= bridge3Text.Length && bridge3Text[fishNo - 1] != null)
                    text = bridge3Text[fishNo - 1];
                break;
            default:
                break;
        }
        infoText.text = text;
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

    #region 小游戏启动

    /// <summary>
    /// 启动小游戏（由CatDistanceCheck调用）
    /// 根据游戏类型和桥序号加载对应的独立场景
    /// </summary>
    /// <param name="gameType">游戏类型：0=钓鱼, 1=拼图, 2=问答</param>
    /// <param name="bridgeIndex">桥序号：1=晋祠, 2=赵州桥, 3=广济桥</param>
    public static void StartMiniGame(int gameType, int bridgeIndex)
    {
        string sceneName = "";
        switch (gameType)
        {
            case 0: // 钓鱼
                sceneName = bridgeIndex switch
                {
                    1 => "Fish",
                    2 => "Fish",
                    3 => "Fish",
                    _ => "Fish"
                };
                break;
            case 1: // 拼图
                sceneName = bridgeIndex switch
                {
                    1 => "Puzzle",
                    2 => "Puzzle 1",
                    3 => "Puzzle 2",
                    _ => "Puzzle"
                };
                break;
            case 2: // 问答
                sceneName = bridgeIndex switch
                {
                    1 => "Question",
                    2 => "Question 1",
                    3 => "Question 2",
                    _ => "Question"
                };
                break;
        }

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(sceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 关闭当前小游戏，返回之前的主场景（由各小游戏的退出按钮调用）
    /// </summary>
    public static void CloseMiniGame()
    {
        string targetScene = !string.IsNullOrEmpty(returnSceneName) ? returnSceneName : ThreeSceneName[1];
        returnSceneName = "";
        returnPlayerPosition = Vector3.zero;

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(targetScene);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetScene);
    }

    #endregion

    #region 触发界面

    public void TriggerOpen(int bridgeNum, int fishNum)
    {
        Show(GameObject.Find("TriggersPanel"));
        Image triggerImage = GameObject.Find("TriggersPanel")?.transform.Find("KnowledgeImage").GetComponent<Image>();
        TMP_Text triggerText = GameObject.Find("TriggersPanel")?.transform.Find("KnowledgeText").GetComponent<TMP_Text>();
        //Debug.Log($"{bridgeNum}, {fishNum}");
        //Debug.Log($"bridge1Image[fishNum - 1] == null is {bridge1Image[fishNum - 1] == null}");
        //Debug.Log($"triggerText == null is {triggerText == null}");

        switch (bridgeNum)
        {
            case 1:
                triggerImage.sprite = bridge1Image[fishNum - 1];
                triggerText.text = bridge1Text[fishNum - 1];
                bridge1fishes[fishNum - 1] = true;
                FishCounter();
                break;
            case 2:
                triggerImage.sprite = bridge2Image[fishNum - 1];
                triggerText.text = bridge2Text[fishNum - 1];
                bridge2fishes[fishNum - 1] = true;
                FishCounter();
                break;
            case 3:
                triggerImage.sprite = bridge3Image[fishNum - 1];
                triggerText.text = bridge3Text[fishNum - 1];
                bridge3fishes[fishNum - 1] = true;
                FishCounter();
                break;
        }
        Time.timeScale = 0;
    }

    public static void TriggerClose()
    {
        Hide(GameObject.Find("TriggersPanel"));
        Show(GameObject.Find("BasePanel"));
        Time.timeScale = 1;
    }

    // 发放pending的鱼干（小游戏通关/完成后调用）
    public static void AwardPendingFish()
    {
        if (pendingFishIndex < 0 || pendingFishAwarded)
            return;

        pendingFishAwarded = true;

        switch (pendingFishBridge)
        {
            case 1:
                if (!bridge1fishes[pendingFishIndex])
                {
                    bridge1fishes[pendingFishIndex] = true;
                    Debug.Log($"[鱼干] 晋祠鱼干{pendingFishIndex + 1} 收集成功！");
                }
                break;
            case 2:
                if (!bridge2fishes[pendingFishIndex])
                {
                    bridge2fishes[pendingFishIndex] = true;
                    Debug.Log($"[鱼干] 赵州桥鱼干{pendingFishIndex + 1} 收集成功！");
                }
                break;
            case 3:
                if (!bridge3fishes[pendingFishIndex])
                {
                    bridge3fishes[pendingFishIndex] = true;
                    Debug.Log($"[鱼干] 广济桥鱼干{pendingFishIndex + 1} 收集成功！");
                }
                break;
        }

        FishCounter();
    }

    // 进入小游戏时设置pending鱼干（由CatUbteract调用）
    public static void SetPendingFish(int bridge, int fishIdx)
    {
        pendingFishBridge = bridge;
        pendingFishIndex = fishIdx;
        pendingFishAwarded = false;
    }

    #endregion

    #region 游戏前界面

    public static void EnterGameOpen()
    {
        Show(GameObject.Find("EnterGamePanel"));
        Time.timeScale = 0;
    }

    public static void YesGame()
    {
        Hide(GameObject.Find("EnterGamePanel"));
        Time.timeScale = 1;
    }

    public static void NoGameTOBridge1()
    {
        SceneTransition.Instance.LoadScene(ThreeSceneName[1]);
    }

    public static void NoGameTOBridge2()
    {
        SceneTransition.Instance.LoadScene(ThreeSceneName[2]);
    }

    public static void NoGameTOBridge3()
    {
        SceneTransition.Instance.LoadScene(ThreeSceneName[3]);
    }

    #endregion

    void Start()
    {
        string nowSceneName = gameObject.scene.name;

        // 音乐由MusicControler单例统一管理，UIControler不再持有引用

        //Debug.Log($"isFirstEnterGame={isFirstEnterGame}");
        // 隐藏对话UI
        //if(gameObject.scene.name == "MainGame" && !isFirstEnterGame)

        // 进入场景时一些界面的显示和隐藏
        if (GameObject.Find("SettingPanel") != null)
        {
            SettingClose();
        }
        if (GameObject.Find("FishPanel") != null)
        {
            FishClose();
        }
        if (GameObject.Find("TriggersPanel") != null)
        {
            TriggerClose();
        }
        if (GameObject.Find("EnterGamePanel") != null)
        {
            Hide(GameObject.Find("EnterGamePanel"));
        }
        if (GameObject.Find("GameIntroductionPanel") != null)
        {
            Hide(GameObject.Find("GameIntroductionPanel"));
        }
        if (GameObject.Find("LoaddingPanel") != null)
        {
            Hide(GameObject.Find("LoaddingPanel"));
        }
        if (GameObject.Find("BridgeNamePanel") != null)
        {
            Hide(GameObject.Find("BridgeNamePanel"));
        }
        if (GameObject.Find("SwitchBGPanel") != null)
        {
            Hide(GameObject.Find("SwitchBGPanel"));
        }
        if (GameObject.Find("BasePanel") != null)
        {
            Hide(GameObject.Find("BasePanel"));
        }

        //// 设置 MusicSlider（如果存在）为当前音乐值
        //var musicSliderObj = GameObject.Find("MusicSlider");
        //if (musicSliderObj != null)
        //{
        //    var sliderComp = musicSliderObj.GetComponent<UnityEngine.UI.Slider>();
        //    if (sliderComp != null && MusicControler.Instance != null)
        //    {
        //        // 使用前四首的主音量作为滑动条初始值，并在值变化时更新 MusicControler
        //        sliderComp.value = MusicControler.Instance.GetMainVolume();
        //        // 保证不会重复订阅
        //        sliderComp.onValueChanged.RemoveAllListeners();
        //        sliderComp.onValueChanged.AddListener((float v) =>
        //        {
        //            MainMusicVolume = v;
        //            if (MusicControler.Instance != null)
        //                MusicControler.Instance.SetMainVolume(v);
        //        });
        //    }
        //}

        switch (nowSceneName)
        {
            case "0StartGame主界面":
                Show(GameObject.Find("StartMenuPanel"));
                break;
            case "SwitchScene选关界面":
                RegisterAllFishButtons();
                FishCounter();
                Show(GameObject.Find("BasePanel"));
                SwitchSceneOpen();
                break;
            case "1-晋祠鱼沼飞梁":
                RegisterAllFishButtons();
                FishCounter();
                Show(GameObject.Find("BasePanel"));
                break;
            case "2-赵州桥":
                RegisterAllFishButtons();
                FishCounter();
                Show(GameObject.Find("BasePanel"));
                break;
            case "3-广济桥":
                RegisterAllFishButtons();
                FishCounter();
                Show(GameObject.Find("BasePanel"));
                break;
            case "Fish":
            case "Puzzle":
            case "Question":
            case "Puzzle 1":
            case "Question 1":
            case "Puzzle 2":
            case "Question 2":
                RegisterAllFishButtons();
                FishCounter();
                Show(GameObject.Find("BasePanel"));
                Show(GameObject.Find("EnterGamePanel"));
                break;
            default:
                // GameObject switchPanel = GameObject.Find("BasePanel");
                break;
        }
        if (nowSceneName == "SwitchScene选关界面" || nowSceneName == ThreeSceneName[1] || nowSceneName == ThreeSceneName[2] || nowSceneName == ThreeSceneName[3])
        {
            //Debug.Log("进入SwitchScene，触发FishCounter");
            RegisterAllFishButtons();
            FishCounter();
            // CreateFishProgressBar();
            // UpdateFishProgressBar();
        }
        if (nowSceneName == "Fish" || nowSceneName == "Puzzle" || nowSceneName == "Question" ||
            nowSceneName == "Puzzle 1" || nowSceneName == "Question 1" ||
            nowSceneName == "Puzzle 2" || nowSceneName == "Question 2")
        {
            Show(GameObject.Find("BasePanel"));
            Show(GameObject.Find("EnterGamePanel"));
        }
    }


}