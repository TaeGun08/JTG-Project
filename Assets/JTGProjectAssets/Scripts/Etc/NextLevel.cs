using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SaveObject saveObject;
    [SerializeField] private Status status;

    private GameManager gameManager;
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
        gameManager = GameManager.Instance;
        keyManager = KeyManager.instance;

        homeInKeyImage.SetActive(false);
    }

    private void Update()
    {
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
            saveObject.PlayerDataResetOn(false);
            player.PlayerSaveOn(true);
            status.PlayerStatusSaveOn(true);
            SceneManager.LoadSceneAsync("LoadingScene");
        }
    }
}
