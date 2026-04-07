using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionEvent : MonoBehaviour
{
    public GameObject EndUI;

    private void Update()
    {
        Debug.Log(Score.CollectScore);
        if(Score.CollectScore==1)
            EndUI.SetActive(true);
    }
}
