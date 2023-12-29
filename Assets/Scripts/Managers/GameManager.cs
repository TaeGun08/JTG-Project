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
    [SerializeField] private GameObject playerSkillCoolTimePanel;
    [SerializeField] private Image playerCoolTimePanelImage;
    [SerializeField] private GameObject playerSkillCollTimeText;
    [SerializeField] private TMP_Text playerCoolTimeText;
    [Space]
    [SerializeField] private GameObject weaponSkill;
    [SerializeField] private GameObject weaponSkillCoolTimePanel;
    [SerializeField] private Image weaponCoolTimePanelImage;
    [SerializeField] private GameObject weaponSkillCollTimeText;
    [SerializeField] private TMP_Text weaponCoolTimeText;

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

    /// <summary>
    /// 무기의 스킬 이미지를 화면에 보이게 함
    /// </summary>
    public void WeaponSkillOn(bool _skillOn)
    {
        weaponSkill.SetActive(_skillOn);
    }

    /// <summary>
    /// 플레이어 스킬을 사용 후 쿨타임이 실행될 때 활성화 됨
    /// </summary>
    /// <param name="_objOn"></param>
    public void PlayerSkillCoolTimePanel(bool _objOn)
    {
        playerSkillCoolTimePanel.SetActive(_objOn);
        playerSkillCollTimeText.SetActive(_objOn);
    }

    public Image PlayerCoolTimePanelImage()
    {
        return playerCoolTimePanelImage;
    }

    public TMP_Text PlayerCoolTimeText()
    {
        return playerCoolTimeText;
    }

    /// <summary>
    /// 무기 스킬을 사용 후 쿨타임이 실행될 때 활성화 됨
    /// </summary>
    /// <param name="_objOn"></param>
    public void WeaponSkillCoolTimePanel(bool _objOn)
    {
        weaponSkillCoolTimePanel.SetActive(_objOn);
        weaponSkillCollTimeText.SetActive(_objOn);
    }
}
