using UnityEngine;

/// <summary>
/// 让Canvas及其子物体跨场景常驻，不被销毁
/// </summary>
public class PersistentCanvas : MonoBehaviour
{
    private void Awake()
    {
        // 单例防重复：保证全局只有一个Canvas
        if (FindObjectOfType<PersistentCanvas>() != this)
        {
            Destroy(gameObject);
            return;
        }

        // 核心：标记物体不随场景切换销毁
        DontDestroyOnLoad(gameObject);
    }
}