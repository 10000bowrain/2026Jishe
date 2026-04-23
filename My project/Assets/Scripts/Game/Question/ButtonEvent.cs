using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvent : MonoBehaviour
{
    public GameObject EndCanvas;
    // 小游戏完成后跳回的场景名，根据当前问答场景自动判断
    // 晋祠问答(Question)→1-晋祠鱼沼飞梁
    // 赵州桥问答(Question1)→2-赵州桥
    // 广济桥问答(Question2)→3-广济桥
    public string nextSceneName = "1-晋祠鱼沼飞梁";

    void Start()
    {
        // 自动检测当前问答场景，设置正确的返回目标
        string currentScene = gameObject.scene.name;
        Debug.Log($"[ButtonEvent] 当前场景: {currentScene}");

        // 注意：CatUbteract 里用的是带空格的 "Question 1" "Question 2"
        if (currentScene == "Question 1")
            nextSceneName = "2-赵州桥";
        else if (currentScene == "Question 2")
            nextSceneName = "3-广济桥";
        else
            nextSceneName = "1-晋祠鱼沼飞梁"; // 默认晋祠（晋祠问答或未知场景）

        Debug.Log($"[ButtonEvent] 将跳转到: {nextSceneName}");
    }

    public void ReStart()
    {
        // 重置问答游戏
        QuizManager quiz = GetComponent<QuizManager>();
        if (quiz != null)
            quiz.ResetGame();
        else
            Debug.LogWarning("[ButtonEvent] 未找到 QuizManager 组件，无法重置");
    }

    public void SceneJump()
    {
        // 兼容旧模式：返回主场景
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("ButtonEvent: nextSceneName 未填写！请在Inspector里填入跳转场景名。");
            return;
        }
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(nextSceneName);
        else
            SceneManager.LoadScene(nextSceneName);
    }
}
