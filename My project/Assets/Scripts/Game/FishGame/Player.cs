using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.UI.Image;

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
    [SerializeField] RaycastHit hit;
    [SerializeField] private LayerMask enemy;//检测面板


    private void Start()
    {
        dash.localPosition = new Vector3(0, 0, 0);
        canMove = true;//检测移动
    }

    private void Update()
    {

        HandleMove();
        HandleCollision();  
    }

    private void HandleCollision()
    {
        if (getDetected) 
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hit, length, enemy)) //检测敌人位置
            {
                DetectedGameObject = hit.collider.gameObject;
                originalPosition = DetectedGameObject.transform.position;
                getBack = true;
                getDetected = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))//鼠标左键
        {
            getInPlay=true;//进入钩爪下落状态
            canMove = false;//玩家停止移动
            getDetected = true;//进入检测状态
        }

        if (getInPlay&&getBack==false)//钩爪下落状态
        {
            float step = dashSpeed * Time.deltaTime;
            length += step;//step连续增加 控制钩爪伸长的长度
            dash.localPosition = new Vector3(0, -length, 0);
        }

        if(DetectedGameObject!=null)//钩爪检测到之后的下坠逻辑
        {
            if (DetectedGameObject.transform.position.y >originalPosition.y)
            {
                length = 0;
                float step = dashSpeed * Time.deltaTime;
                length += step;
                dash.localPosition = new Vector3(0, dash.localPosition.y- length, 0);
                DetectedGameObject.transform.position = new Vector3(DetectedGameObject.transform.position.x, DetectedGameObject.transform.position.y- length, 0);
            }
        }



        if (getBack)
        {
            if (Input.GetKeyDown(KeyCode.Space))//上拉
            {
                dash.localPosition = new Vector3(dash.localPosition.x, dash.localPosition.y+Force, 0);
                DetectedGameObject.transform.position = new Vector3(DetectedGameObject.transform.position.x, DetectedGameObject.transform.position.y + Force, 0); 
            }
        }


        if (DetectedGameObject !=null&&DetectedGameObject.transform.position.y>0)//物体被拉住时的状态重置
        {
            length = 0;
            canMove=true;
            getInPlay = false ;
            getBack = false ;
            DetectedGameObject.SetActive(false);
            DetectedGameObject = null;
        }
    }

    private void HandleMove()
    {
        if(canMove)
        {
        float horizontal = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector3(horizontal * moveSpeed, 0, 0);
        }

        if(!canMove)
        {
            rb.velocity = new Vector3(0, 0, 0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -length));//画线 第一个是起点 第二个是终点  其作用其实就是一个辅助线的作用
    }
}
