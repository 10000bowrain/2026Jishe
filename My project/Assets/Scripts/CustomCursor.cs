using UnityEngine;

/// <summary>
/// 自定义鼠标光标设置脚本
/// 挂载到任意持久化的对象上（如 PersistentCanvas 或 SceneTransition）
/// </summary>
public class CustomCursor : MonoBehaviour
{
    [Header("光标图片")]
    public Texture2D cursorTexture;

    [Header("光标热点（鼠标点击位置）")]
    public Vector2 hotspot = Vector2.zero;

    [Header("光标模式")]
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        SetCustomCursor();
    }

    public void SetCustomCursor()
    {
        if (cursorTexture != null)
        {
            // hotspot 是光标的"点击点"，设为左上角 (0, 高度)
            // 这样鼠标指针的左上角就是实际点击位置
            Vector2 hotSpot = new Vector2(0, cursorTexture.height);
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
            Debug.Log($"[CustomCursor] 光标已设置: {cursorTexture.name}");
        }
        else
        {
            Debug.LogWarning("[CustomCursor] 未设置光标图片！");
        }
    }

    // 恢复默认光标
    public void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
