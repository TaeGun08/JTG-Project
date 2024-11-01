using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialSceneLoad : MonoBehaviour
{
    private float sceneChangeTimer;

    private void Update()
    {
        sceneChangeTimer += Time.deltaTime;

        if (sceneChangeTimer > 1)
        {
            SceneManager.LoadSceneAsync(1);
            sceneChangeTimer = 0;
        }
    }
}
