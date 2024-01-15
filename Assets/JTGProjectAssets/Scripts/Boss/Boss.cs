using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private Animator anim;
        
    [Header("보스 기본 설정")]
    [SerializeField, Tooltip("보스의 이동속도")] private float bossSpeed;
    [SerializeField, Tooltip("보스의 현재체력")] private int bossCurHp;
    [SerializeField, Tooltip("보스의 패턴별 체력")] private List<int> bossPatternHp;
    [SerializeField] private int curPattern;
    private bool patternChange = false;
    private float changeTimer;

    private void Awake()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();

        bossCurHp = bossPatternHp[0];

        changeTimer = 0;
    }

    private void Update()
    {
        timers();
        bossDeadCheck();
    }

    /// <summary>
    /// 보스에 관련된 타이머 모음
    /// </summary>
    private void timers()
    {
        if (patternChange == true)
        {
            changeTimer += Time.deltaTime;
            if (changeTimer > 2)
            {
                ++curPattern;
                patternHpChange();
                changeTimer = 0;
                patternChange = false;
            }
        }
    }

    /// <summary>
    /// 보스의 패턴이 변경될 때 체력도 변경
    /// </summary>
    private void patternHpChange()
    {
        if (curPattern > 2)
        {
            return;
        }

        if (curPattern == 1)
        {
            bossCurHp = bossPatternHp[1];
        }
        else if (curPattern == 2)
        {
            bossCurHp = bossPatternHp[2];
        }
    }

    /// <summary>
    /// 보스가 모든 패턴을 사용 후 죽었을 시 삭제
    /// </summary>
    private void bossDeadCheck()
    {
        if (bossCurHp <= 0 && curPattern > 2)
        {
            Destroy(gameObject);
        }
        else if (bossCurHp <= 0)
        {
            patternChange = true;
        }
    }
}
