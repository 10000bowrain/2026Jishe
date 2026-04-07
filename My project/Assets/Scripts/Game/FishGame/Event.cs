using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Event : MonoBehaviour
{
    [SerializeField]private List<Collider> collidersInRange = new List<Collider>();
    [SerializeField] private GameObject Exit;
    [SerializeField] private GameObject EndEvent;

    public void RegisterCollider(Collider col)
    {
        if (!collidersInRange.Contains(col))
            collidersInRange.Add(col);
    }

    public void UnregisterCollider(Collider col)
    {
        if (collidersInRange.Contains(col))
            collidersInRange.Remove(col);
    }

    private void Update()
    {
        if(collidersInRange.Count==0)
        {
            Exit.SetActive(true);
            EndEvent.SetActive(true);
        }
    }
}
