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

    [Header("중력")]
    [SerializeField] private float gravity;

    [Header("재장전UI")]
    [SerializeField] private GameObject reloadingObj;
    [SerializeField] private Slider reloadingUI;

    [Header("스킬 UI")]
    [SerializeField] private GameObject playerSkill;
    [SerializeField] private Image playerSkillImage;
    [SerializeField] private GameObject playerSkillCoolTimePanel;
    [SerializeField] private Image playerCoolTimePanelImage;
    [SerializeField] private GameObject playerSkillCollTimeText;
    [SerializeField] private TMP_Text playerCoolTimeText;

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

    public bool GamePause()
    {
        return gamePause;
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

    public Transform ItemDropTrs()
    {
        return itemDropTrs;
    }
}
