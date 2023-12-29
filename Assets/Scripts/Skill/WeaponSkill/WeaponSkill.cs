using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSkill : MonoBehaviour
{
    //무기 스킬에 가져올 매니저
    private GameManager gameManager; //게임매니저
    private KeyManager keyManager; //키매니저

    private TrashPreFab trashPreFab;

    private Weapons weapons;

    [Header("스킬 프리팹")]
    [SerializeField] private List<GameObject> skillPrefabs = new List<GameObject>();

    [Header("스킬이 나타날 위치 및 회전 값")]
    [SerializeField] private Transform skillPos;
    [SerializeField] private Transform skillRot;

    [Header("스킬 설정")]
    [SerializeField] private float skillCoolTime = 1.0f;
    private float skillCoolTimer = 0.0f;
    private bool skillCoolOn = false;
    private float coolTime;

    private void Awake()
    {
        weapons = GetComponent<Weapons>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        keyManager = KeyManager.instance;

        trashPreFab = TrashPreFab.instance;

        skillCoolTimer = skillCoolTime;

        coolTime = gameManager.WeaponCoolTimePanelImage().fillAmount;
    }

    private void Update()
    {
        skillCool();
        weaponSpecialAttack();
    }

    /// <summary>
    /// 스킬 사용 시 쿨타임들
    /// </summary>
    private void skillCool()
    {
        if (skillCoolOn == true)
        {
            gameManager.WeaponSkillCoolTime(true);

            if (skillCoolTimer > 1.0f)
            {
                string timerTextInt = $"{(int)skillCoolTimer}";
                gameManager.WeaponCoolTimeText().text = timerTextInt;
            }
            else if (skillCoolTimer < 1.0f)
            {
                string timerTextInt = $"{skillCoolTimer.ToString("F1")}";
                gameManager.WeaponCoolTimeText().text = timerTextInt;
            }

            skillCoolTimer -= Time.deltaTime;
            gameManager.WeaponCoolTimePanelImage().fillAmount = skillCoolTimer / skillCoolTime;
            if (skillCoolTimer < 0.0f)
            {
                skillCoolOn = false;
                gameManager.WeaponSkillCoolTime(false);
                gameManager.WeaponCoolTimePanelImage().fillAmount = coolTime;
                skillCoolTimer = skillCoolTime;
            }
        }
    }

    /// <summary>
    /// 무기 스킬을 담당하는 함수
    /// </summary>
    private void weaponSpecialAttack()
    {
        if (Input.GetKeyDown(keyManager.WeaponSkiilKey()) && skillCoolOn == false)
        {
            skillCoolOn = true;

            GameObject skillObj = Instantiate(skillPrefabs[0], skillPos.position,
                    skillRot.rotation, trashPreFab.transform);
        }
    }
}
