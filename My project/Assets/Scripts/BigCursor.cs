using UnityEngine;

/// <summary>
/// 自定义光标系统
/// - 隐藏系统光标
/// - 用小图片设置系统光标（精准点击）
/// - 用大图片显示光标（清晰好看）
/// </summary>
public class BigCursor : MonoBehaviour
{
    public static BigCursor Instance;

    [Header("系统光标用的小图（48x48或64x64）")]
    public Texture2D systemCursor;

    [Header("显示用的大图（2048也行，会缩放显示）")]
    public Texture2D displayCursor;

    [Header("显示光标的大小")]
    public float displaySize = 48f;

    [Header("显示光标是否可见")]
    public bool showDisplayCursor = true;

    // 用这个来存储鼠标位置，避免Event.current问题
    private Vector3 mousePos;

    void Awake()
    {
        Instance = this;
        // 立即隐藏系统光标
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;

        // 设置系统光标
        SetSystemCursor();
    }

    void SetSystemCursor()
    {
        if (systemCursor != null)
        {
            // 热点设置在左上角
            Vector2 hotspot = new Vector2(0, systemCursor.height);
            Cursor.SetCursor(systemCursor, hotspot, CursorMode.Auto);
        }
    }

    void Update()
    {
        // 记录鼠标位置
        mousePos = Input.mousePosition;
    }

    void OnGUI()
    {
        if (!showDisplayCursor || displayCursor == null)
            return;

        // 使用记录的鼠标位置计算绘制位置
        float x = mousePos.x - displaySize / 2;
        float y = Screen.height - mousePos.y - displaySize / 2;

        GUI.DrawTexture(new Rect(x, y, displaySize, displaySize), displayCursor);
    }

    void OnDestroy()
    {
        // 退出时恢复
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true;
    }
}
