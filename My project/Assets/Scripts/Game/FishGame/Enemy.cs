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
        myCollider = GetComponent<Collider>();
        manager = FindObjectOfType<Event>();
        if (manager != null)
            manager.RegisterCollider(myCollider);
    }
    void OnDisable()
    {
        if (manager != null)
            manager.UnregisterCollider(myCollider);
    }
}
