using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    //플레이어스킬에 가져올 매니저
    private GameManager gameManager; //게임매니저
    private KeyManager keyManager; //키매니저

    private TrashPreFab trashPreFab;

    private Player player;

    [Header("스킬 프리팹")]
    [SerializeField] private List<GameObject> skillPrefabs = new List<GameObject>();

    [Header("스킬이 나타날 위치 및 회전 값")]
    [SerializeField] private Transform skillPos;
    [SerializeField] private Transform skillRot;

    private void Start()
    {
        gameManager = GameManager.Instance;
        keyManager = KeyManager.instance;

        trashPreFab = TrashPreFab.instance;

        player = GetComponent<Player>();
    }

    private void Update()
    {
        playerSpecialAttack();
    }

    /// <summary>
    /// 플레이어들의 특수 스킬들을 담당하는 함수
    /// </summary>
    private void playerSpecialAttack()
    {
        if (Input.GetKeyDown(keyManager.PlayerSpecialAttackKey()))
        {
            Player.PlayerSkillType skillType = player.SkillType();
            if (skillType.ToString() == "skillTypeA")
            {
                GameObject skillObj = Instantiate(skillPrefabs[0], skillPos.position,
                    Quaternion.identity, trashPreFab.transform);
                Knife knifeSc = skillObj.GetComponent<Knife>();

                knifeSc.KnifeForce(new Vector2(15.0f, 0f), player.playerMouseAimRight());
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
