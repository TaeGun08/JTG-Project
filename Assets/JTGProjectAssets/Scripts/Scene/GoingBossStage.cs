using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoingBossStage : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SaveObject saveObject;
    [SerializeField] private Status status;

    [SerializeField] private FadeOut fadeSc;
    private bool fadeOut = false;
    private float fadeTimer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            fadeSc.FadeInOut(true);
            fadeOut = true;
        }
    }

    private void Update()
    {
        if (fadeOut == true)
        {
            fadeTimer += Time.unscaledDeltaTime;
            if (fadeTimer > 1)
            {
                nextLevelLoading();
                fadeTimer = 0;
                fadeOut = false;
            }

        }
    }

    private void nextLevelLoading()
    {
        saveObject.PlayerDataResetOn(false);
        player.PlayerSaveOn(true);
        status.PlayerStatusSaveOn(true);
        SceneManager.LoadSceneAsync("BossCutScene");
    }
}
