using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayOption : MonoBehaviour
{
    [SerializeField] Player player;

    [Header("플레이 옵션창 설정")]
    [SerializeField] private Button backPlay;
    [SerializeField] private Button goTitle;
    [SerializeField] private Button gameExitCheck;
    [SerializeField] private Button gameExit;
    [SerializeField] private Button cancleExit;
    [SerializeField] private GameObject exitCheck;

    private void Awake()
    {
        backPlay.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            player.OptionOff(false);
        });

        goTitle.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(0);
        });

        gameExitCheck.onClick.AddListener(() =>
        {
            exitCheck.SetActive(true);
        });

        gameExit.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });

        cancleExit.onClick.AddListener(() => 
        {
            exitCheck.SetActive(false);
        });
    }
}
