using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.ComponentModel.Design;

public class CatDistanceCheck : MonoBehaviour
{
    [Header("拖拽设置")]
    public Transform cat; // 猫猫
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
    public float triggerDistance = 3f;

    [Header("是否为与猫的首次对话")]
    public static bool isCatFirstDialog = true;

    [Header("小鱼干是否全部集齐")]
    public static bool isFishAllCollected = false;

    private static float catDistance;

    private static float[] gameTriggersDistance = new float[12];

    // 每帧检测距离
    void Update()
    {
        // 计算各种距离
        catDistance = Vector3.Distance(transform.position, cat.transform.position);
        for (int i = 0; i < 12; i++)
        {
            gameTriggersDistance[i] = Vector3.Distance(transform.position, bridgeTriggersTransform[i].position);
        }

        // 三种判定
        if (catDistance < triggerDistance)// 对话判定
        {
            tipText.text = "按E来和猫猫互动";
            tipText.enabled = true;
            // 新增：按E键触发对话（判断对话未在播放、核心脚本和配置文件均绑定）
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == "合并成功-晋祠")
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
                else if (gameObject.scene.name == "测试-赵州桥")
                {

                }
                else if (gameObject.scene.name == "")
                {

                }
            }
        }
        else if (gameTriggersDistance[0] < triggerDistance)// 收集和小游戏判定
        {
            tipText.text = "按E进行交互";
            tipText.enabled = true;
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == "合并成功-晋祠")
                {
                    UIControler.bridge1fishes[0] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(1, 1);
                }
                else if (gameObject.scene.name == "测试-赵州桥")
                {
                    UIControler.bridge2fishes[0] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(2, 1);
                }
                else if (gameObject.scene.name == "")
                {
                    UIControler.bridge3fishes[0] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(3, 1);
                }
            }

        }
        else if (gameTriggersDistance[1] < triggerDistance)
        {
            tipText.text = "按E进行交互";
            tipText.enabled = true;
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == "合并成功-晋祠")
                {
                    UIControler.bridge1fishes[1] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(1, 2);
                }
                else if (gameObject.scene.name == "测试-赵州桥")
                {
                    UIControler.bridge2fishes[1] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(2, 2);
                }
                else if (gameObject.scene.name == "")
                {
                    UIControler.bridge3fishes[1] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(3, 2);
                }
            }
        }
        else if (gameTriggersDistance[2] < triggerDistance)
        {
            tipText.text = "按E进行交互";
            tipText.enabled = true;
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == "合并成功-晋祠")
                {
                    UIControler.bridge1fishes[2] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(1, 3);
                }
                else if (gameObject.scene.name == "测试-赵州桥")
                {
                    UIControler.bridge2fishes[2] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(2, 3);
                }
                else if (gameObject.scene.name == "")
                {
                    UIControler.bridge3fishes[2] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(3, 3);
                }
            }

        }
        else if (gameTriggersDistance[3] < triggerDistance)
        {
            tipText.text = "按E进行交互";
            tipText.enabled = true;
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == "合并成功-晋祠")
                {
                    UIControler.bridge1fishes[3] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(1, 4);
                }
                else if (gameObject.scene.name == "测试-赵州桥")
                {
                    UIControler.bridge2fishes[3] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(2, 4);
                }
                else if (gameObject.scene.name == "")
                {
                    UIControler.bridge3fishes[3] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(3, 4);
                }
            }

        }
        else if (gameTriggersDistance[4] < triggerDistance)
        {
            tipText.text = "按E进行交互";
            tipText.enabled = true;
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == "合并成功-晋祠")
                {
                    UIControler.bridge1fishes[4] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(1, 5);
                }
                else if (gameObject.scene.name == "测试-赵州桥")
                {
                    UIControler.bridge2fishes[4] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(2, 5);
                }
                else if (gameObject.scene.name == "")
                {
                    UIControler.bridge3fishes[4] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(3, 5);
                }
            }

        }
        else if (gameTriggersDistance[5] < triggerDistance)
        {
            tipText.text = "按E进行交互";
            tipText.enabled = true;
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == "合并成功-晋祠")
                {
                    UIControler.bridge1fishes[5] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(1, 6);
                }
                else if (gameObject.scene.name == "测试-赵州桥")
                {
                    UIControler.bridge2fishes[5] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(2, 6);
                }
                else if (gameObject.scene.name == "")
                {
                    UIControler.bridge3fishes[5] = true;
                    GameObject.Find("UIControler").GetComponent<UIControler>().TriggerOpen(3, 6);
                }
            }

        }
        else if (gameTriggersDistance[6] < triggerDistance)// 小游戏判定
        {
            tipText.text = "按E进行交互";
            tipText.enabled = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (gameObject.scene.name == "合并成功-晋祠")
                {
                    SceneFade.Instance.LoadScene("Fish");
                }
                else if (gameObject.scene.name == "测试-赵州桥")
                {
                    SceneFade.Instance.LoadScene("Puzzle");
                }
                else if (gameObject.scene.name == "")
                {
                    SceneFade.Instance.LoadScene("Question");
                }
            }
        }
        else if (gameTriggersDistance[7] < triggerDistance)
        {
            tipText.text = "按E进行交互";
            tipText.enabled = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (gameObject.scene.name == "合并成功-晋祠")
                {
                    SceneFade.Instance.LoadScene("Fish");
                }
                else if (gameObject.scene.name == "测试-赵州桥")
                {
                    SceneFade.Instance.LoadScene("Puzzle");
                }
                else if (gameObject.scene.name == "")
                {
                    SceneFade.Instance.LoadScene("Question");
                }
            }
        }
        else if (gameTriggersDistance[8] < triggerDistance)
        {
            tipText.text = "按E进行交互";
            tipText.enabled = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (gameObject.scene.name == "合并成功-晋祠")
                {
                    SceneFade.Instance.LoadScene("Fish");
                }
                else if (gameObject.scene.name == "测试-赵州桥")
                {
                    SceneFade.Instance.LoadScene("Puzzle");
                }
                else if (gameObject.scene.name == "")
                {
                    SceneFade.Instance.LoadScene("Question");
                }
            }
        }
        else if (gameTriggersDistance[9] < triggerDistance)
        {
            tipText.text = "按E进行交互";
            tipText.enabled = true;
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == "合并成功-晋祠")
                {
                    // dialogueCore.StartDialogue(catStartDialogueData);
                }
                else if (gameObject.scene.name == "测试-赵州桥")
                {
                    // dialogueCore.StartDialogue(catStartDialogueData);

                }
                else if (gameObject.scene.name == "")
                {
                    // dialogueCore.StartDialogue(catStartDialogueData);

                }
            }
        }
        else if (gameTriggersDistance[10] < triggerDistance)
        {
            tipText.text = "按E进行交互";
            tipText.enabled = true;
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == "合并成功-晋祠")
                {
                }
                else if (gameObject.scene.name == "测试-赵州桥")
                {
                }
                else if (gameObject.scene.name == "")
                {
                }
            }
        }
        else if (gameTriggersDistance[11] < triggerDistance)
        {
            tipText.text = "按E进行交互";
            tipText.enabled = true;
            if (Input.GetKeyDown(KeyCode.E) && dialogueCore != null && !dialogueCore.isDialoguePlaying)
            {
                if (gameObject.scene.name == "合并成功-晋祠")
                {
                }
                else if (gameObject.scene.name == "测试-赵州桥")
                {
                }
                else if (gameObject.scene.name == "")
                {
                }
            }
        }
        else
        {
            tipText.enabled = false;
        }
    }
}