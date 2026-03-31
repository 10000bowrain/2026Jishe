// UI控制：负责刷新角色头像、名称、对话文本，控制UI显隐
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    // 绑定左侧角色相关UI（拖拽对应组件）
    [Header("左侧角色UI")]
    public Image leftRoleHead;       // 左侧角色头像
    public Text leftRoleName;        // 左侧角色名称
    //public TextMeshPro leftRoleName; // 左侧角色名称
    public GameObject leftRoleArea;  // 左侧角色总区域（控制显隐）

    // 绑定右侧角色相关UI（拖拽对应组件）
    [Header("右侧角色UI")]
    public Image rightRoleHead;      // 右侧角色头像
    public Text rightRoleName;       // 右侧角色名称
    //public TextMeshPro rightRoleName;       // 右侧角色名称
    public GameObject rightRoleArea; // 右侧角色总区域（控制显隐）

    // 绑定对话文本UI
    [Header("对话文本UI")]
    public Text dialogText;          // 居中对话文本
    public GameObject dialogPanel;   // 对话总容器（控制整体显隐）

    // 初始化UI：对话开始时调用，重置所有UI状态
    public void InitDialogueUI()
    {
        // 显示对话总容器
        dialogPanel.SetActive(true);
        // 隐藏左右角色区域（默认隐藏，说话时再显示）
        leftRoleArea.SetActive(false);
        rightRoleArea.SetActive(false);
        // 清空对话文本
        dialogText.text = "";
    }

    // 刷新单句对话UI（根据当前对话数据，显示角色/旁白）
    public void RefreshDialogueUI(DialogueLine dialogueLine)
    {
        // 1. 刷新对话文本
        dialogText.text = dialogueLine.dialogueContent;
        //Debug.Log($"dialogueLine.isMonologue = {dialogueLine.isMonologue} ");
        //Debug.Log($"dialogueLine.speaker == null : {dialogueLine.speaker == null}");
        // 2. 判断是否为旁白：是则隐藏所有角色，否则显示对应角色
        if (dialogueLine.isMonologue || dialogueLine.speaker == null)
        {
            leftRoleArea.SetActive(false);
            rightRoleArea.SetActive(false);
        }

        else
        {
            //Debug.Log("不是旁白");
            //Debug.Log($"{leftRoleArea.activeSelf}");
            //Debug.Log($"{rightRoleArea.activeSelf}");

            // 根据角色显示位置，刷新对应侧的UI
            if (dialogueLine.speaker.rolePosition == RolePosition.Left)
            {
                // 显示左侧角色，隐藏右侧角色
                leftRoleArea.SetActive(true);
                rightRoleArea.SetActive(false);
                // 赋值左侧角色头像和名称
                leftRoleHead.sprite = dialogueLine.speaker.roleHead;
                leftRoleName.text = dialogueLine.speaker.roleName;
                // 避免头像拉伸（可选优化）
                leftRoleHead.preserveAspect = true;
            }
            else if (dialogueLine.speaker.rolePosition == RolePosition.Right)
            {
                // 显示右侧角色，隐藏左侧角色
                rightRoleArea.SetActive(true);
                leftRoleArea.SetActive(false);
                // 赋值右侧角色头像和名称
                rightRoleHead.sprite = dialogueLine.speaker.roleHead;
                rightRoleName.text = dialogueLine.speaker.roleName;
                // 避免头像拉伸（可选优化）
                rightRoleHead.preserveAspect = true;
            }
        }
    }

    // 对话结束时调用：隐藏对话总容器
    public void CloseDialogueUI()
    {
        dialogPanel.SetActive(false);
        leftRoleArea.SetActive(false);
        rightRoleArea.SetActive(false);
    }
}