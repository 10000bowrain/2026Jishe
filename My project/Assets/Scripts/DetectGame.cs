using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectGame : MonoBehaviour
{
    public GameObject gameEnd;

    private void Start()
    {
        if (gameEnd)
        { 
            SaveData();
            gameEnd = null;
        }
    }



    void SaveData()
    {
        // 通过单例访问，存储数据
        //Score.CollectScore += 1;
    }
}
