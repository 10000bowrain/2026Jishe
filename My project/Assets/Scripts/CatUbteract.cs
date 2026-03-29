using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 必须加，用于调用TextMeshPro

public class CatDistanceCheck : MonoBehaviour
{
    [Header("=== 拖拽设置 ===")]
    public Transform cat; // 拖拽场景中的Cat胶囊体到这里
    public TextMeshProUGUI tipText; // 拖拽刚才创建的CatTip文字到这里

    [Header("=== 触发距离(米) ===")]
    public float triggerDistance = 3f; // 角色离小猫3米内显示文字

    // 每帧检测距离
    void Update()
    {
        // 计算 角色 和 Cat 的直线距离
        float distance = Vector3.Distance(transform.position, cat.transform.position);
        // 判断距离：小于设定值 → 显示文字，否则隐藏
        if (distance < triggerDistance)
        {
            tipText.enabled = true;
            // tipText.color = new Color(255,255,255);
            Debug.Log("距离小于3");
            Debug.Log($"{tipText.enabled}");
            if (Input.GetKeyDown(KeyCode.E))
            {
                OnInteractWithCat();
            }
        }

        else
        {
            tipText.enabled = false;
        }
    }
}
