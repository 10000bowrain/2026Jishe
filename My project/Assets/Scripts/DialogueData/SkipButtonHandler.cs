// 跳过按钮交互：绑定按钮点击事件，调用核心脚本的跳过方法
using UnityEngine;
using UnityEngine.UI;

public class SkipButtonHandler : MonoBehaviour
{
    // 引用对话核心脚本（拖拽DialogPanel即可）
    public DialogueCore dialogueCore;

    private void Awake()
    {
        // 自动绑定按钮点击事件，无需手动在Inspector面板设置
        GetComponent<Button>().onClick.AddListener(OnSkipButtonClick);
    }

    // 跳过按钮点击事件
    private void OnSkipButtonClick()
    {
        // 调用核心脚本的跳过方法，结束当前对话
        if (dialogueCore != null)
        {
            dialogueCore.SkipDialogue();
        }
    }
}