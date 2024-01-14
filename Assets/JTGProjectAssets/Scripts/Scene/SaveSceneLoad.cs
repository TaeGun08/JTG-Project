using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSceneLoad : MonoBehaviour
{
    private float sceneChangeTimer;

    private void Update()
    {
        sceneChangeTimer += Time.deltaTime;

        if (sceneChangeTimer > 1)
        {
            int nextLevel = JsonConvert.DeserializeObject<int>(PlayerPrefs.GetString("saveKey"));
            SceneManager.LoadSceneAsync(nextLevel);
            sceneChangeTimer = 0;
        }
    }
}
