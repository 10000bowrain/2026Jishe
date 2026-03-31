using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    private CharacterController controller;
    private float gravity = -9.81f;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. 处理移动输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection.Normalize();

        // 2. 重力与贴地核心逻辑
        velocity.y += gravity * Time.deltaTime;

        // 关键：角色在地面时，给一个向下的小力，确保紧贴地面
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 让角色“压”在地面上，跟随地形起伏
        }

        //// 3. 跳跃逻辑（可选）
        //if (Input.GetButtonDown("Jump") && controller.isGrounded)
        //{
        //    velocity.y = Mathf.Sqrt(2f * -gravity * 1f); // 跳跃高度约1米
        //}

        // 4. 应用移动：水平移动 + 垂直重力/跳跃
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);

        // 5. 视角旋转
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
    }
}