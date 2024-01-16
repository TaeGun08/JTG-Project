using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private BoxCollider2D boxColl2D;
    private RaycastHit2D hit;
    private Rigidbody2D rigid;
    private Vector3 moveVec;
    private Animator anim;
    private SpriteRenderer bossRen;

    private GameManager gameManager;

    [Header("보스 기본 설정")]
    [SerializeField, Tooltip("보스의 이동속도")] private float bossSpeed;
    [SerializeField, Tooltip("보스의 현재체력")] private int bossCurHp;
    [SerializeField, Tooltip("보스의 패턴별 체력")] private List<int> bossPhaseHp;
    [SerializeField] private int curPhase;
    private bool phaseChange = false;
    private float changeTimer;
    private bool isGround = false;
    private float gravity;
    private float gravityVelocity;

    [Header("보스 공격 설정")]
    [SerializeField, Tooltip("보스의 기본 공격을 위한 영역")] private List<BoxCollider2D> pattern1Attack;
    [SerializeField, Tooltip("보스의 패턴2 공격을 위한 영역")] private BoxCollider2D pattern2Attack;
    [SerializeField, Tooltip("보스의 패턴3 공격을 위한 영역")] private BoxCollider2D pattern3Attack;
    [SerializeField, Tooltip("보스의 패턴3의 공격 프리팹")] private GameObject patternAttackPrefab;
    private bool isAttack = false;
    private bool patternAttack = false;
    private int randomPattern;
    private float attackDelay;

    private void OnDrawGizmos() //박스캐스트를 씬뷰에서 눈으로 확인이 가능하게 보여줌
    {
        if (boxColl2D != null) //콜라이더가 null이 아니라면 박스레이 범위를 씬뷰에서 확인할 수 있게
        {
            Gizmos.color = Color.red;
            Vector3 pos = boxColl2D.bounds.center - new Vector3(0, 0.1f, 0);
            Gizmos.DrawWireCube(pos, boxColl2D.bounds.size);
        }
    }

    private void onTriggerArea1Check(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            pattern1Attack[1].gameObject.SetActive(true);
        }
    }

    private void onTriggerArea2Check(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            pattern1Attack[1].gameObject.SetActive(true);
        }
    }

    private void onTriggerArea3Check(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isAttack = true;
        }
    }

    private void onTriggerAttackCheck(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {          
            anim.SetTrigger("isPatternA");
            //anim.SetBool("isAttack", true);
            //pattern1Attack[1].gameObject.SetActive(false);
            //isAttack = true;
        }
    }

    private void Awake()
    {
        boxColl2D = GetComponent<BoxCollider2D>();
        rigid = GetComponent<Rigidbody2D>();

        bossCurHp = bossPhaseHp[0];

        changeTimer = 0;

        pattern1Attack[1].gameObject.SetActive(false);
    }

    private void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        bossRen = transform.GetChild(0).GetComponent<SpriteRenderer>();

        gameManager = GameManager.Instance;

        gravity = gameManager.GravityScale();
    }

    private void Update()
    {
        if (gameManager.GetGamePause() == true)
        {
            return;
        }

        playerCollCheck();
        timers();
        bossGroundCheck();
        bossGravity();
        bossDeadCheck();
    }

    private void playerCollCheck()
    {
        Collider2D pattern1 = Physics2D.OverlapBox(pattern1Attack[0].bounds.center,
            pattern1Attack[0].bounds.size, 0, LayerMask.GetMask("Player"));

        Collider2D pattern2 = Physics2D.OverlapBox(pattern2Attack.bounds.center,
            pattern2Attack.bounds.size, 0, LayerMask.GetMask("Player"));

        Collider2D pattern3 = Physics2D.OverlapBox(pattern3Attack.bounds.center,
            pattern3Attack.bounds.size, 0, LayerMask.GetMask("Player"));

        Collider2D pattern1Att = Physics2D.OverlapBox(pattern1Attack[1].bounds.center,
            pattern1Attack[1].bounds.size, LayerMask.GetMask("Player"));


        if (patternAttack == false && curPhase == 0)
        {
            if (pattern1 != null && isAttack == false)
            {
                onTriggerArea1Check(pattern1);
            }

            if (pattern1Att != null && isAttack == false)
            {
                onTriggerAttackCheck(pattern1Att);
            }
        }
        else if (patternAttack == true && curPhase == 1)
        {
            if (pattern1 != null && isAttack == false)
            {
                onTriggerArea1Check(pattern1);
            }
            else if (pattern2 != null && isAttack == false)
            {
                onTriggerArea2Check(pattern2);
            }

            if (pattern1Att != null && isAttack == false)
            {
                onTriggerAttackCheck(pattern1Att);
            }
        }
        else if (patternAttack == true && curPhase == 2)
        {
            if (pattern1 != null && isAttack == false)
            {
                onTriggerArea1Check(pattern1);
            }
            else if (pattern2 != null && isAttack == false)
            {
                onTriggerArea2Check(pattern2);
            }
            else if (pattern3 != null && isAttack == false)
            {
                onTriggerArea3Check(pattern3);
            }

            if (pattern1Att != null && isAttack == false)
            {
                onTriggerAttackCheck(pattern1Att);
            }
        }
    }

    /// <summary>
    /// 보스에 관련된 타이머 모음
    /// </summary>
    private void timers()
    {
        if (phaseChange == true)
        {
            if (curPhase == 2)
            {
                ++curPhase;
            }

            changeTimer += Time.deltaTime;
            if (changeTimer > 2)
            {
                ++curPhase;
                patternHpChange();
                changeTimer = 0;
                phaseChange = false;
            }
        }

        if (isAttack == true)
        {
            attackDelay += Time.deltaTime;
            if (attackDelay > 3)
            {
                anim.SetBool("isAttack", false);
                isAttack = false;
            }
        }
    }

    /// <summary>
    /// 보스가 땅을 체크할 수 있게 담당하는 함수
    /// </summary>
    private void bossGroundCheck()
    {
        isGround = false;

        hit = Physics2D.BoxCast(boxColl2D.bounds.center, boxColl2D.bounds.size, 0,
            Vector2.down, 0.1f, LayerMask.GetMask("Ground"));

        if (hit.transform != null && hit.transform.gameObject.layer
            == LayerMask.NameToLayer("Ground"))
        {
            isGround = true;
        }
    }

    /// <summary>
    /// 보스의 중력
    /// </summary>
    private void bossGravity()
    {
        if (isGround == false)
        {
            gravityVelocity -= gravity * Time.deltaTime;
        }
        else if (isGround == true)
        {
            gravityVelocity = -1;
        }

        moveVec.y = gravityVelocity;
        rigid.velocity = moveVec;
    }

    /// <summary>
    /// 보스의 패턴이 변경될 때 체력도 변경
    /// </summary>
    private void patternHpChange()
    {
        if (curPhase > 2)
        {
            return;
        }

        if (curPhase == 1)
        {
            bossRen.color = Color.magenta;
            bossCurHp = bossPhaseHp[1];
        }
        else if (curPhase == 2)
        {
            bossRen.color = Color.red;
            bossCurHp = bossPhaseHp[2];
        }
    }

    /// <summary>
    /// 보스가 모든 페이즈를 사용 후 죽었을 시 삭제
    /// </summary>
    private void bossDeadCheck()
    {
        if (bossCurHp <= 0 && curPhase > 2)
        {
            Destroy(gameObject);
        }
        else if (bossCurHp <= 0)
        {
            phaseChange = true;

            //Color curColor = bossRen.color; 알파값 변경 방법
            //curColor.a = 0.5f;
            //bossRen.color = curColor;
        }
    }
}
