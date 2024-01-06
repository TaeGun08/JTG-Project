using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;

public class MainSceneManager : MonoBehaviour
{
    [Header("버튼 설정")]
    [SerializeField, Tooltip("파일을 처음 시작할 때 버튼")] private Button startButton;
    [SerializeField, Tooltip("저장 파일을 불러오는 버튼")] private Button loadButton;
    [SerializeField, Tooltip("종료 버튼")] private Button exitButton;
    private bool loadOn = false;
    [Space]
    [SerializeField, Tooltip("저장된 파일")] private GameObject saveFileObj;
    [SerializeField, Tooltip("저장 파일 닫기 버튼")] private Button saveCloseButton;
    [SerializeField, Tooltip("저장 파일 A")] private Button saveFileA;
    [SerializeField, Tooltip("저장 파일 B")] private Button saveFileB;
    [SerializeField, Tooltip("저장 파일 C")] private Button saveFileC;


    private void Awake()
    {
        saveFileObj.SetActive(false);

        startButton.onClick.AddListener(() =>
        {
            saveFileObj.SetActive(true);
        });

        loadButton.onClick.AddListener(() =>
        {
            saveFileObj.SetActive(true);
            loadOn = true;
        });

        exitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });

        saveCloseButton.onClick.AddListener(() =>
        {
            saveFileObj.SetActive(false);
            loadOn = false;
        });

        saveFileA.onClick.AddListener(() =>
        {
            if (loadOn == false)
            {
                SceneManager.LoadSceneAsync("TutorialScene");
            }
            else if (loadOn == true)
            {

            }
        });

        saveFileB.onClick.AddListener(() =>
        {
            if (loadOn == false)
            {

            }
            else if (loadOn == true)
            {

            }
        });

        saveFileC.onClick.AddListener(() =>
        {
            if (loadOn == false)
            {

            }
            else if (loadOn == true)
            {

            }
        });
    }

    private void resetSaveFileA()
    {
    }
}
