using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSkill : MonoBehaviour
{
    private PlayerUI playerUI;

    private KeyManager keyManager; //키매니저

    private TrashPreFab trashPreFab;
    private Player player;

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
    [SerializeField] private float skillADamage;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerUI = GetComponent<PlayerUI>();
    }

    private void Start()
    {
        keyManager = KeyManager.instance;

        trashPreFab = TrashPreFab.instance;

        skillCoolTimer = skillCoolTime;

        playerUI.PlayerSkiilCool(false);
    }

    private void Update()
    {
        skillCool();
        playerSpecialAttack();
    }

    /// <summary>
    /// 스킬 사용 시 쿨타임들
    /// </summary>
    private void skillCool()
    {
        if (skillCoolOn == true)
        {
            playerUI.PlayerSkiilCool(true);

            if (skillCoolTimer > 1.0f)
            {
                string timerTextInt = $"{(int)skillCoolTimer}";
                playerUI.SetPlayerSkillCool(skillCoolTimer / skillCoolTime, timerTextInt);
            }
            else if (skillCoolTimer < 1.0f)
            {
                string timerTextInt = $"{skillCoolTimer.ToString("F1")}";
                playerUI.SetPlayerSkillCool(skillCoolTimer / skillCoolTime, timerTextInt);
            }

            skillCoolTimer -= Time.deltaTime;           
            if (skillCoolTimer < 0.0f)
            {
                skillCoolOn = false;
                playerUI.PlayerSkiilCool(false);
                skillCoolTimer = skillCoolTime;
            }
        }
    }

    /// <summary>
    /// 플레이어들의 특수 스킬들을 담당하는 함수
    /// </summary>
    private void playerSpecialAttack()
    {
        if (Input.GetKeyDown(keyManager.PlayerSpecialAttackKey()) && skillCoolOn == false)
        {
            skillCoolOn = true;          

            Player.PlayerSkillType skillType = player.SkillType();
            if (skillType.ToString() == "skillTypeA")
            {
                GameObject skillObj = Instantiate(skillPrefabs[0], skillPos.position,
                    skillRot.rotation, trashPreFab.transform);
                Knife knifeSc = skillObj.GetComponent<Knife>();
                knifeSc.KnifeDamage(skillADamage + player.PlayerBuffDamage());
                knifeSc.KnifeForce(skillRot.rotation * (new Vector2(15.0f, 0f)), player.PlayerMouseAimRight());
            }
            else if (skillType.ToString() == "skillTypeB")
            {

            }
            else if (skillType.ToString() == "skillTypeC")
            {

            }
            else if (skillType.ToString() == "skillTypeD")
            {

            }
        }
    }
}
