using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFade : MonoBehaviour
{
    public static SceneFade Instance;
    public Image fadeImage;
    public float fadeSpeed = 1.25f;
    public float showSpeed = 0.75f;

    // 核心：根节点绑定DontDestroyOnLoad，永不销毁
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 根节点全局不销毁，彻底杜绝场景切换重置
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // 初始透明
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    // 调用跳转
    public void LoadScene(string sceneName)
    {
        StartCoroutine(Transition(sceneName));
    }

    // 完整过渡：渐黑→加载→渐亮
    IEnumerator Transition(string sceneName)
    {
        // 1. 同时启动渐黑和加载场景
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        load.allowSceneActivation = false;

        while (fadeImage.color.a < 0.9f)
        {
            fadeImage.color += new Color(0, 0, 0, fadeSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
        fadeImage.color = Color.black;

        // 2. 等待场景加载到90%
        while (load.progress < 0.9f)
        {
            yield return null;
        }
        // 3. 激活场景
        load.allowSceneActivation = true;
        // 4. 等待场景真正切换完成
        while (!load.isDone)
        {
            yield return null;
        }

        // 5. 渐亮
        while (fadeImage.color.a > 0.01f)
        {
            fadeImage.color -= new Color(0, 0, 0, showSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
    }
}