using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

public class CatDistanceCheck : MonoBehaviour
{
    [Header("拖拽设置")]
    public Transform cat; // 猫猫
    public Transform player; // 玩家对象（用于保存正确位置）
    public TextMeshProUGUI tipText; // 拖拽刚才创建的CatTip文字到这里
    public DialogueCore dialogueCore; // 新增：拖拽Hierarchy面板的DialogPanel（绑定了核心脚本）

    [Header("某个桥的全部触发点，前6个给鱼干，中间3是小游戏，后面3个不给")]
    public Transform[] bridgeTriggersTransform = new Transform[12];

    [Header("与猫的首次对话")]
    public DialogueDataSO catStartDialogueData;

    [Header("小于管收集途中对话")]
    public DialogueDataSO catHalfDialogueData;

    [Header("小鱼干全部收集完成对话")]
    public DialogueDataSO catEndDialogueData;

    [Header("提示与猫对话文字触发距离")]
    public float triggerDistance = 0.5f;

    [Header("是否为与猫的首次对话")]
    public static bool isCatFirstDialog = true;

    [Header("小鱼干是否全部集齐")]
    public static bool isFishAllCollected = false;

    private static float catDistance;

    private static float[] gameTriggersDistance = new float[12];


    private bool IsFishCollected(int index)
    {
        string sceneName = gameObject.scene.name;
        if (sceneName == UIControler.ThreeSceneName[1])
            return UIControler.bridge1fishes[index];
        else if (sceneName == UIControler.ThreeSceneName[2])
            return UIControler.bridge2fishes[index];
        else if (sceneName == UIControler.ThreeSceneName[3])
            return UIControler.bridge3fishes[index];
        return false;
    }

    // 获取提示文案（根据是否已收集）
    private string GetTipText(int fishIndex, bool isMiniGame = false)
    {
        if (IsFishCollected(fishIndex))
        {
            return "你已经得到这里的小鱼干啦~";
        }
        return isMiniGame ? "按E开始小游戏" : "按E进行交互";
    }

    // 获取当前桥的索引（1晋祠, 2赵州桥, 3广济桥）
    private int GetCurrentBridgeIndex()
    {
        string sceneName = gameObject.scene.name;
        if (sceneName == UIControler.ThreeSceneName[1]) return 1;
        if (sceneName == UIControler.ThreeSceneName[2]) return 2;
        if (sceneName == UIControler.ThreeSceneName[3]) return 3;
        return 1;
    }

