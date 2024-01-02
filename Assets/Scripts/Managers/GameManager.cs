using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("게임 중지")]
    [SerializeField] private bool gamePause = false;

    [Header("플레이어 관련 설정")]
    [SerializeField] private GameObject playerPrefab;
    private GameObject playerObj;
    [SerializeField] private Transform playerStartPos;
    private bool playerCreateOn = false;

    [Header("중력")]
    [SerializeField] private float gravity;

    [Header("재장전UI")]
    [SerializeField] private GameObject reloadingObj;
    [SerializeField] private Slider reloadingUI;

    [Header("플레이어 UI")]
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject playerSkill;
    [SerializeField] private Image playerSkillImage;
    [SerializeField] private GameObject playerSkillCoolTimePanel;
    [SerializeField] private Image playerCoolTimePanelImage;
    [SerializeField] private GameObject playerSkillCollTimeText;
    [SerializeField] private TMP_Text playerCoolTimeText;
    [SerializeField] private GameObject playerDashCoolPanel;
    [SerializeField] private Image playerDashCoolPanelImage;
    [SerializeField] private GameObject playerDashCoolText;
    [SerializeField] private TMP_Text playerDashCoolTimeText;
    [SerializeField] private Slider playerHpSlider;
    [SerializeField] private TMP_Text playerHpText;

    [Header("아이템 드랍 오브젝트 위치")]
    [SerializeField] private Transform itemDropTrs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerCreate();
    }

    private void LateUpdate()
    {
        if (playerObj == null)
        {
            playerUI.SetActive(false);
        }
        else if (playerObj != null)
        {
            playerUI.SetActive(true);
        }
    }

    private void playerCreate()
    {
        if (playerCreateOn == false)
        {
            playerObj = Instantiate(playerPrefab, playerStartPos.position, Quaternion.identity, playerStartPos);
        }
    }

    public bool GamePause()
    {
        return gamePause;
    }

    public GameObject PlayerPrefab()
    {
        return playerObj;
    }

    public float gravityScale()
    {
        return gravity;
    }

    public GameObject ReloadingObj()
    {
        return reloadingObj;
    }

    public Slider ReloadingUI()
    {
        return reloadingUI;
    }

    /// <summary>
    /// 플레이어의 스킬 이미지를 화면에 보이게 함
    /// </summary>
    public void PlayerSkillOn(bool _skillOn)
    {
        playerSkill.SetActive(_skillOn);
    }

    public Image PlayerSkillImage()
    {
        return playerSkillImage;
    }

    /// <summary>
    /// 플레이어 스킬을 사용 후 쿨타임이 실행될 때 활성화 됨
    /// </summary>
    /// <param name="_objOn"></param>
    public void PlayerSkillCoolTime(bool _objOn)
    {
        playerSkillCoolTimePanel.SetActive(_objOn);
        playerSkillCollTimeText.SetActive(_objOn);
    }

    public Image PlayerCoolTimeImage()
    {
        return playerCoolTimePanelImage;
    }

    public TMP_Text PlayerCoolTimeText()
    {
        return playerCoolTimeText;
    }

    public GameObject PlayerDashPanel()
    {
        return playerDashCoolPanel;
    }

    public GameObject PlayerDashText()
    {
        return playerDashCoolText;
    }

    public Image PlayerDashCoolPanelImage()
    {
        return playerDashCoolPanelImage;
    }

    public TMP_Text PlayerDashCoolTimeText()
    {
        return playerDashCoolTimeText;
    }

    public Slider PlayerHpSlider()
    {
        return playerHpSlider;
    }

    public TMP_Text PlayerHpText()
    {
        return playerHpText;
    }

    public Transform ItemDropTrs()
    {
        return itemDropTrs;
    }
}
