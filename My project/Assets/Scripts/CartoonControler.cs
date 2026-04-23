using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CartoonControler : MonoBehaviour
{
    public Image Image1;
    public Image Image2;
    public Image Image3;
    public Image Image4;

    [Header("背景插图（在四张漫画图下方显示）")]
    public Image BackgroundImage;

    public Button SkipBtn;
    public float fadeTime = 2f;         // 每张图片渐显耗时（4张图共4秒）
    public float autoJumpDelay = 2f;    // 全部显示后等待2秒自动跳转

    // 状态
    private bool isWaitingForJump = false;
    private Coroutine fadeCoroutine;    // 淡入协程引用
    private Coroutine autoJumpCoroutine;

    void OnEnable()
    {
        Debug.Log("=== OnEnable 被调用 ===");
        // 每次进入场景都重置状态
        isWaitingForJump = false;
        
        // 立即设置图片透明
        SetAllImagesAlphaImmediate(0f);
        if (BackgroundImage != null)
            BackgroundImage.color = SetAlpha(BackgroundImage.color, 0f);
    }

    void Start()
    {
        //Debug.Log("=== Start 被调用 ===");

        //// 停止所有残留协程（关键！）
        //if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        //if (autoJumpCoroutine != null) StopCoroutine(autoJumpCoroutine);

        //// 隐藏加载面板
        //GameObject loadingPanel = GameObject.Find("LoaddingPanel");
        //if (loadingPanel != null) loadingPanel.SetActive(false);

        // SkipBtn 绑定
        if (SkipBtn != null)
        {
            SkipBtn.onClick.RemoveAllListeners();
            SkipBtn.onClick.AddListener(OnSkipClicked);
        }

        //// 启动淡入协程
        //Debug.Log("启动FadeImagesInOrder协程");
        fadeCoroutine = StartCoroutine(FadeImagesInOrder(fadeTime));
    }

    public void OnSkipClicked()
    {
        Debug.Log("OnSkipClicked isWaitingForJump=" + isWaitingForJump);
        
        // 如果已经在等待跳转状态，直接跳转到下一个界面
        if (isWaitingForJump)
        {
            StopAllCoroutines();
            fadeCoroutine = null;
            GoToNextScene();
            return;
        }

        // 停止淡入协程
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        // 立即显示所有图片
        SetAllImagesAlphaImmediate(1f);
        if (BackgroundImage != null)
            BackgroundImage.color = SetAlpha(BackgroundImage.color, 1f);

        // 开始等待自动跳转
        StartWaitingForJump();
    }

    private IEnumerator FadeImagesInOrder(float fadeDuration)
    {
        Debug.Log("FadeImagesInOrder 开始");
        
        // 依次淡入4张图片
        if (Image1 != null) yield return FadeSingleImage(Image1, 0, 1, fadeDuration);
        if (Image2 != null) yield return FadeSingleImage(Image2, 0, 1, fadeDuration);
        if (Image3 != null) yield return FadeSingleImage(Image3, 0, 1, fadeDuration);
        if (Image4 != null) yield return FadeSingleImage(Image4, 0, 1, fadeDuration);

        Debug.Log("FadeImagesInOrder 淡入完成");
        fadeCoroutine = null; // 协程结束，清除引用

        // 如果还没进入等待跳转状态，才开始等待
        if (!isWaitingForJump)
        {
            StartWaitingForJump();
        }
    }

    private void StartWaitingForJump()
    {
        if (isWaitingForJump) return; // 防止重复启动
        isWaitingForJump = true;
        Debug.Log("StartWaitingForJump 开始等待 " + autoJumpDelay + "秒");

        autoJumpCoroutine = StartCoroutine(AutoJumpAfterDelay());
    }

    private IEnumerator AutoJumpAfterDelay()
    {
        yield return new WaitForSeconds(autoJumpDelay);
        Debug.Log("等待结束，检查isWaitingForJump=" + isWaitingForJump);

        if (isWaitingForJump)
        {
            Debug.Log("执行自动跳转");
            GoToNextScene();
        }
    }

    private void GoToNextScene()
    {
        string currentScene = gameObject.scene.name;
        string nextScene = "";

        if (currentScene == "Cartoon1Scene漫画1")
            nextScene = "Cartoon2Scene漫画2";
        else if (currentScene == "Cartoon2Scene漫画2")
            nextScene = "Cartoon3Scene漫画3";
        else if (currentScene == "Cartoon3Scene漫画3")
            nextScene = "SwitchScene选关界面";

        Time.timeScale = 1;

        if (!string.IsNullOrEmpty(nextScene))
        {
            if (SceneTransition.Instance == null)
            {
                GameObject stObj = new GameObject("SceneTransition");
                stObj.AddComponent<SceneTransition>();
            }       
            SceneTransition.Instance.LoadScene(nextScene);
        }
        else
        {
            isWaitingForJump = false;
        }
    }

    private IEnumerator FadeSingleImage(Image image, float startAlpha, float endAlpha, float time)
    {
        if (image == null) yield break;
        float timer = 0;
        Color originColor = image.color;

        while (timer < time)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / time);
            image.color = SetAlpha(originColor, alpha);
            yield return null;
        }
        image.color = SetAlpha(originColor, endAlpha);
    }

    private void SetAllImagesAlphaImmediate(float alpha)
    {
        if (Image1) Image1.color = SetAlpha(Image1.color, alpha);
        if (Image2) Image2.color = SetAlpha(Image2.color, alpha);
        if (Image3) Image3.color = SetAlpha(Image3.color, alpha);
        if (Image4) Image4.color = SetAlpha(Image4.color, alpha);
    }

    private Color SetAlpha(Color c, float a)
    {
        return new Color(c.r, c.g, c.b, a);
    }
}
