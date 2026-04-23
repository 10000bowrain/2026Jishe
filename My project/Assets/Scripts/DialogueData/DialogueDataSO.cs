// 对话配置文件，可在编辑器右键直接创建
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueDataSO : ScriptableObject
{
    [Header("这段对话的名称（方便区分）")]
    public string dialogueTitle;

    [Header("对话内容列表（按顺序填写）")]
    public List<DialogueLine> dialogueLines;
}