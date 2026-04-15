using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneJump : MonoBehaviour
{
    public string nextSceneName = "NextScene";//下一个场景的名字

    public void Scene()
    { 
        if(gameObject.scene.name == "Fish")
        {
            
        }
        SceneManager.LoadScene(nextSceneName);
    }
}
