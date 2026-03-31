// 对话核心逻辑：串联所有脚本，控制对话启动、逐句播放、点击切句、结束
using UnityEngine;
using UnityEngine.InputSystem;
public class DialogueCore : MonoBehaviour
{
    // 玩家输入组件（拖拽玩家对象即可）
    public PlayerInput playerInput;
    // 玩家移动组件（根据自己的玩家脚本修改，比如CharacterController、Rigidbody）
    public CharacterController playerMove; // 若用Rigidbody，改
    // 引用其他脚本（自动获取，无需手动拖拽）
    private DialogueDataManager dataManager;
    private DialogueUIController uiController;

    // 当前正在播放的对话配置文件
    private DialogueDataSO currentDialogueData;

    // 对话是否正在播放（防止重复点击）
    public bool isDialoguePlaying = false;

    private void Awake()
    {
        // 自动获取同物体上的其他脚本，无需手动绑定
        dataManager = GetComponent<DialogueDataManager>();
        uiController = GetComponent<DialogueUIController>();
    }

    // 外部调用：启动对话（后续触发对话时调用，比如点击NPC）
    public void StartDialogue(DialogueDataSO dialogueData)
    {
        // 解放鼠标
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isDialoguePlaying = true;
        currentDialogueData = dialogueData;
        dataManager.LoadDialogue(dialogueData);
        uiController.InitDialogueUI();
        PlayNextDialogueLine();

        // 新增：冻结玩家输入和移动（核心代码）
        if (playerInput != null)
        {
            playerInput.enabled = false; // 禁用玩家输入
        }
        if (playerMove != null)
        {
            playerMove.enabled = false; // 禁用玩家移动（若用Rigidbody，改为playerMove.velocity = Vector3.zero; playerMove.isKinematic = true;）
        }
    }

    // 播放下一句对话（鼠标点击、自动切换时调用）
    public void PlayNextDialogueLine()
    {
        // 获取下一句对话
        DialogueLine nextLine = dataManager.GetNextDialogueLine();
        if (nextLine != null)
        {
            //Debug.Log($"{nextLine.dialogueContent}");
            // 有下一句，刷新UI显示
            uiController.RefreshDialogueUI(nextLine);
        }
        else
        {
            // 没有下一句，对话结束
            EndDialogue();
        }
    }

    // 对话结束：重置状态、关闭UI
    private void EndDialogue()
    {
        isDialoguePlaying = false;
        currentDialogueData = null;
        uiController.CloseDialogueUI();
        //禁用鼠标
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // 新增：恢复玩家输入和移动（核心代码）
        if (playerInput != null)
        {
            playerInput.enabled = true;
        }
        if (playerMove != null)
        {
            playerMove.enabled = true; // 若用Rigidbody，改为playerMove.isKinematic = false;
        }
    }

    // 跳过当前对话（绑定跳过按钮）
    public void SkipDialogue()
    {
        if (isDialoguePlaying)
        {
            // 清空对话队列，直接结束对话
            dataManager.SkipCurrentDialogue();
            EndDialogue();
        }
    }

    // 监听鼠标左键点击（每点击一次，播放下一句）
    private void Update()
    {
        // 只有对话正在播放时，点击才有效
        if (isDialoguePlaying && Input.GetMouseButtonDown(0))
        {
            // 鼠标左键点击（0代表左键），播放下一句
            PlayNextDialogueLine();
        }
        Debug.Log($"{Cursor.visible}");
        Debug.Log($"{Cursor.lockState}");
    }
}