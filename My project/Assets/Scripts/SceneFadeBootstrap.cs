using UnityEngine;

/// <summary>
/// 空的SceneFade补充组件：确保每个Cartoon场景都有SceneFade可用
/// 只起到"让SceneFade.Awake()执行、Instance被赋值"的作用
/// 无需拖入任何引用！
/// </summary>
public class SceneFadeBootstrap : MonoBehaviour
{
    void Awake()
    {
        // SceneFade.Instance会在子物体的SceneFade Awake中自动赋值
        // 这里只需要保证这个GameObject存在且有DontDestroyOnLoad父级
        DontDestroyOnLoad(gameObject);
    }
}
