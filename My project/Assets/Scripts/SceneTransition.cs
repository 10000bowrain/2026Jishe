using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [Header("渐变速度（数值越大越快）")]
    public float fadeSpeed = 15f;
    public float showSpeed = 30f;

    [Header("桥场景名称")]
    public string jinciSceneName = "1-晋祠鱼沼飞梁";
    public string zhaozhouSceneName = "2-赵州桥";
    public string guangjiSceneName = "3-广济桥";
    public string loadingSceneName = "LoaddingScene";

    private Image fadeImage;
    private CanvasGroup canvasGroup;
    private bool isTransitioning = false;

    // 桥场景异步加载操作
    private AsyncOperation pendingBridgeLoad;
    private string pendingTargetScene;

    // 当前活跃的桥场景
    private string currentBridgeScene = null;

    // 晋祠是否已预加载（整个游戏生命周期只加载一次）
    private bool jinCiPreloaded = false;

    // 进度条UI
    private GameObject loadingProgressObj;
    private Image loadingFillImage;
    private TMP_Text loadingText;

    // 公开方法
    public bool IsTransitioning()
    {
        return isTransitioning;
    }

    // 获取当前活跃的桥场景
    public string GetCurrentBridgeScene()
    {
        return currentBridgeScene;
    }

    // 晋祠是否已预加载
    public bool IsJinCiPreloaded()
    {
        return jinCiPreloaded;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SetupFadeImage();
    }

    private void SetupFadeImage()
    {
        if (fadeImage != null && fadeImage.gameObject.activeInHierarchy)
            return;

        fadeImage = null;
        canvasGroup = null;

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("[SceneTransition] 场景中没有Canvas！");
            return;
        }

        Transform existing = canvas.transform.Find("FadeImage");
        if (existing != null)
        {
            fadeImage = existing.GetComponent<Image>();
            if (fadeImage == null)
            {
                Debug.LogWarning("[SceneTransition] 复用FadeImage但没有Image组件！");
                return;
            }
            
            canvasGroup = existing.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = existing.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.raycastTarget = false;
            return;
        }

        GameObject fadeCanvasObj = new GameObject("FadeOverlayCanvas");
        Canvas fadeCanvas = fadeCanvasObj.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.overrideSorting = true;
        fadeCanvas.sortingOrder = 32766;
        fadeCanvasObj.AddComponent<CanvasScaler>();
        fadeCanvasObj.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(fadeCanvasObj);

        GameObject imgObj = new GameObject("FadeImage");
        imgObj.transform.SetParent(fadeCanvasObj.transform, false);

        RectTransform rect = imgObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        
        fadeImage = imgObj.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.raycastTarget = false;

        canvasGroup = imgObj.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
    }

    // 创建加载进度条UI
    private void CreateLoadingProgressUI()
    {
        if (loadingProgressObj != null)
        {
            Destroy(loadingProgressObj);
            loadingProgressObj = null;
            loadingFillImage = null;
            loadingText = null;
        }

        GameObject canvasObj = new GameObject("LoadingOverlayCanvas");
        Canvas overlayCanvas = canvasObj.AddComponent<Canvas>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        overlayCanvas.sortingOrder = 32767;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        loadingProgressObj = new GameObject("LoadingProgress");
        loadingProgressObj.transform.SetParent(canvasObj.transform);
        RectTransform rect = loadingProgressObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0, -80);
        rect.sizeDelta = new Vector2(400, 20);

        // 背景条
        GameObject bgObj = new GameObject("FillBG");
        bgObj.transform.SetParent(loadingProgressObj.transform);
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.pivot = new Vector2(0.5f, 0.5f);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.5f);

        // 进度填充
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(loadingProgressObj.transform);
        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.pivot = new Vector2(0, 0.5f);
        fillRect.anchoredPosition = Vector2.zero;
        loadingFillImage = fillObj.AddComponent<Image>();
        loadingFillImage.type = UnityEngine.UI.Image.Type.Filled;
        loadingFillImage.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
        loadingFillImage.fillOrigin = (int)UnityEngine.UI.Image.OriginHorizontal.Left;
        loadingFillImage.fillAmount = 0f;
        loadingFillImage.color = new Color(1f, 0.8f, 0.4f, 1f);

        // 文字
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(loadingProgressObj.transform);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 1);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.sizeDelta = new Vector2(0, 30);
        textRect.pivot = new Vector2(0.5f, 1f);
        textRect.anchoredPosition = new Vector2(0, 0);
        loadingText = textObj.AddComponent<TMPro.TextMeshProUGUI>();
        if (loadingText != null)
        {
            loadingText.text = "0%";
            loadingText.fontSize = 18;
            loadingText.alignment = TextAlignmentOptions.Center;
            loadingText.color = Color.white;
        }
    }

    private void UpdateLoadingProgress(float progress)
    {
        if (loadingProgressObj == null || loadingFillImage == null || loadingText == null) return;
        loadingFillImage.fillAmount = progress;
        loadingText.text = Mathf.RoundToInt(progress * 100) + "%";
    }

    private void DestroyLoadingProgress()
    {
        if (loadingProgressObj != null)
        {
            Transform parentCanvas = loadingProgressObj.transform.parent;
            Destroy(loadingProgressObj);
            loadingProgressObj = null;
            loadingFillImage = null;
            loadingText = null;
            if (parentCanvas != null)
                Destroy(parentCanvas.gameObject);
        }
    }

    /// <summary>
    /// 统一的场景切换方法
    /// 规则：
    /// 1. 任意场景切换都有渐黑和渐亮
    /// 2. 切换到三个桥场景（晋祠/赵州桥/广济桥）：
    ///    - 先渐黑
    ///    - 于此同时后台加载对应的桥场景
    ///    - 然后切换到LoadingScene
    ///    - 等待桥场景加载完成
    ///    - 切换到对应桥场景
    ///    - 渐亮
    /// 3. 切换到其他场景：
    ///    - 直接渐变切换
    /// </summary>
    public void LoadScene(string sceneName)
    {
        // 防止重复调用
        if (isTransitioning) return;

        // 检查场景是否存在于BuildSettings
        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogError($"[SceneTransition] 场景 '{sceneName}' 不存在于BuildSettings！");
            return;
        }

        Debug.Log($"[SceneTransition] LoadScene请求: {sceneName}, 当前桥场景: {currentBridgeScene}, 晋祠已预加载: {jinCiPreloaded}");
        isTransitioning = true;
        StartCoroutine(Transition(sceneName));
    }

    /// <summary>
    /// 检查场景是否存在于BuildSettings
    /// </summary>
    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 判断是否为桥场景（晋祠/赵州桥/广济桥）
    /// </summary>
    private bool IsBridgeScene(string sceneName)
    {
        return sceneName == jinciSceneName || sceneName == zhaozhouSceneName || sceneName == guangjiSceneName;
    }

    /// <summary>
    /// 判断是否为晋祠场景
    /// </summary>
    private bool IsJinCiScene(string sceneName)
    {
        return sceneName == jinciSceneName;
    }

    /// <summary>
    /// 判断是否为赵州桥或广济桥
    /// </summary>
    private bool IsOtherBridgeScene(string sceneName)
    {
        return sceneName == zhaozhouSceneName || sceneName == guangjiSceneName;
    }

    /// <summary>
    /// 检查晋祠场景是否已加载
    /// </summary>
    private bool IsJinCiLoaded()
    {
        Scene s = SceneManager.GetSceneByName(jinciSceneName);
        return s.IsValid() && s.isLoaded;
    }

    /// <summary>
    /// 核心切换流程
    /// 规则1：任意场景切换都有渐黑和渐亮
    /// 规则2：切换到桥场景走特殊流程（LoadingScene + 进度条）
    /// 规则3：切换到其他场景直接切换
    /// </summary>
    private IEnumerator Transition(string targetScene)
    {
        Debug.Log($"[SceneTransition] === 开始切换到: {targetScene} ===");

        // ========== 阶段1：渐黑 ==========
        // 确保fadeImage有效
        if (fadeImage == null)
        {
            SetupFadeImage();
            if (fadeImage == null)
            {
                Debug.LogError("[SceneTransition] fadeImage为null，无法切换！");
                isTransitioning = false;
                yield break;
            }
        }

        // 0.5 音乐淡出
        bool needFadeOut = true;
        if (MusicControler.Instance != null)
        {
            needFadeOut = !MusicControler.Instance.ShouldSkipFadeOut(targetScene);
        }
        if (needFadeOut && MusicControler.Instance != null)
        {
            MusicControler.Instance.FadeOutCurrent();
        }

        // 1. 渐黑
        fadeImage.color = new Color(0, 0, 0, 0);
        while (fadeImage.color.a < 0.99f)
        {
            float newAlpha = fadeImage.color.a + fadeSpeed * Time.unscaledDeltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(newAlpha));
            yield return null;
        }
        fadeImage.color = Color.black;

        // 等待音乐淡出
        if (needFadeOut && MusicControler.Instance != null)
        {
            float fadeOutDuration = MusicControler.Instance.GetFadeOutDuration();
            yield return new WaitForSecondsRealtime(fadeOutDuration);
        }

        // 2. 根据目标场景类型执行切换逻辑
        // 规则2：切换到桥场景走HandleBridgeSceneSwitch（有LoadingScene进度条）
        // 规则3：切换到其他场景走HandleNormalSceneSwitch（直接切换）
        // 优化：如果目标是晋祠且已预加载，走普通切换（跳过LoadingScene）
        if (IsBridgeScene(targetScene) && !(IsJinCiScene(targetScene) && jinCiPreloaded))
        {
            yield return HandleBridgeSceneSwitch(targetScene);
        }
        else
        {
            yield return HandleNormalSceneSwitch(targetScene);
        }

        // ========== 阶段3：渐亮 ==========
        // 3. 等待新场景准备好
        yield return new WaitForSecondsRealtime(0.1f);

        // 4. 重新设置fadeImage
        for (int i = 0; i < 3; i++)
        {
            yield return null;
            SetupFadeImage();
            if (fadeImage != null) break;
        }

        if (fadeImage == null)
        {
            Debug.LogError("[SceneTransition] FadeImage创建失败！");
            isTransitioning = false;
            yield break;
        }

        // 5. 触发新场景音乐
        if (MusicControler.Instance != null)
        {
            MusicControler.Instance.FadeInForScene(targetScene);
        }

        // 6. 渐亮
        fadeImage.color = new Color(0, 0, 0, 1f);
        fadeImage.raycastTarget = true;

        float fadeInDuration = 1f / showSpeed;
        float elapsed = 0f;
        while (fadeImage.color.a > 0.01f)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / fadeInDuration);
            float newAlpha = 1f - progress;
            fadeImage.color = new Color(0, 0, 0, newAlpha);

            if (newAlpha < 0.2f)
                fadeImage.raycastTarget = false;

            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.raycastTarget = false;

        // 7. 清理进度条
        DestroyLoadingProgress();

        isTransitioning = false;
        Debug.Log($"[SceneTransition] === 切换完成: {targetScene} ===");
    }

    /// <summary>
    /// 处理桥场景切换
    /// 流程：后台加载目标桥 → LoadingScene → 等待加载完成 → 激活桥场景
    /// 注意：渐黑和渐亮由Transition方法统一处理
    /// </summary>
    private IEnumerator HandleBridgeSceneSwitch(string targetScene)
    {
        Debug.Log($"[SceneTransition] HandleBridgeSceneSwitch: 目标={targetScene}, 当前={currentBridgeScene}");

        bool isTargetJinCi = IsJinCiScene(targetScene);
        bool isTargetOtherBridge = IsOtherBridgeScene(targetScene);

        // ========== 步骤1：卸载旧场景 ==========
        // 如果当前有其他桥场景，先卸载
        if (currentBridgeScene != null && currentBridgeScene != targetScene)
        {
            Scene oldScene = SceneManager.GetSceneByName(currentBridgeScene);
            if (oldScene.IsValid())
            {
                Debug.Log($"[SceneTransition] 卸载旧桥场景: {currentBridgeScene}");
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentBridgeScene);
                if (unloadOp != null)
                    yield return unloadOp;
            }
        }

        // 如果目标是赵州桥或广济桥，需要先卸载晋祠（整个游戏最多只有一个晋祠）
        if (isTargetOtherBridge && IsJinCiLoaded())
        {
            Debug.Log($"[SceneTransition] 目标桥是 {targetScene}，先卸载晋祠");
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(jinciSceneName);
            if (unloadOp != null)
                yield return unloadOp;
            jinCiPreloaded = false;
        }

        // ========== 步骤2：切换到LoadingScene ==========
        Debug.Log("[SceneTransition] 切换到LoadingScene");
        SceneManager.LoadScene(loadingSceneName);
        yield return null;

        // 创建进度条
        CreateLoadingProgressUI();
        UpdateLoadingProgress(0f);

        // ========== 步骤3：后台加载目标桥场景 ==========
        if (isTargetJinCi)
        {
            // 目标是晋祠：加载晋祠
            if (!IsJinCiLoaded())
            {
                Debug.Log($"[SceneTransition] 加载晋祠场景: {jinciSceneName}");
                AsyncOperation loadOp = SceneManager.LoadSceneAsync(jinciSceneName, LoadSceneMode.Additive);
                if (loadOp != null)
                {
                    pendingBridgeLoad = loadOp;
                    pendingTargetScene = jinciSceneName;

                    // 等待加载完成，同时更新进度条
                    while (!loadOp.isDone)
                    {
                        UpdateLoadingProgress(loadOp.progress / 0.9f);
                        yield return null;
                    }
                }
                jinCiPreloaded = true;
            }
            else
            {
                Debug.Log("[SceneTransition] 晋祠已加载，跳过加载");
                UpdateLoadingProgress(1f);
            }
        }
        else if (isTargetOtherBridge)
        {
            // 目标是其他桥：加载目标桥
            Debug.Log($"[SceneTransition] 加载目标桥场景: {targetScene}");
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
            if (loadOp != null)
            {
                pendingBridgeLoad = loadOp;
                pendingTargetScene = targetScene;

                // 等待加载完成
                while (!loadOp.isDone)
                {
                    UpdateLoadingProgress(loadOp.progress / 0.9f);
                    yield return null;
                }
            }
        }

        // ========== 步骤4：激活目标场景 ==========
        // 清理进度条
        DestroyLoadingProgress();

        // 卸载LoadingScene
        Debug.Log("[SceneTransition] 卸载LoadingScene");
        AsyncOperation unloadLoading = SceneManager.UnloadSceneAsync(loadingSceneName);
        if (unloadLoading != null)
            yield return unloadLoading;

        // 激活目标场景
        if (isTargetJinCi)
        {
            Scene jinCiScene = SceneManager.GetSceneByName(jinciSceneName);
            if (jinCiScene.IsValid())
            {
                SceneManager.SetActiveScene(jinCiScene);
            }
            currentBridgeScene = jinciSceneName;
        }
        else if (isTargetOtherBridge)
        {
            Scene targetSceneObj = SceneManager.GetSceneByName(targetScene);
            if (targetSceneObj.IsValid())
            {
                SceneManager.SetActiveScene(targetSceneObj);
            }
            currentBridgeScene = targetScene;
        }

        pendingBridgeLoad = null;
        pendingTargetScene = null;

        Debug.Log($"[SceneTransition] 桥场景切换完成: {targetScene}");
    }

    /// <summary>
    /// 处理普通场景切换
    /// 直接加载目标场景
    /// 注意：渐黑和渐亮由Transition方法统一处理
    /// </summary>
    private IEnumerator HandleNormalSceneSwitch(string targetScene)
    {
        Debug.Log($"[SceneTransition] HandleNormalSceneSwitch: 目标={targetScene}, 当前桥={currentBridgeScene}");

        // 保存之前的桥场景（用于从小游戏返回）
        string previousBridgeScene = currentBridgeScene;

        // 如果当前在赵州桥/广济桥，需要预加载晋祠
        if (IsOtherBridgeScene(currentBridgeScene))
        {
            Debug.Log("[SceneTransition] 当前在其他桥，预加载晋祠");

            // 先卸载当前桥
            Scene oldScene = SceneManager.GetSceneByName(currentBridgeScene);
            if (oldScene.IsValid())
            {
                Debug.Log($"[SceneTransition] 卸载当前桥场景: {currentBridgeScene}");
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentBridgeScene);
                if (unloadOp != null)
                    yield return unloadOp;
            }

            // 预加载晋祠（不阻塞，异步）
            if (!IsJinCiLoaded())
            {
                Debug.Log("[SceneTransition] 异步预加载晋祠");
                AsyncOperation loadOp = SceneManager.LoadSceneAsync(jinciSceneName, LoadSceneMode.Additive);
                if (loadOp != null)
                {
                    pendingBridgeLoad = loadOp;
                    pendingTargetScene = jinciSceneName;
                }
            }
            jinCiPreloaded = true;
        }

        // ========== 直接加载目标场景 ==========
        Debug.Log($"[SceneTransition] 加载目标场景: {targetScene}");
        SceneManager.LoadScene(targetScene);

        // 更新状态
        if (IsJinCiScene(targetScene))
        {
            currentBridgeScene = jinciSceneName;
            jinCiPreloaded = true;
        }
        else if (IsOtherBridgeScene(targetScene))
        {
            currentBridgeScene = targetScene;
        }
        else
        {
            // 非桥场景（小游戏等），保留之前桥场景记录
            Debug.Log($"[SceneTransition] 进入非桥场景 {targetScene}，保留之前桥场景: {previousBridgeScene}");
        }

        pendingBridgeLoad = null;
        pendingTargetScene = null;

        yield return null;
    }

    /// <summary>
    /// 进入游戏时调用，用于预加载晋祠场景
    /// </summary>
    public void PreloadJinCiScene()
    {
        if (isTransitioning) return;

        if (!IsSceneInBuildSettings(jinciSceneName))
        {
            Debug.LogWarning($"[SceneTransition] 晋祠场景不存在: {jinciSceneName}");
            return;
        }

        if (IsJinCiLoaded())
        {
            Debug.Log("[SceneTransition] 晋祠已加载，跳过预加载");
            jinCiPreloaded = true;
            currentBridgeScene = jinciSceneName;
            return;
        }

        Debug.Log($"[SceneTransition] 预加载晋祠场景: {jinciSceneName}");
        StartCoroutine(PreloadJinCiCoroutine());
    }

    private IEnumerator PreloadJinCiCoroutine()
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(jinciSceneName, LoadSceneMode.Additive);
        if (loadOp != null)
        {
            pendingBridgeLoad = loadOp;
            pendingTargetScene = jinciSceneName;

            while (!loadOp.isDone)
            {
                yield return null;
            }
        }

        jinCiPreloaded = true;
        currentBridgeScene = jinciSceneName;
        pendingBridgeLoad = null;
        pendingTargetScene = null;

        Debug.Log("[SceneTransition] 晋祠场景预加载完成");
    }
}
