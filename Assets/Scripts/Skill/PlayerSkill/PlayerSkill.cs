using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSkill : MonoBehaviour
{
    //플레이어스킬에 가져올 매니저
    private GameManager gameManager; //게임매니저
    private KeyManager keyManager; //키매니저

    private TrashPreFab trashPreFab;

    private Player player;

    [Header("스킬 UI관련 데이터")]
    [SerializeField] private GameObject playerSkill;
    [SerializeField] private GameObject skillCoolTimePanel;
    [SerializeField] private Image coolTimePanelImage;
    [SerializeField] private GameObject skillCollTimeText;
    [SerializeField] private TMP_Text coolTimeText;
    private float coolTime;

    [Header("스킬 프리팹")]
    [SerializeField] private List<GameObject> skillPrefabs = new List<GameObject>();

    [Header("스킬이 나타날 위치 및 회전 값")]
    [SerializeField] private Transform skillPos;
    [SerializeField] private Transform skillRot;

    [Header("스킬 설정")]
    [SerializeField] private float skillCoolTime = 1.0f;
    private float skillCoolTimer = 0.0f;
    private bool skillCoolOn = false;


    private void Start()
    {
        gameManager = GameManager.Instance;
        keyManager = KeyManager.instance;

        trashPreFab = TrashPreFab.instance;

        player = GetComponent<Player>();

        skillCoolTimer = skillCoolTime;

        playerSkill.SetActive(true);
        skillCollTimeText.SetActive(false);
        skillCoolTimePanel.SetActive(false);

        coolTime = coolTimePanelImage.fillAmount;
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
            string timerText = $"{(int)skillCoolTimer + 1}";
            coolTimeText.text = timerText;
            skillCoolTimer -= Time.deltaTime;
            coolTimePanelImage.fillAmount = skillCoolTimer;
            if (skillCoolTimer < 0.0f)
            {
                skillCoolOn = false;
                skillCollTimeText.SetActive(false);
                skillCoolTimePanel.SetActive(false);
                coolTimePanelImage.fillAmount = coolTime;
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
            skillCollTimeText.SetActive(true);
            skillCoolTimePanel.SetActive(true);

            Player.PlayerSkillType skillType = player.SkillType();
            if (skillType.ToString() == "skillTypeA")
            {
                GameObject skillObj = Instantiate(skillPrefabs[0], skillPos.position,
                    skillRot.rotation, trashPreFab.transform);
                Knife knifeSc = skillObj.GetComponent<Knife>();

                knifeSc.KnifeForce(skillRot.rotation * (new Vector2(15.0f, 0f)), player.playerMouseAimRight());
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