    // 每帧检测距离
    private void Update()
    {
        // 计算各种距离
        var catDistance = Vector3.Distance(transform.position, cat.transform.position);
        for (int i = 0; i < 12; i++)
        {
            gameTriggersDistance[i] = Vector3.Distance(transform.position, bridgeTriggersTransform[i].position);
        }

        // 三种判定
        if (catDistance < triggerDistance)// 对话判定
        {
            tipText.text = "按E来和猫猫互动";
            UIControler.Show(GameObject.Find("FindTextPanel"));
            //UIControler.Hide(GameObject.Find("FindTextPanel"));
            // 新增：按E键触发对话（判断对话未在播放、核心脚本和配置文件均绑定）
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == UIControler.ThreeSceneName[1])
                {
                    if (isCatFirstDialog)// 首次对话
                    {
                        dialogueCore.StartDialogue(catStartDialogueData);
                        isCatFirstDialog = false;
                    }
                    else if (isFishAllCollected)//全部集齐
                    {
                        dialogueCore.StartDialogue(catEndDialogueData);
                    }
                    else//非全部集齐
                    {
                        dialogueCore.StartDialogue(catHalfDialogueData);
                    }
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[2])
                {
                    if (isCatFirstDialog)// 首次对话
                    {
                        dialogueCore.StartDialogue(catStartDialogueData);
                        isCatFirstDialog = false;
                    }
                    else if (isFishAllCollected)//全部集齐
                    {
                        dialogueCore.StartDialogue(catEndDialogueData);
                    }
                    else//非全部集齐
                    {
                        dialogueCore.StartDialogue(catHalfDialogueData);
                    }
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[3])
                {
                    if (isCatFirstDialog)// 首次对话
                    {
                        dialogueCore.StartDialogue(catStartDialogueData);
                        isCatFirstDialog = false;
                    }
                    else if (isFishAllCollected)//全部集齐
                    {
                        dialogueCore.StartDialogue(catEndDialogueData);
                    }
                    else//非全部集齐
                    {
                        dialogueCore.StartDialogue(catHalfDialogueData);
                    }
                }
            }
        }
        else if (gameTriggersDistance[0] < triggerDistance)// 收集和小游戏判定
        {
            tipText.text = GetTipText(0);
            UIControler.Show(GameObject.Find("FindTextPanel"));
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == UIControler.ThreeSceneName[1])
                {
                    UIControler.bridge1fishes[0] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(1, 1);
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[2])
                {
                    UIControler.bridge2fishes[0] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(2, 1);
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[3])
                {
                    UIControler.bridge3fishes[0] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(3, 1);
                }
            }

        }
        else if (gameTriggersDistance[1] < triggerDistance)
        {
            tipText.text = GetTipText(1);
            UIControler.Show(GameObject.Find("FindTextPanel"));
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == UIControler.ThreeSceneName[1])
                {
                    UIControler.bridge1fishes[1] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(1, 2);
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[2])
                {
                    UIControler.bridge2fishes[1] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(2, 2);
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[3])
                {
                    UIControler.bridge3fishes[1] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(3, 2);
                }
            }
        }
        else if (gameTriggersDistance[2] < triggerDistance)
        {
            tipText.text = GetTipText(2);
            UIControler.Show(GameObject.Find("FindTextPanel"));
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == UIControler.ThreeSceneName[1])
                {
                    UIControler.bridge1fishes[2] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(1, 3);
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[2])
                {
                    UIControler.bridge2fishes[2] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(2, 3);
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[3])
                {
                    UIControler.bridge3fishes[2] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(3, 3);
                }
            }

        }
        else if (gameTriggersDistance[3] < triggerDistance)
        {
            tipText.text = GetTipText(3);
            UIControler.Show(GameObject.Find("FindTextPanel"));
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == UIControler.ThreeSceneName[1])
                {
                    UIControler.bridge1fishes[3] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(1, 4);
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[2])
                {
                    UIControler.bridge2fishes[3] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(2, 4);
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[3])
                {
                    UIControler.bridge3fishes[3] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(3, 4);
                }
            }

        }
        else if (gameTriggersDistance[4] < triggerDistance)
        {
            tipText.text = GetTipText(4);
            UIControler.Show(GameObject.Find("FindTextPanel"));
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == UIControler.ThreeSceneName[1])
                {
                    UIControler.bridge1fishes[4] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(1, 5);
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[2])
                {
                    UIControler.bridge2fishes[4] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(2, 5);
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[3])
                {
                    UIControler.bridge3fishes[4] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(3, 5);
                }
            }

        }
        else if (gameTriggersDistance[5] < triggerDistance)
        {
            tipText.text = GetTipText(5);
            UIControler.Show(GameObject.Find("FindTextPanel"));
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == UIControler.ThreeSceneName[1])
                {
                    UIControler.bridge1fishes[5] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(1, 6);
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[2])
                {
                    UIControler.bridge2fishes[5] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(2, 6);
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[3])
                {
                    UIControler.bridge3fishes[5] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(3, 6);
                }
            }

        }
        // 小游戏判定
        else if (gameTriggersDistance[6] < triggerDistance)
        {
            Debug.Log($"进入小游戏触发范围：桥{GetCurrentBridgeIndex()}的第6个触发点");
            tipText.text = GetTipText(6, true);
            UIControler.Show(GameObject.Find("FindTextPanel"));
            if (Input.GetKeyDown(KeyCode.E))
            {
                //int bridgeIdx = GetCurrentBridgeIndex();
                //Debug.Log($"触发小游戏：桥{bridgeIdx}的第6个触发点（钓鱼）");
                //UIControler.SetPendingFish(bridgeIdx, 6);
                //UIControler.StartMiniGame(0, bridgeIdx); // 0=钓鱼

                if (gameObject.scene.name == UIControler.ThreeSceneName[1])
                {
                    UIControler.bridge1fishes[6] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(1, 7);
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[2])
                {
                    UIControler.bridge2fishes[6] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(2, 7);
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[3])
                {
                    UIControler.bridge3fishes[6] = true;
                    if (UIControler.Instance != null) UIControler.Instance.TriggerOpen(3, 7);
                }


            }
        }
        else if (gameTriggersDistance[7] < triggerDistance)
        {
            tipText.text = GetTipText(7, true);
            UIControler.Show(GameObject.Find("FindTextPanel"));
            if (Input.GetKeyDown(KeyCode.E))
            {
                int bridgeIdx = GetCurrentBridgeIndex();
                UIControler.SetPendingFish(bridgeIdx, 7);
                UIControler.StartMiniGame(1, bridgeIdx); // 1=拼图
            }
        }
        else if (gameTriggersDistance[8] < triggerDistance)
        {
            tipText.text = GetTipText(8, true);
            UIControler.Show(GameObject.Find("FindTextPanel"));
            if (Input.GetKeyDown(KeyCode.E))
            {
                int bridgeIdx = GetCurrentBridgeIndex();
                UIControler.SetPendingFish(bridgeIdx, 8);
                UIControler.StartMiniGame(2, bridgeIdx); // 2=问答
            }
        }
        // 无鱼干判定
        else if (gameTriggersDistance[9] < triggerDistance)
        {
            tipText.text = "按E进行交互";
            UIControler.Show(GameObject.Find("FindTextPanel"));
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == UIControler.ThreeSceneName[1])
                {
                    // dialogueCore.StartDialogue(catStartDialogueData);
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[2])
                {
                    // dialogueCore.StartDialogue(catStartDialogueData);

                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[3])
                {
                    // dialogueCore.StartDialogue(catStartDialogueData);

                }
            }
        }
        else if (gameTriggersDistance[10] < triggerDistance)
        {
            tipText.text = "按E进行交互";
            UIControler.Show(GameObject.Find("FindTextPanel"));
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == UIControler.ThreeSceneName[1])
                {
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[2])
                {
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[3])
                {
                }
            }
        }
        else if (gameTriggersDistance[11] < triggerDistance)
        {
            tipText.text = "按E进行交互";
            UIControler.Show(GameObject.Find("FindTextPanel"));
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == UIControler.ThreeSceneName[1])
                {
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[2])
                {
                }
                else if (gameObject.scene.name == UIControler.ThreeSceneName[3])
                {
                }
            }
        }
        else
        {
            UIControler.Hide(GameObject.Find("FindTextPanel"));
        }
    }


}