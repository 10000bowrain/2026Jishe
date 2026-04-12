// 对话数据管理：读取配置文件、整理对话队列，供其他脚本调用
using System.Collections.Generic;
using UnityEngine;

public class DialogueDataManager : MonoBehaviour
{
    // 存储当前正在播放的对话队列（逐句存储）
    private Queue<DialogueLine> currentDialogueQueue = new Queue<DialogueLine>();

    // 外部调用：加载指定的对话配置文件，初始化对话队列
    public void LoadDialogue(DialogueDataSO dialogueData)
    {
        // Debug.Log("进入LoadDialogue方法");
        // 清空之前的对话队列，避免重复播放
        currentDialogueQueue.Clear();

        // 将配置文件中的对话，逐句加入队列（按配置顺序）
        foreach (DialogueLine line in dialogueData.dialogueLines)
        {
            currentDialogueQueue.Enqueue(line);
        }
        // Debug.Log("退出LoadDialogue方法");

    }

    // 外部调用：获取下一句对话（鼠标点击切句时使用）
    public DialogueLine GetNextDialogueLine()
    {
        // 判断队列是否有下一句，有则返回，无则返回空（对话结束）
        if (currentDialogueQueue.Count > 0)
        {
            return currentDialogueQueue.Dequeue();
        }
        else
        {
            return null;
        }
    }

    // 外部调用：判断对话是否播放完毕
    public bool IsDialogueEnd()
    {
        return currentDialogueQueue.Count == 0;
    }

    // 外部调用：跳过当前对话（清空队列）
    public void SkipCurrentDialogue()
    {
        currentDialogueQueue.Clear();
    }
}