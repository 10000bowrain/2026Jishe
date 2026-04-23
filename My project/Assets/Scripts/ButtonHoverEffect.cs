using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 给按钮加悬停放大效果：鼠标放上微微放大，移开恢复原样
/// 挂在每个按钮上即可使用
/// </summary>
[RequireComponent(typeof(Selectable))]
public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("放大幅度（1.1 = 放大10%）")]
    public float hoverScale = 1.1f;

    [Header("缩放速度")]
    public float scaleSpeed = 10f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        // 平滑缩放到目标大小
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
}
