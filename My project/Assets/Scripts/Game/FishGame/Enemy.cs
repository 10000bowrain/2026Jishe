using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsDetected;
    [SerializeField] private Collider myCollider;
    [SerializeField] private Event manager;

    void Start()
    {
        myCollider = GetComponent<Collider>();//获取碰撞
        manager = FindObjectOfType<Event>();
        if (manager != null)
            manager.RegisterCollider(myCollider);//将现有敌人的碰撞加入数组
    }
    void OnDisable()
    {
        if (manager != null)
            manager.UnregisterCollider(myCollider);//注销敌人的碰撞
    }
}
