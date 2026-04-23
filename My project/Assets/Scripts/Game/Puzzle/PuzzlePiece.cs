using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public bool isComplete; // 是否已完成拼图
    private bool isDragging = false; // 是否正在拖拽
    private Vector3 offset; // 鼠标点击位置与拼图块中心的偏移
    private Camera cam; // 主摄像机
    public Transform targetPosition; // 目标位置
    public Vector3 originalPosition;
    public float snapThreshold = 0.5f; // 吸附阈值

    void Start()
    {
        cam = Camera.main; // 获取主摄像机
        originalPosition = transform.position;
    }

    void OnMouseDown()
    {
        if (isComplete)
            return; // 如果已完成拼图，则不处理
        transform.localScale= new Vector3(1.5f,1.5f,1.5f);
        isDragging = true; // 开始拖拽
        offset = transform.position - GetMouseWorldPosition(); // 计算偏移
    }

    void OnMouseUp()
    {
        // 先重置拖拽状态和缩放（无论是否完成都要重置）
        isDragging = false;
        transform.localScale = new Vector3(1, 1, 1);

        if (isComplete)
            return; // 如果已完成拼图，则不处理后续逻辑

        CheckPosition(); // 检查位置
        if (!isComplete)
            transform.position = originalPosition;
    }

    void Update()
    {
        // 如果拼图已完成，直接返回，不响应任何拖拽逻辑
        if (isComplete)
            return;

        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset; // 更新拼图块位置
        }
    }

    // 获取鼠标在世界空间中的位置
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition; // 获取鼠标位置
        mousePoint.z = cam.WorldToScreenPoint(transform.position).z; // 获取Z坐标
        return cam.ScreenToWorldPoint(mousePoint); // 转换为世界坐标
    }

    // 检查拼图块是否接近目标位置
    private void CheckPosition()
    {
        if (Vector3.Distance(transform.position, targetPosition.position) < snapThreshold)
        {
            transform.position = targetPosition.position; // 如果接近，则吸附到目标位置
            isComplete = true; // 设置已完成拼图
            PuzzleMG.instance.Check();
        }
    }

    // 重置拼图块状态（用于重玩）
    public void ResetPiece()
    {
        isComplete = false;
        isDragging = false;
        transform.position = originalPosition;
        transform.localScale = Vector3.one;
    }
}
