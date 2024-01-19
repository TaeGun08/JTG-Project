using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    private BoxCollider2D boxColl2D;
    private RaycastHit2D hit;
    private Rigidbody2D rigid;
    private Vector3 moveVec;
    private Animator anim;
    private SpriteRenderer bossRen;

    private GameManager gameManager;

    private TrashPreFab trashPreFab;

    [Header("보스 기본 설정")]
    [SerializeField, Tooltip("보스의 이동속도")] private float bossSpeed;
    [SerializeField, Tooltip("보스의 현재체력")] private int bossCurHp;
    [SerializeField, Tooltip("보스의 패턴별 체력")] private List<int> bossPhaseHp;
    [SerializeField, Tooltip("보스의 방어력")] private int bossArmor;
    private int curPhase; //현재 페이즈
    private bool phaseChange = false; //페이즈 변경을 확인하기 위한 변수
    private float changeTimer; //페이즈 변경 시간
    private bool isGround = false; //땅인지 체크하기 위한 변수
    private float gravity; //중력값을 받아올 변수
    private float gravityVelocity; //중력값을 계산해 적용할 변수
    private bool isRight = false; //오른쪽인지 확인하기 위한 변수
    private bool moveStop = false; //특정행동을 위해 움직임을 멈춰줄 변수
    private bool isScaleChange = false; //공격중 좌우 변경을 멈춰줄 변수
    private bool bossHit = false; //보스가 맞았는지 체크하기 위한 변수
    private float bossHitTimer; //보스가 맞았을 시 스프라이트 색깔을 변경 후 원상태로 돌아오는 시간
    private bool bossDead = false;

    [Header("보스 공격 설정")]
    [SerializeField, Tooltip("보스가 플레이어를 추격하기 위한 콜라이더")] private BoxCollider2D playerChase;
    [SerializeField, Tooltip("보스의 기본 공격을 위한 영역")] private BoxCollider2D AttackCheck;
    [SerializeField, Tooltip("보스의 패턴2 공격을 위한 영역")] private BoxCollider2D telpoArea;
    [SerializeField, Tooltip("보스의 패턴3 공격을 위한 영역")] private BoxCollider2D darkHandArea;
    [SerializeField, Tooltip("보스의 기본 공격 범위")] private BoxCollider2D bossAttack;
    [SerializeField, Tooltip("보스의 패턴3의 공격 프리팹")] private GameObject darkHandPrefab;
    private bool isAttack = false; //플레이어를 공격했는지 확인하기 위한 변수
    private bool attackOn = false; //플레이어를 다시 공격하기 위한 변수
    [SerializeField] private bool patternAttack = false; //기본 공격 시 데미지가 들어가게 하는 변수
    private int randomPattern; //랜덤으로 패턴을 적용시킬 변수
    private float attackDelayTimer; //기본 공격의 딜레이 타이머
    private float telpoDelayTimer; //텔레포트의 딜레이 타이머
    private float darkHandDelayTimer; //다크핸드의 딜레이 타이머
    private bool attackDelayOn = false; //기본 공격 딜레이를 켜주는 변수
    private bool telpoDelayOn = false; //텔레포트 딜레이를 켜주는 변수
    private bool darkHandDelayOn = false; //다크핸드 딜레이를 켜주는 변수
    [Space]
    [SerializeField, Tooltip("보스가 두 번째 패턴을 사용 후 재사용 대기 시간")] private float bossTelpoCoolTime;
    [SerializeField, Tooltip("보스가 세 번째 패턴을 사용 후 재사용 대기 시간")] private float darkHandCoolTime;
    private bool useTelpo = false; //텔포를 사용했는지 알리기 위한 변수
    private bool useDarkHand = false; //타크핸드를 사용했는지 알리기 위한 변수
    private float bossTelpoCoolTimer; //텔레포트의 쿨타이머
    private float darkHandCoolTimer; //다크핸드의 쿨타이머
    private Transform playerTrs; //스킬을 사용하기 위한 플레이어의 Transform

    [Header("보스의 체력을 위한 UI")]
    [SerializeField] private GameObject bossHpObj;
    [SerializeField] private Slider bossHpSlider;
    [SerializeField] private Image bossHpColor;

    [Header("DPS를 위한 오브젝트")]
    [SerializeField] private GameObject dpsObj;

    private float goingEndingTimer; //엔딩으로 넘어가기 위한 타이머

    private void OnDrawGizmos() //박스캐스트를 씬뷰에서 눈으로 확인이 가능하게 보여줌
    {
        if (boxColl2D != null) //콜라이더가 null이 아니라면 박스레이 범위를 씬뷰에서 확인할 수 있게
        {
            Gizmos.color = Color.red;
            Vector3 pos = boxColl2D.bounds.center - new Vector3(0, 0.1f, 0);
            Gizmos.DrawWireCube(pos, boxColl2D.bounds.size);
        }
    }

    private void playerCheck(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Vector3 playerPos = collision.transform.position - transform.position;

            Vector2 scale = transform.localScale;
            scale.x *= -1;
            if (playerPos.x > 2 && transform.localScale.x != -1 
                && isScaleChange == false && phaseChange == false)
            {
                transform.localScale = scale;
                bossSpeed *= -1;
                isRight = true;
            }
            else if (playerPos.x < -2 && transform.localScale.x != 1 
                && isScaleChange == false && phaseChange == false)
            {
                transform.localScale = scale;
                bossSpeed *= -1;
                isRight = false;
            }
        }
    }

    /// <summary>
    /// 기본 공격을 위해 플레이어를 확인하기 위한 함수
    /// </summary>
    /// <param name="collision"></param>
    private void attackCheck(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (isAttack == false && attackOn == false)
            {
                anim.SetTrigger("isPatternA");
                attackDelayOn = true;
                isAttack = true;
                attackOn = true;
                moveStop = true;
                isScaleChange = true;
            }
        }
    }

    /// <summary>
    /// 텔레포트를 위해 플레이어를 확인하기 위한 함수
    /// </summary>
    /// <param name="collision"></param>
    private void telpoAreaCheck(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (isAttack == false && useTelpo == false &&
                attackOn == false && curPhase >= 1 && randomPattern == 1)
            {
                anim.SetBool("isPatternB_bool", true);
                playerTrs = collision.transform;
                telpoDelayOn = true;
                useTelpo = true;
                isAttack = true;
                attackOn = true;
                moveStop = true;
                isScaleChange = true;
            }
        }
    }

    /// <summary>
    /// 다크핸드를 위해 플레이어를 확인하기 위한 함수
    /// </summary>
    /// <param name="collision"></param>
    private void darkHandCheck(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (isAttack == false && useDarkHand == false && attackOn == false 
                && curPhase == 2 && randomPattern == 2)
            {
                anim.SetTrigger("isPatternC");
                darkHandDelayOn = true;
                playerTrs = collision.transform;
                useDarkHand = true;
                isAttack = true;
                attackOn = true;
                moveStop = true;
                isScaleChange = true;
            }
        }
    }

    /// <summary>
    /// 기본 공격의 범위에서 플레이어가 있으면 데미지를 넣어주는 함수
    /// </summary>
    /// <param name="collision"></param>
    private void bossAttackArea(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (patternAttack == true)
            {
                Player playerSc = collision.gameObject.GetComponent<Player>();
                if (curPhase == 0)
                {
                    playerSc.PlayerCurHp(10, true, false);

                    if (isRight == false)
                    {
                        playerSc.PlayerKnockBack(-10, 2);
                    }
                    else if (isRight == true)
                    {
                        playerSc.PlayerKnockBack(10, 2);
                    }
                }
                else if (curPhase == 1)
                {
                    playerSc.PlayerCurHp(20, true, false);

                    if (isRight == false)
                    {
                        playerSc.PlayerKnockBack(-20, 2);
                    }
                    else if (isRight == true)
                    {
                        playerSc.PlayerKnockBack(20, 2);
                    }
                }
                else if (curPhase == 2)
                {
                    playerSc.PlayerCurHp(20, true, true);

                    if (isRight == false)
                    {
                        playerSc.PlayerKnockBack(-30, 2);
                    }
                    else if (isRight == true)
                    {
                        playerSc.PlayerKnockBack(30, 2);
                    }
                }

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

        moveVec.x = 1;
    }

    private void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        bossRen = transform.GetChild(0).GetComponent<SpriteRenderer>();

        gameManager = GameManager.Instance;

        trashPreFab = TrashPreFab.Instance;

        gravity = gameManager.GravityScale();

        Color bossHp = bossHpColor.color;
        bossHp.r = 0.6977215f;
        bossHp.g = 0.4577697f;
        bossHp.b = 0.764151f;
        bossHpColor.color = bossHp;
    }

    private void Update()
    {
        if (bossDead == true)
        {
            goingEndingTimer += Time.deltaTime;
            if (goingEndingTimer > 3)
            {
                ending();
                bossDead = false;
            }
            else if (goingEndingTimer > 1)
            {
                bossRen.enabled = false;
                bossHpObj.SetActive(false);
            }
        }

        if (gameManager.GetGamePause() == true || bossDead == true)
        {
            return;
        }

        playerCollCheck();
        timers();
        bossGroundCheck();
        bossMove();
        bossGravity();
        bossDeadCheck();
        bossHpCheck();
        BossHit();
        bossAni();
    }

    /// <summary>
    /// 보스가 플레이어를 체크하여 공격할 수 있게 담당을 하는 함수
    /// </summary>
    private void playerCollCheck()
    {
        Collider2D playerChaseCheck = Physics2D.OverlapBox(playerChase.bounds.center,
            playerChase.bounds.size, 0, LayerMask.GetMask("Player"));

        Collider2D patternAttackCheck = Physics2D.OverlapBox(AttackCheck.bounds.center,
            AttackCheck.bounds.size, 0, LayerMask.GetMask("Player"));

        Collider2D patternTelpoCheck = Physics2D.OverlapBox(telpoArea.bounds.center,
            telpoArea.bounds.size, 0, LayerMask.GetMask("Player"));

        Collider2D patternDarkHandCheck = Physics2D.OverlapBox(darkHandArea.bounds.center,
            darkHandArea.bounds.size, 0, LayerMask.GetMask("Player"));

        Collider2D patternBossAttack = Physics2D.OverlapBox(bossAttack.bounds.center,
            bossAttack.bounds.size, 0, LayerMask.GetMask("Player"));

        if (playerChaseCheck != null)
        {
            playerCheck(playerChaseCheck);
        }

        if (patternAttackCheck != null)
        {
            attackCheck(patternAttackCheck);
        }

        if (patternTelpoCheck != null)
        {
            telpoAreaCheck(patternTelpoCheck);
        }

        if (patternDarkHandCheck != null)
        {
            darkHandCheck(patternDarkHandCheck);
        }

        if (patternBossAttack != null)
        {
            bossAttackArea(patternBossAttack);
        }
        else
        {
            patternAttack = false;
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

        if (isAttack == true) //공격을 하면 바로 공격을 할 수 없게 만드는 코드
        {
            attackDelayTimer += Time.deltaTime;
            if (attackDelayTimer > 2 )
            {
                attackDelayTimer = 0;
                if (curPhase == 1) //2페이즈면 실행되는 코드
                {
                    if (useTelpo == false)
                    {
                        randomPattern = 1;
                    }
                }
                else if (curPhase == 2) //3페이즈면 실행되는 코드
                {
                    if (useTelpo == false && useDarkHand == false)
                    {
                        int randomPat = Random.Range(1, 3);
                        randomPattern = randomPat;
                    }
                    else if (useTelpo == true && useDarkHand == false)
                    {
                        randomPattern = 2;
                    }
                    else if (useTelpo == false && useDarkHand == true)
                    {
                        randomPattern = 1;
                    }
                }
                isAttack = false;
                moveStop = false;
                isScaleChange = false;
            }
        }

        if (attackDelayOn == true) //보스의 기본 공격 딜레이
        {
            attackDelayTimer += Time.deltaTime;
            if (attackDelayTimer > 0.8f)
            {
                patternAttack = true;
                attackOn = false;
                attackDelayTimer = 0;
                attackDelayOn = false;
            }
        }

        if (telpoDelayOn == true) //보스의 텔레포트 딜레이
        {
            telpoDelayTimer += Time.deltaTime;
            if (telpoDelayTimer > 0.8f)
            {
                transform.position = playerTrs.position + new Vector3(0, 1, 0);
                anim.SetBool("isPatternB_bool", false);
                anim.SetTrigger("isPatternB_trigger");
                telpoDelayTimer = 0;
                telpoDelayOn = false;
                attackOn = false;
                isScaleChange = false;
            }
        }

        if (darkHandDelayOn == true) //보스의 다크핸드 공격 딜레이
        {
            darkHandDelayTimer += Time.deltaTime;
            if (darkHandDelayTimer > 0.8f)
            {
                Instantiate(darkHandPrefab,  new Vector3(playerTrs.position.x, -18.3f, 0), Quaternion.identity, trashPreFab.transform);
                darkHandDelayTimer = 0;
                darkHandDelayOn = false;
                attackOn = false;
            }
        }

        if (useTelpo == true) //보스의 텔레포트 쿨타임
        {
            bossTelpoCoolTimer += Time.deltaTime;
            if (bossTelpoCoolTimer >= bossTelpoCoolTime)
            {
                bossTelpoCoolTimer = 0;
                useTelpo = false;
            }
        }

        if (useDarkHand == true) //보스의 다크핸드 패턴의 쿨타임
        {
            darkHandCoolTimer += Time.deltaTime;
            if (darkHandCoolTimer >= darkHandCoolTime)
            {
                darkHandCoolTimer = 0;
                useDarkHand = false;
            }
        }

        if(bossHit == true)
        {
            bossHitTimer += Time.deltaTime;
            if (bossHitTimer > 0.1f)
            {
                if (curPhase == 0)
                {
                    bossRen.color = Color.white;
                }
                else if (curPhase == 1)
                {
                    bossRen.color = Color.magenta;
                }
                else if (curPhase == 2)
                {
                    bossRen.color = Color.red;
                }
                bossHitTimer = 0;
                bossHit = false;
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
    /// 보스의 기본 움직임
    /// </summary>
    private void bossMove()
    {
        if (moveStop == true)
        {
            moveVec.x = 0;
            return;
        }

        moveVec.x = -bossSpeed;
        rigid.velocity = moveVec;
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
            Color bossHp = bossHpColor.color;
            bossHp.r = 0.4426705f;
            bossHp.g = 0;
            bossHp.b = 0.5660378f;
            bossHpColor.color = bossHp;
            moveStop = false;
        }
        else if (curPhase == 2)
        {
            bossRen.color = Color.red;
            bossCurHp = bossPhaseHp[2];
            Color bossHp = bossHpColor.color;
            bossHp.r = 0.5647059f;
            bossHp.g = 0;
            bossHp.b = 0.04554264f;
            bossHpColor.color = bossHp;
            moveStop = false;
        }
    }

    /// <summary>
    /// 보스가 모든 페이즈를 사용 후 죽었을 시 삭제
    /// </summary>
    private void bossDeadCheck()
    {
        if (bossCurHp <= 0 && curPhase > 2)
        {
            bossRen.color = Color.white;
            anim.SetBool("isDead", true);
            bossDead = true;
        }
        else if (bossCurHp <= 0)
        {
            phaseChange = true;
            moveStop = true;
            isAttack = true;

            //Color curColor = bossRen.color; 알파값 변경 방법
            //curColor.a = 0.5f;
            //bossRen.color = curColor;
        }
    }

    /// <summary>
    /// 보스 Hp를 슬라이더에 실시간으로 적용시키기 위한 함수
    /// </summary>
    private void bossHpCheck()
    {
        if (curPhase == 0)
        {
            bossHpSlider.maxValue = bossPhaseHp[0];
            bossHpSlider.value = bossCurHp;
        }
        else if (curPhase == 1)
        {
            bossHpSlider.maxValue = bossPhaseHp[1];
            bossHpSlider.value = bossCurHp;
        }
        else if (curPhase == 2)
        {
            bossHpSlider.maxValue = bossPhaseHp[2];
            bossHpSlider.value = bossCurHp;
        }
    }

    private void bossDpsCheck(int _dps, bool _trueDmg, bool _critical)
    {
        Vector3 enemyPos = transform.position;
        enemyPos.x += Random.Range(-0.4f, 0.4f);
        enemyPos.y += Random.Range(1.2f, 1.5f);
        TMP_Text dpsText = dpsObj.transform.GetComponentInChildren<TMP_Text>();
        dpsText.text = _dps.ToString();
        if (_critical == false)
        {
            if (_trueDmg == false)
            {
                dpsText.color = Color.white;
            }
            else if (_trueDmg == true)
            {
                dpsText.color = Color.blue;
            }
        }
        else if (_critical == true)
        {
            dpsText.color = Color.red;
        }
        Instantiate(dpsObj, enemyPos, Quaternion.identity, trashPreFab.transform);
    }

    /// <summary>
    /// 보스가 맞았을 때 알파값을 낮추는 함수
    /// </summary>
    private void BossHit()
    {
        if (bossHit == true)
        {
            Color bossColor = bossRen.color;
            bossColor.a = 0.8f;
            bossRen.color = bossColor;
        }
    }

    private void ending()
    {
        SceneManager.LoadScene("EndingScene");
    }

    /// <summary>
    /// 보스의 기본적인 애니메이션을 담당하는 함수
    /// </summary>
    private void bossAni()
    {
        anim.SetInteger("isWalk", (int)moveVec.x);
    }

    public void BossHp(int _damage, bool _hit, bool _trueDam, bool _critical)
    {
        if (phaseChange == true)
        {
            return;
        }

        if (_trueDam == true)
        {
            bossCurHp -= _damage;
            bossHit = _hit;
            bossDpsCheck(_damage, _trueDam, _critical);
        }
        else if (_trueDam == false)
        {
            int dmgReduction = _damage - bossArmor;
            if (dmgReduction <= 0)
            {
                bossCurHp -= 1;
                bossHit = _hit;
                bossDpsCheck(1, _trueDam, _critical);
            }
            else if (dmgReduction > 0)
            {
                bossCurHp -= dmgReduction;
                bossHit = _hit;
                bossDpsCheck(dmgReduction, _trueDam, _critical);
            }
        }
    }
}
