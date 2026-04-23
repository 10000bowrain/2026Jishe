using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneJump : MonoBehaviour
{
    // 小游戏完成后跳回的场景名，根据当前场景自动判断
    // 晋祠拼图(Puzzle)→1-晋祠鱼沼飞梁
    // 赵州桥拼图(Puzzle1)→2-赵州桥
    // 广济桥拼图(Puzzle2)→3-广济桥
    public string nextSceneName = "1-晋祠鱼沼飞梁";

    void Start()
    {
        // 自动检测当前拼图场景，设置正确的返回目标
        string currentScene = gameObject.scene.name;
        Debug.Log($"[SceneJump] 当前场景: {currentScene}");

        // 注意：CatUbteract 里用的是带空格的 "Puzzle 1" "Puzzle 2"
        if (currentScene == "Puzzle 1")
            nextSceneName = "2-赵州桥";
        else if (currentScene == "Puzzle 2")
            nextSceneName = "3-广济桥";
        else
            nextSceneName = "1-晋祠鱼沼飞梁"; // 默认晋祠（拼图或未知场景）

        Debug.Log($"[SceneJump] 将跳转到: {nextSceneName}");
    }

    public void Scene()
    {
        // 返回主场景
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("SceneJump: nextSceneName 未填写！请在Inspector里填入跳转场景名。");
            return;
        }
        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.LoadScene(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
