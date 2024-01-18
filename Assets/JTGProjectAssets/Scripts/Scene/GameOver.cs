using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [Header("게임 오버 시 메인으로 돌아가는 버튼")]
    [SerializeField] private Button gameOverButton;

    private void Awake()
    {
        gameOverButton.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(0);
        });
    }
}
