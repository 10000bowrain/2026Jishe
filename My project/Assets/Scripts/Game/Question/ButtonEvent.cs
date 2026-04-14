using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvent : MonoBehaviour
{
    public GameObject EndCanvas;
    public string nextSceneName = "NextScene";//下一个场景的名字

    public void ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SceneJump()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
