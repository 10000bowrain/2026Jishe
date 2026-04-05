using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.UI.Image;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;


    [Header("CollisionDetected")]
    [SerializeField] private float length;
    [SerializeField] private float dashSpeed;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform dash;
    [SerializeField] private bool isFalling;
    [SerializeField] private bool back;
    [SerializeField] private bool canMove;
    [SerializeField] private bool isDetected;
    [SerializeField] private GameObject DetectedGameObject;
    [SerializeField]RaycastHit hit;
    [SerializeField] private LayerMask enemy;

    private void Start()
    {
        dash.localPosition = new Vector3(0, 0, 0);
        canMove = true;
    }

    private void Update()
    {
        HandleMove();
        HandleCollision();
    }

    private void HandleCollision()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, length, enemy))
        {
            DetectedGameObject = hit.collider.gameObject;
            DetectedGameObject.SetActive(false);
            isDetected = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isFalling = true;
            canMove = false;
        }

        if (isFalling == true && isDetected == false)
        {
            float step = dashSpeed * Time.deltaTime;
            length += step;
        }

        if (isDetected)
        {
            isFalling = false;
            back=true;
        }

        if(back)
        {
            float step = dashSpeed * Time.deltaTime;
            length -= step;
            isDetected = false;
        }

        dash.localPosition = new Vector3(0, -length, 0);

        if (length < 0)
        {
            length = 0;
            back=false;
            canMove=true;
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
