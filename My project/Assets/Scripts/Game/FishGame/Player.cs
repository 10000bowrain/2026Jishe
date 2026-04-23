using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;

    [Header("BackWay")]
    [SerializeField] private bool getBack;//钩爪检测后进入到回拉状态
    [SerializeField] private float Force;//上拉的力气
    [SerializeField] private bool getInPlay;//进入钩爪状态
    [SerializeField] private Vector3 originalPosition;//获取敌人的初始位置
    [SerializeField] private bool getDetected;//检测是否钩爪到位

    [Header("CollisionDetected")]
    [SerializeField] private float length;//下坠长度
    [SerializeField] private float dashSpeed;//下坠速度
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform dash;//钩爪位置
    [SerializeField] private bool canMove;//移动检测
    [SerializeField] private GameObject DetectedGameObject;//钩爪碰撞的敌人
    [SerializeField] private float currentDashY;//当前钩爪的Y坐标

    private void Start()
    {
        dash.localPosition = new Vector3(0, 0, 0);
        canMove = true;//检测移动
        length = 0;
        currentDashY = 0;
        
        // 确保Layer 6（Player/钩爪）和Layer 7（鱼）能够碰撞
        Physics.IgnoreLayerCollision(6, 7, false);
    }

    private void Update()
    {
        HandleMove();
        HandleCollision();  
    }

    // 物理碰撞检测（两个物体都有BoxCollider且IsTrigger=false）
    private void OnCollisionEnter(Collision collision)
    {
        // 检查碰撞对象的Layer（鱼在Layer 7）
        if (collision.gameObject.layer == 7 && getInPlay && !getBack)
        {
            // 立即停止下坠
            DetectedGameObject = collision.gameObject;
            originalPosition = DetectedGameObject.transform.position;
            getBack = true;
            getInPlay = false;
            
            Debug.Log("钩爪碰到鱼: " + collision.gameObject.name);
        }
    }

    private void HandleCollision()
    {
        // 鼠标左键按下 - 开始下坠
        if (Input.GetKeyDown(KeyCode.Mouse0) && canMove)
        {
            getInPlay = true;
            canMove = false;
            getBack = false;
            length = 0;
            currentDashY = 0;
            DetectedGameObject = null;
        }

        // 钩爪下坠状态
        if (getInPlay)
        {
            float step = dashSpeed * Time.deltaTime;
            length += step;
            currentDashY -= step;
            dash.localPosition = new Vector3(0, currentDashY, 0);
        }

        // 钩住鱼后的下坠（鱼跟着钩爪下沉）
        if (DetectedGameObject != null && getBack && !getInPlay)
        {
            if (DetectedGameObject.transform.position.y > originalPosition.y)
            {
                float step = dashSpeed * Time.deltaTime * 0.5f;
                currentDashY -= step;
                dash.localPosition = new Vector3(0, currentDashY, 0);
                DetectedGameObject.transform.position = new Vector3(
                    DetectedGameObject.transform.position.x, 
                    DetectedGameObject.transform.position.y - step, 
                    0);
            }
        }

        // 按空格上拉
        if (getBack && DetectedGameObject != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentDashY += Force;
                dash.localPosition = new Vector3(dash.localPosition.x, currentDashY, 0);
                DetectedGameObject.transform.position = new Vector3(
                    DetectedGameObject.transform.position.x, 
                    DetectedGameObject.transform.position.y + Force, 
                    0);
            }
        }

        // 鱼被拉上来到达顶部 - 重置状态
        if (DetectedGameObject != null && DetectedGameObject.transform.position.y > 0)
        {
            ResetFishing();
        }
    }

    private void ResetFishing()
    {
        Debug.Log("钓鱼成功!");
        
        length = 0;
        currentDashY = 0;
        canMove = true;
        getInPlay = false;
        getBack = false;
        
        // 隐藏鱼
        if (DetectedGameObject != null)
        {
            DetectedGameObject.SetActive(false);
            DetectedGameObject = null;
        }
        
        // 重置钩爪位置
        dash.localPosition = new Vector3(0, 0, 0);
    }

    private void HandleMove()
    {
        if(canMove)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector3(horizontal * moveSpeed, 0, 0);
        }
        else
        {
            rb.velocity = new Vector3(0, 0, 0);
        }
    }
}
