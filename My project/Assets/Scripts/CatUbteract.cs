using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.ComponentModel.Design; // 必须加，用于调用TextMeshPro

public class CatDistanceCheck : MonoBehaviour
{
    [Header("拖拽设置")]
    public Transform cat; // 拖拽场景中的Cat胶囊体到这里
    public TextMeshProUGUI tipText; // 拖拽刚才创建的CatTip文字到这里
    public DialogueCore dialogueCore; // 新增：拖拽Hierarchy面板的DialogPanel（绑定了核心脚本）

    [Header("与猫的首次对话")]
    public DialogueDataSO catStartDialogueData;

    [Header("小于管收集途中对话")]
    public DialogueDataSO catHalfDialogueData;

    [Header("小鱼干全部收集完成对话")]
    public DialogueDataSO catEndDialogueData;

    [Header("提示与猫对话文字触发距离")]
    public float triggerDistance = 3f; 

    [Header("是否为与猫的首次对话")]
    public static bool isCatFirstDialog = true;

    [Header("小鱼干是否全部集齐")]
    public static bool isFishAllCollected = false;

    // 每帧检测距离
    void Update()
    {
        // 计算 角色 和 Cat 的直线距离
        float distance = Vector3.Distance(transform.position, cat.transform.position);
        // 判断距离：小于设定值 → 显示文字，否则隐藏
        if (distance < triggerDistance)
        {
            tipText.enabled = true;
            // Debug.Log("距离小于3");
            // Debug.Log($"{tipText.enabled}");
            // 新增：按E键触发对话（判断对话未在播放、核心脚本和配置文件均绑定）
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null  && !dialogueCore.isDialoguePlaying)
            {
                Debug.Log("进行按E判定");
                if (isCatFirstDialog)// 首次对话
                {
                    // 调用核心脚本，启动Cat的对话（对接原有对话系统）
                    dialogueCore.StartDialogue(catStartDialogueData);
                    isCatFirstDialog = false;
                }
                else if(isFishAllCollected)//全部集齐
                {
                    dialogueCore.StartDialogue(catEndDialogueData);
                }
                else//非全部集齐
                {
                    Debug.Log("非全收集途中对话");
                    dialogueCore.StartDialogue(catHalfDialogueData);
                }
            }
        }

        else
        {
            tipText.enabled = false;
        }
    }
}