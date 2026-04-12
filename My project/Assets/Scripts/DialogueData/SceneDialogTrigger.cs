// 场景加载完成后，自动触发对话
using UnityEngine;

public class SceneDialogTrigger : MonoBehaviour
{
    public DialogueDataSO dialogueData;
    public DialogueCore dialogueCore;
    public static bool isFirstEnterGame = true;
    public static bool kaishi = true;
    // 场景加载完成后自动执行
    private void Start()
    {
        if (dialogueData != null && !dialogueCore.isDialoguePlaying && isFirstEnterGame)
        {
            //Debug.Log("触发开场对话");
            dialogueCore.StartDialogue(dialogueData);
            isFirstEnterGame = false;
        }
        else
        {
            FindObjectOfType<DialogueUIController>().CloseDialogueUI();
        }
    }
    public void Update()
    {
        //Debug.Log($"dialogueData != null:{dialogueData != null}");
        //Debug.Log($"!dialogueCore.isDialoguePlaying:{!dialogueCore.isDialoguePlaying}");

    }
}