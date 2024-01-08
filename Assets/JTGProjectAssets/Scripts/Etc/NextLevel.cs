using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    private KeyManager keyManager;

    [SerializeField] private GameObject homeInKeyImage;
    private bool homeIn = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            homeIn = true;

            homeInKeyImage.SetActive(true);

            if (Input.GetKeyDown(keyManager.InteractionKey()) && homeIn == true)
            {
                int nextLevel = JsonConvert.DeserializeObject<int>(PlayerPrefs.GetString("saveKey"));
                nextLevel++;
                SceneManager.LoadSceneAsync(nextLevel);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            homeIn = false;

            homeInKeyImage.SetActive(false);
        }
    }

    private void Start()
    {
        keyManager = KeyManager.instance;

        homeInKeyImage.SetActive(false);
    }
}
