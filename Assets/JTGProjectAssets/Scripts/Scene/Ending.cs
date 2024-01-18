using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ending : MonoBehaviour
{
    [Header("엔딩버튼")]
    [SerializeField] private Button gameOverButton;

    private void Awake()
    {
        gameOverButton.onClick.AddListener(() =>
        {
            string getScene = JsonConvert.SerializeObject(1);
            PlayerPrefs.SetString("saveKey", getScene);
            SceneManager.LoadSceneAsync(0);
        });
    }
}
