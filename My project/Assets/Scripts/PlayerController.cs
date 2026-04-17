using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float gravity = -9.81f;           // 重力加速度
    private float verticalVelocity;           // 垂直方向速度（用于重力）

    private CharacterController controller;
    private Animator animator;
    private bool isWalking;
    private bool isRight;
    private bool isUp;
    private bool isBack;

    void Start()
    {
        Set();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        // Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. 获取输入（与原代码符号逻辑一致）
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 计算移动方向（注意：与原 velocity 赋值方向相同）
        Vector3 moveDirection = new Vector3(-horizontal, 0, -vertical).normalized;

        // 2. 处理重力
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;      // 小负值让角色紧贴地面
        else
            verticalVelocity += gravity * Time.deltaTime;

        // 3. 计算最终位移（速度 × 时间）
        Vector3 move = moveDirection * moveSpeed * Time.deltaTime;
        move.y = verticalVelocity * Time.deltaTime;

        // 4. 移动角色
        controller.Move(move);

        // 5. 动画控制（基于实际移动方向，而不是 velocity）
        UpdateAnimation(moveDirection);
    }

    private void UpdateAnimation(Vector3 moveDir)
    {
        // 根据移动方向的分量设置动画布尔值（与原逻辑保持一致）
        if (moveDir.z > 0.1f)          // 向后（对应原 velocity.z > 0）
        {
            isWalking = true;
            isBack = true;
            isRight = false;
            isUp = false;
        }
        else if (moveDir.z < -0.1f)    // 向前（对应原 velocity.z < 0）
        {
            isWalking = true;
            isUp = true;
            isRight = false;
            isBack = false;
        }
        else if (moveDir.x > 0.1f)     // 向右（对应原 velocity.x > 0）
        {
            // 旋转角色朝向（保持原逻辑：向右时旋转到 (0,1,0,0)）
            transform.rotation = new Quaternion(0, 1, 0, 0);
            isWalking = true;
            isRight = true;
            isUp = false;
            isBack = false;
        }
        else if (moveDir.x < -0.1f)    // 向左（对应原 velocity.x < 0）
        {
            // 向左时旋转到 (0,0,0,1)
            transform.rotation = new Quaternion(0, 0, 0, 1);
            isWalking = true;
            isRight = true;
            isUp = false;
            isBack = false;
        }
        else
        {
            // 无移动
            isWalking = false;
            isRight = false;
            isUp = false;
            isBack = false;
        }

        // 更新动画参数
        GetBool();
    }

    private void Set()
    {
        isWalking = false;
        isRight = false;
        isUp = false;
        isBack = false;
    }

    private void GetBool()
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRight", isRight);
        animator.SetBool("isUp", isUp);
        animator.SetBool("isBack", isBack);
    }
}