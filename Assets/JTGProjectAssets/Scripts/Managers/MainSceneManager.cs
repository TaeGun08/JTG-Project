using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Globalization;

public class MainSceneManager : MonoBehaviour
{
    [Header("버튼 설정")]
    [SerializeField, Tooltip("파일을 처음 시작할 때 버튼")] private Button startButton;
    [SerializeField, Tooltip("저장 파일을 불러오는 버튼")] private Button loadButton;
    [SerializeField, Tooltip("종료 버튼")] private Button exitButton;
    [Space]
    [SerializeField, Tooltip("저장된 파일")] private GameObject saveClearCheckObj;
    [SerializeField, Tooltip("저장 파일 초기화 확인 버튼")] private Button newClearButton;
    [SerializeField, Tooltip("저장 파일 초기화 취소 버튼")] private Button newCancelButton;
    [SerializeField, Tooltip("저장 파일이 없으면 뜨는 메세지")] private GameObject notSaveFile;
    private float closeObj;
    [Space]
    [SerializeField] private FadeOut fadeSc;
    private bool fadeOut = false;
    private float fadeTimer;
    private bool tutoOn = false;
    private bool saveOn = false;

    private void Awake()
    {
        Time.timeScale = 1;

        saveClearCheckObj.SetActive(false);
        notSaveFile.SetActive(false);

        if (PlayerPrefs.GetString("saveKey") == string.Empty)
        {
            string getScene = JsonConvert.SerializeObject(1);
            PlayerPrefs.SetString("saveKey", getScene);
        }

        startButton.onClick.AddListener(() =>
        {           
            int nextLevel = JsonConvert.DeserializeObject<int>(PlayerPrefs.GetString("saveKey"));

            if (nextLevel > 1)
            {
                saveClearCheckObj.SetActive(true);
            }
            else
            {
                fadeSc.FadeInOut(true);
                fadeOut = true;
                tutoOn = true;
            }
        });

        loadButton.onClick.AddListener(() =>
        {
            int nextLevel = JsonConvert.DeserializeObject<int>(PlayerPrefs.GetString("saveKey"));
            if (nextLevel > 1)
            {
                fadeSc.FadeInOut(true);
                fadeOut = true;
                saveOn = true;
            }
            else
            {
                notSaveFile.SetActive(true);
            }
        });

        exitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });

        newClearButton.onClick.AddListener(() =>
        {
            string getScene = JsonConvert.SerializeObject(1);
            PlayerPrefs.SetString("saveKey", getScene);
            saveClearCheckObj.SetActive(false);
        });

        newCancelButton.onClick.AddListener(() =>
        {
            saveClearCheckObj.SetActive(false);
        });
    }

    private void Update()
    {
        if (notSaveFile.activeSelf == true)
        {
            closeObj += Time.deltaTime;
            if (closeObj >= 1)
            {
                notSaveFile.SetActive(false);
                closeObj = 0;
            }
        }

        if (fadeOut == true)
        {
            fadeTimer += Time.deltaTime;
            if (fadeTimer > 1 && tutoOn == true)
            {
                SceneManager.LoadSceneAsync("TutorialLoadingScene");
                fadeTimer = 0;
                fadeOut = false;
                tutoOn = false;
            }
            else if (fadeTimer > 1 && saveOn == true)
            {
                SceneManager.LoadSceneAsync("SaveLoadScene");
                fadeTimer = 0;
                fadeOut = false;
                saveOn = false;
            }
        }
    }
}
