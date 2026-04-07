// 存放单句对话的内容
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [Header("对话文本内容")]
    [TextArea(2, 4)]
    public string dialogueContent;

    [Header("说话角色（为空则是旁白）")]
    public RoleInfo speaker;

    [Header("是否为旁白（勾选后隐藏所有角色）")]
    public bool isMonologue;
}