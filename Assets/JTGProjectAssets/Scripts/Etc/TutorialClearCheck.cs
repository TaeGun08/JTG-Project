using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class TutorialClearCheck : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SaveObject saveObject;
    [SerializeField] private Status status;

    private KeyManager keyManager;

    [SerializeField] private GameObject homeInKeyImage;
    private bool homeIn = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            homeIn = true;

            homeInKeyImage.SetActive(true);
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

        saveObject.PlayerDataResetOn(true);

        homeInKeyImage.SetActive(false);
    }

    private void Update()
    {
        nextLevelLoading();
    }

    private void nextLevelLoading()
    {
        if (Input.GetKeyDown(keyManager.InteractionKey()) && homeIn == true)
        {
            saveObject.PlayerDataResetOn(true);
            string get = JsonConvert.SerializeObject(2);
            PlayerPrefs.SetString("saveKey", get);
            int nextLevel = JsonConvert.DeserializeObject<int>(PlayerPrefs.GetString("saveKey"));
            SceneManager.LoadSceneAsync(nextLevel);
        }
    }
}
