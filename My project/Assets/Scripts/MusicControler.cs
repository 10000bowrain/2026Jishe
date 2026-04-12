using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControler : MonoBehaviour
{

    [Header("不要把任何对象拖到这里")]
    public AudioSource[] musicList = new AudioSource[musicNum];

    [Header("把音乐文件拖到这里")]
    public AudioClip[] addMusicHere = new AudioClip[musicNum];

    [Header("全部音乐音效的数量")]
    public static int musicNum = 10;

    void Start()
    {
        // 初始化音乐
        for (int i = 0; i < musicNum; i++)
        {
            musicList[i] = gameObject.AddComponent<AudioSource>();
            musicList[i].clip = addMusicHere[i];
        }
        // 这几行控制一进入游戏就播放的循环主音乐
        musicList[0].loop = true;
        musicList[0].volume = 0.5f; // 这里默认0.5f，不再定义MainMusicVolume变量
        musicList[0].Play();
        DontDestroyOnLoad(GameObject.Find("MusicControler"));
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
