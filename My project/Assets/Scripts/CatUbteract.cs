using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 必须加，用于调用TextMeshPro

public class CatDistanceCheck : MonoBehaviour
{
    [Header("拖拽设置")]
    public Transform cat; // 拖拽场景中的Cat胶囊体到这里
    public TextMeshProUGUI tipText; // 拖拽刚才创建的CatTip文字到这里
    public DialogueCore dialogueCore; // 新增：拖拽Hierarchy面板的DialogPanel（绑定了核心脚本）
    public DialogueDataSO catDialogueData; // 新增：拖拽Cat的对话配置文件（提前创建好）

    [Header("触发距离(米)")]
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
            Debug.Log("距离小于3");
            Debug.Log($"{tipText.enabled}");
            // 新增：按E键触发对话（判断对话未在播放、核心脚本和配置文件均绑定）
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && catDialogueData != null && !dialogueCore.isDialoguePlaying)
            {
                // 调用核心脚本，启动Cat的对话（对接原有对话系统）
                dialogueCore.StartDialogue(catDialogueData);
            }
        }
        else
        {
            tipText.enabled = false;
        }
    }
}