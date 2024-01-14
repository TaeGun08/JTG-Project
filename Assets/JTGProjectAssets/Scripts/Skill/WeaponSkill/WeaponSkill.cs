using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSkill : MonoBehaviour
{
    public enum WeaponSkillType
    {
        skillA,
        skillB,
        skillC,
        skillD,
    }

    [SerializeField] WeaponSkillType skillType;

    //무기 스킬에 가져올 매니저
    private GameManager gameManager; //게임매니저
    private KeyManager keyManager; //키매니저

    private TrashPreFab trashPreFab;
    private Weapons weapons;

    [Header("스킬 프리팹")]
    [SerializeField, Tooltip("사용될 스킬 프리팹")] private List<GameObject> skillPrefabs = new List<GameObject>();

    [Header("스킬이 나타날 위치 및 회전 값")]
    [SerializeField, Tooltip("스킬이 발동될 위치")] private Transform skillPos;
    [SerializeField, Tooltip("스킬의 회전 값")] private Transform skillRot;

    [Header("스킬 기본 설정")]
    [SerializeField, Tooltip("스킬 쿨타임")] private float skillCoolTime = 1.0f;
    private float skillCoolTimer = 0.0f; //스킬 쿨타이머
    private bool skillCoolOn = false; //스킬을 썼을 때 쿨타임을 작동시키기 위한 변수
    private SpriteRenderer weaponRen; //무기의 스프라이트 렌더러
    private bool useSkill = false;
    private float skillDamage;

    [Header("스킬 UI")]
    [SerializeField, Tooltip("활성화 시킬 스킬 오브젝트")] private GameObject weaponSkill;
    [SerializeField, Tooltip("스킬 이미지")] private Image weaponSkilImage; //스킬 이미지를 받아올 변수
    [SerializeField, Tooltip("스킬 쿨타임 이미지")] private GameObject weaponSkillCoolTimePanel;
    private Image weaponCoolTimePanelImage; //스킬 쿨타임 이미지 오브젝트를 받아올 변수
    [SerializeField, Tooltip("스킬 쿨타임 텍스트")] private GameObject weaponSkillCollTimeText;
    private TMP_Text weaponCoolTimeText; //스킬 쿨타임 텍스트를 받아올 변수

    [Header("스킬A 설정")]
    [SerializeField, Tooltip("스킬 A의 지속시간")] private float skillATime = 0.0f;
    private float skillATimer = 0.0f; //스킬 실행 시 돌아가는 타이머
    private bool skillAOn = false; //스킬 A가 발동이 되었는지를 체크해주는 변수
    private GameObject curBullet; //총알의 원본 데이터를 담을 변수

    [Header("스킬C 설정")]
    [SerializeField, Tooltip("스킬 C의 차징 1단계 시간")] private float chargingLevel1Time;
    [SerializeField, Tooltip("스킬 C의 최대 차징 시간")] private float lastChargingTime;
    [SerializeField] private GameObject chargingObj;
    private Image chargingImage;
    private float chargingTimer;

    private void Awake()
    {
        weapons = GetComponent<Weapons>();
        weaponRen = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        keyManager = KeyManager.instance;

        trashPreFab = TrashPreFab.Instance;

        weaponCoolTimePanelImage = weaponSkillCoolTimePanel.GetComponent<Image>();
        weaponCoolTimeText = weaponSkillCollTimeText.GetComponent<TMP_Text>();
        chargingImage = chargingObj.GetComponent<Image>();

        skillCoolTimer = skillCoolTime;
    }

    private void Update()
    {
        weaponSkillOn();

        if (gameManager.GetGamePause() == true)
        {
            return;
        }

        if (weaponSkill.activeSelf == false)
        {
            skillATimer = 0;
            chargingTimer = 0;
            chargingImage.fillAmount = 0;
            return;
        }

        skillCool();
        weaponSpecialAttack();
        skillAControll();
    }

    private void weaponSkillOn()
    {
        if (weaponRen.enabled == true && weapons.ShootingOnCheck() == true)
        {
            weaponSkill.SetActive(true);
        }
        else if (weaponRen.enabled == false && weapons.ShootingOnCheck() == false)
        {
            weaponSkill.SetActive(false);
        }
    }

    /// <summary>
    /// 스킬 사용 시 쿨타임들
    /// </summary>
    private void skillCool()
    {
        if (skillCoolOn == true)
        {
            if (weaponRen.enabled == true)
            {
                weaponSkillCoolTimePanel.SetActive(true);
                weaponSkillCollTimeText.SetActive(true);
            }

            if (skillCoolTimer > 1.0f)
            {
                string timerTextInt = $"{(int)skillCoolTimer}";
                weaponCoolTimeText.text = timerTextInt;
            }
            else if (skillCoolTimer < 1.0f)
            {
                string timerTextInt = $"{skillCoolTimer.ToString("F1")}";
                weaponCoolTimeText.text = timerTextInt;
            }

            skillCoolTimer -= Time.deltaTime;
            weaponCoolTimePanelImage.fillAmount = skillCoolTimer / skillCoolTime;
            if (skillCoolTimer < 0.0f)
            {
                skillCoolOn = false;
                weaponSkillCoolTimePanel.SetActive(false);
                weaponSkillCollTimeText.SetActive(false);
                skillCoolTimer = skillCoolTime;
            }
        }
    }

    /// <summary>
    /// 무기 스킬을 담당하는 함수
    /// </summary>
    private void weaponSpecialAttack()
    {
        //스킬 A와 B를 제어하는 코드
        if (Input.GetKeyDown(keyManager.WeaponSkiilKey()) && skillCoolOn == false
            && weapons.ShootingOnCheck() == true && weaponRen.enabled == true && skillType.ToString() != "skillC")
        {
            skillCoolOn = true;

            useSkill = true;

            weapons.WeaponUseShooting(false);

            if (skillType.ToString() == "skillA")
            {
                curBullet = weapons.CurBullet();
                weapons.BulletChange(skillPrefabs[0]);
                skillATimer = skillATime;
                skillAOn = true;
                useSkill = false;
            }
            else if (skillType.ToString() == "skillB")
            {
                GameObject bulletObj = Instantiate(skillPrefabs[0], skillPos.position, skillRot.rotation, trashPreFab.transform);
                Bullet bulletSc = bulletObj.GetComponent<Bullet>();
                bulletSc.BulletDamage(skillDamage, 2, true, weapons.HitCriticalCheck());
                useSkill = false;
            }
            else if (skillType.ToString() == "skillD")
            {
                Instantiate(skillPrefabs[0], skillPos.position, skillRot.rotation, trashPreFab.transform);
            }
        }

        //스킬 C를 제어하는 전용 코드
        if (Input.GetKey(keyManager.WeaponSkiilKey()) && skillCoolOn == false
            && weapons.ShootingOnCheck() == true && weaponRen.enabled == true && skillType.ToString() == "skillC")
        {
            useSkill = true;
            chargingObj.SetActive(true);
            chargingTimer += Time.deltaTime;
            chargingImage.fillAmount = chargingTimer / lastChargingTime;
        }
        else if (Input.GetKeyUp(keyManager.WeaponSkiilKey()) && skillCoolOn == false
            && weapons.ShootingOnCheck() == true && weaponRen.enabled == true && skillType.ToString() == "skillC")
        {
            skillCoolOn = true;

            useSkill = false;

            weapons.WeaponUseShooting(false);

            if (chargingTimer < chargingLevel1Time)
            {
                GameObject skillObj = Instantiate(skillPrefabs[0], skillPos.position,
                skillRot.rotation, trashPreFab.transform);
                Bullet bulletSc = skillObj.GetComponent<Bullet>();
                bulletSc.BulletDamage(skillDamage, 1.2f, true, weapons.HitCriticalCheck());
                skillObj.transform.localScale = new Vector2(1.0f, 1.0f);
            }
            else if (chargingTimer >= chargingLevel1Time && chargingTimer <= lastChargingTime)
            {
                GameObject skillObj = Instantiate(skillPrefabs[1], skillPos.position,
                skillRot.rotation, trashPreFab.transform);
                Bullet bulletSc = skillObj.GetComponent<Bullet>();
                bulletSc.BulletDamage(skillDamage, 2f, true, weapons.HitCriticalCheck());
                skillObj.transform.localScale = new Vector2(2.0f, 2.0f);
            }
            else if (chargingTimer >= lastChargingTime)
            {
                GameObject skillObj = Instantiate(skillPrefabs[2], skillPos.position,
                skillRot.rotation, trashPreFab.transform);
                Bullet bulletSc = skillObj.GetComponent<Bullet>();
                bulletSc.BulletDamage(skillDamage, 3f, true, weapons.HitCriticalCheck());
                skillObj.transform.localScale = new Vector2(3.0f, 3.0f);
            }

            chargingObj.SetActive(false);

            chargingImage.fillAmount = 0.0f;

            chargingTimer = 0.0f;
        }
    }

    /// <summary>
    /// 스킬 A의 기능을 담고 있는 함수
    /// </summary>
    private void skillAControll()
    {
        if (skillAOn == true)
        {
            skillATimer -= Time.deltaTime;
            if (skillATimer < 0.0f)
            {
                weapons.BulletChange(curBullet);
                skillAOn = false;
                skillATimer = skillATime;
            }
        }
    }

    public bool SkillAOn()
    {
        return skillAOn;
    }

    public bool UseSkill()
    {
        return useSkill;
    }

    public void WeaponSkillOff(bool _skillOff)
    {
        weaponSkill.SetActive(_skillOff);
    }

    public void GetSkillDamage(float _skillDmg)
    {
        skillDamage = _skillDmg;
    }
}
