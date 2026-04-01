using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//如果制作了UI弹窗
using UnityEngine.SceneManagement;//用于场景管理  

public class UI : MonoBehaviour
{
    public bool useClick = true;//是否使用点击
    public bool useCollision = false;//是否使用碰撞
    public bool goToNextScene = false;//是否进入下一个脚本
    public string nextSceneName = "NextScene";//下一个场景的名字
    public GameObject popup;//弹窗的GameObject

    private void Update()
    {
        if (useClick && Input.GetMouseButtonDown(0))//检测鼠标
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                HandleInteraction();
            }
        }


        if(useCollision)
        {
            //碰撞检测代码
        }

    }


    private void OnCollisionEnter2D(Collision2D  collision)
    {
        if(useCollision&&collision.gameObject.CompareTag("Player"))//假设玩家有Player标签
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        if(popup!=null)
            popup.SetActive(true);//显示弹窗

        if (goToNextScene)
            SceneManager.LoadScene(nextSceneName);//加载下一个场景

    }
}
