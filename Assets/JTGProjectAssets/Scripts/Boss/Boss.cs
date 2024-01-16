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
    [SerializeField, Tooltip("보스의 기본 공격을 위한 영역")] private BoxCollider2D pattern1AttackCheck;
    [SerializeField, Tooltip("보스의 패턴2 공격을 위한 영역")] private BoxCollider2D pattern2Attack;
    [SerializeField, Tooltip("보스의 패턴3 공격을 위한 영역")] private BoxCollider2D pattern3Attack;
    [SerializeField, Tooltip("보스의 기본 공격 범위")] private BoxCollider2D pattern1Attack;
    [SerializeField, Tooltip("보스의 패턴3의 공격 프리팹")] private GameObject patternAttackPrefab;
    private bool isAttack = false;
    private bool patternAttack = false;
    [SerializeField] private int randomPattern;
    private float attack1DelayTimer;
    private float attack2DelayTimer;
    private float attack3DelayTimer;
    private bool attack1DelayOn = false;
    private bool attack2DelayOn = false;
    private bool attack3DelayOn = false;
    [Space]
    [SerializeField, Tooltip("보스가 두 번째 패턴을 사용 후 재사용 대기 시간")] private float pattern2CoolTime;
    [SerializeField, Tooltip("보스가 세 번째 패턴을 사용 후 재사용 대기 시간")] private float pattern3CoolTime;
    private bool usePattern2 = false;
    private bool usePattern3 = false;
    private Transform playerTrs;

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
            if (isAttack == false && randomPattern == 0)
            {
                anim.SetTrigger("isPatternA");
                attack1DelayOn = true;
                isAttack = true;
            }
        }
    }

    private void onTriggerArea2Check(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (usePattern2 == true && randomPattern == 1)
            {
                return;
            }

            if (isAttack == false && curPhase >= 1 && randomPattern == 1)
            {
                anim.SetBool("isPatternB_bool", true);
                playerTrs = collision.transform;
                attack2DelayOn = true;
                usePattern2 = true;
            }
        }
    }

    private void onTriggerArea3Check(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (usePattern3 == true && randomPattern == 2)
            {
                return;
            }

            if (isAttack == false && curPhase == 2 && randomPattern == 2)
            {
                anim.SetTrigger("isPatternC");
                attack3DelayOn = true;
                playerTrs = collision.transform;
                usePattern3 = true;
                isAttack = true;
            }
        }
    }

    private void onTriggerAttackArea(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (patternAttack == true)
            {
                Player playerSc = collision.gameObject.GetComponent<Player>();
                playerSc.PlayerCurHp(10, true, false);
                patternAttack = false;
            }
        }        
    }

    private void Awake()
    {
        boxColl2D = GetComponent<BoxCollider2D>();
        rigid = GetComponent<Rigidbody2D>();

        bossCurHp = bossPhaseHp[0];

        changeTimer = 0;
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
        Collider2D pattern1 = Physics2D.OverlapBox(pattern1AttackCheck.bounds.center,
            pattern1AttackCheck.bounds.size, 0, LayerMask.GetMask("Player"));

        Collider2D pattern2 = Physics2D.OverlapBox(pattern2Attack.bounds.center,
            pattern2Attack.bounds.size, 0, LayerMask.GetMask("Player"));

        Collider2D pattern3 = Physics2D.OverlapBox(pattern3Attack.bounds.center,
            pattern3Attack.bounds.size, 0, LayerMask.GetMask("Player"));

        Collider2D patternAtt = Physics2D.OverlapBox(pattern1Attack.bounds.center,
            pattern1Attack.bounds.size, 0, LayerMask.GetMask("Player"));

        if (pattern1 != null)
        {
            onTriggerArea1Check(pattern1);
        }

        if (pattern2 != null)
        {
            onTriggerArea2Check(pattern2);
        }

        if (pattern3 != null)
        {
            onTriggerArea3Check(pattern3);
        }

        if (patternAtt != null)
        {
            onTriggerAttackArea(patternAtt);
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
            attack1DelayTimer += Time.deltaTime;
            if (attack1DelayTimer > 3)
            {
                attack1DelayTimer = 0;
                if (curPhase == 1)
                {
                    int randomPat = Random.Range(0, 2);
                    randomPattern = randomPat;
                }
                else if (curPhase == 2)
                {
                    int randomPat = Random.Range(0, 3);
                    randomPattern = randomPat;
                }
                isAttack = false;
            }
        }

        if (attack1DelayOn == true)
        {
            attack1DelayTimer += Time.deltaTime;
            if (attack1DelayTimer > 0.8f)
            {
                attack1DelayTimer = 0;
                attack1DelayOn = false;
                patternAttack = true;
            }
        }

        if (attack2DelayOn == true)
        {

        }

        if (attack3DelayOn == true)
        {
            attack3DelayTimer += Time.deltaTime;
            if (attack3DelayTimer > 0.8f)
            {
                Instantiate(patternAttackPrefab, playerTrs.position + new Vector3(0, 2.5f, 0), Quaternion.identity, transform);
                attack3DelayTimer = 0;
                attack3DelayOn = false;
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
