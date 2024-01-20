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

    private GameManager gameManager;
    private KeyManager keyManager;

    [SerializeField] private GameObject homeInKeyImage;
    private bool homeIn = false;

    [SerializeField] private FadeOut fadeSc;
    private bool fadeOut = false;
    private float fadeTimer;

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
        gameManager = GameManager.Instance;
        keyManager = KeyManager.instance;

        saveObject.PlayerDataResetOn(true);

        homeInKeyImage.SetActive(false);
    }

    private void Update()
    {
        if (fadeOut == true)
        {
            fadeTimer += Time.unscaledDeltaTime;
            if (fadeTimer > 1)
            {
                saveObject.PlayerDataResetOn(true);
                SceneManager.LoadSceneAsync("LoadingScene");
                fadeTimer = 0;
                fadeOut = false;
            }
        }

        if (gameManager.GetGamePause() == true)
        {
            return;
        }

        nextLevelLoading();
    }

    private void nextLevelLoading()
    {
        if (Input.GetKeyDown(keyManager.InteractionKey()) && homeIn == true)
        {
            fadeSc.FadeInOut(true);
            fadeOut = true;
        }
    }
}
