using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        enemyA,
        enemyB,
        enemyC,
    }

    [Header("적의 형태")]
    [SerializeField] private EnemyType enemyType;

    [Header("체크할 레이어")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask trapLayer;

    private Rigidbody2D rigid; //플레이어의 리지드바디
    private RaycastHit2D hit;
    private BoxCollider2D enemyBoxColl2D;
    private Animator anim;
    private SpriteRenderer enemyRen;

    private bool enemyHitDamage = false;
    private float hitTimer;

    //적에게 가져올 매니저
    private GameManager gameManager; //게임매니저

    private TrashPreFab trashPreFab;

    [SerializeField] private float gravity; //게임매니저에서 가져올 중력값을 저장할 변수
    private bool isGround = false;
    private float gravityVelocity;

    [Header("적의 기본 설정")]
    [SerializeField, Tooltip("적의 체력")] private int enemyHp;
    [SerializeField, Tooltip("적의 최대체력")] private int enemyMaxHp;
    [SerializeField, Tooltip("적의 공격력")] private int enemyDamage;
    [SerializeField, Tooltip("적의 방어력")] private int enemyArmor;

    [Header("이동")]
    [SerializeField, Tooltip("적의 이동속도")] private float speed = 1.0f; //플레이어의 이동속도
    private Vector2 moveVec; //적군의 움직임을 위한 벡터
    private bool isTurn = false;
    private float turnTimer;
    private bool moveStop = false;
    private float useMove;
    private bool isRight = false;

    [Header("점프")]
    [SerializeField, Tooltip("점프를 하기 위한 힘")] private float enemyJumpPower = 5.0f;
    private bool isJump = false;

    [Header("공격 설정")]
    [SerializeField, Tooltip("적의 총알")] private GameObject enemyBullet;
    [SerializeField, Tooltip("총알이 발사될 위치")] private Transform bulletTrs;
    [SerializeField, Tooltip("유저를 저격할 총")] private Transform enemyWeaponTrs;
    [SerializeField, Tooltip("적의 공격 딜레이")] private float attackDelay;
    private bool isAttack = false;
    private float isAttackTimer;
    private bool firstAttack = false;

    [Header("적이 체크를 위한 콜라이더")]
    [SerializeField, Tooltip("점프 체크를 위한 콜라이더")] private Collider2D jumpCheckColl;
    [SerializeField, Tooltip("벽 체크를 위한 콜라이더")] private Collider2D wallCheckColl;
    [SerializeField, Tooltip("땅 체크를 위한 콜라이더")] private Collider2D groundCheckColl;
    [SerializeField, Tooltip("공격을 위한 콜라이더")] private CircleCollider2D attackCheckColl;

    [Header("DPS측정을 위한 텍스트")]
    [SerializeField] private GameObject dpsObj;

    [Header("적이 죽었을 때 플레이어에게 줄 경험치")]
    [SerializeField] private float SetExp;

    private void OnDrawGizmos() //박스캐스트를 씬뷰에서 눈으로 확인이 가능하게 보여줌
    {
        if (enemyBoxColl2D != null) //콜라이더가 null이 아니라면 박스레이 범위를 씬뷰에서 확인할 수 있게
        {
            Gizmos.color = Color.red;
            Vector3 pos = enemyBoxColl2D.bounds.center - new Vector3(0, 0.1f, 0);
            Gizmos.DrawWireCube(pos, enemyBoxColl2D.bounds.size);
        }
    }

    private void attackTrigger(Collider2D _collision)
    {
        if (_collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Vector3 playerPos = _collision.gameObject.transform.position - transform.position;

            if (isAttack == false)
            {
                if (enemyType.ToString() == "enemyA")
                {
                    float angle = Quaternion.FromToRotation(Vector3.up, playerPos).eulerAngles.z;
                    GameObject bulletObj = Instantiate(enemyBullet, bulletTrs.position, Quaternion.Euler(0, 0, angle), trashPreFab.transform);
                    Bullet bulletSc = bulletObj.GetComponent<Bullet>();
                    bulletSc.BulletDamage(enemyDamage, 0, false, false);
                }
                else if (enemyType.ToString() == "enemyB")
                {
                    float angle = Quaternion.FromToRotation(Vector3.up, playerPos).eulerAngles.z;
                    GameObject bulletObj = Instantiate(enemyBullet, bulletTrs.position, Quaternion.Euler(0, 0, angle), trashPreFab.transform);
                    GameObject bulletObjB = Instantiate(enemyBullet, bulletTrs.position, Quaternion.Euler(0, 0, angle - 15), trashPreFab.transform);
                    GameObject bulletObjC = Instantiate(enemyBullet, bulletTrs.position, Quaternion.Euler(0, 0, angle + 15), trashPreFab.transform);
                    Bullet bulletSc = bulletObj.GetComponent<Bullet>();
                    Bullet bulletScB = bulletObjB.GetComponent<Bullet>();
                    Bullet bulletScC = bulletObjC.GetComponent<Bullet>();
                    bulletSc.BulletDamage(enemyDamage, 0, false, false);
                    bulletScB.BulletDamage(enemyDamage, 0, false, false);
                    bulletScC.BulletDamage(enemyDamage, 0, false, false);
                }
                else if (enemyType.ToString() == "enemyC")
                {
                    float angle = Quaternion.FromToRotation(Vector3.up, playerPos).eulerAngles.z;
                    GameObject bulletObj = Instantiate(enemyBullet, bulletTrs.position, Quaternion.Euler(0, 0, angle), trashPreFab.transform);
                    Bullet bulletSc = bulletObj.GetComponent<Bullet>();
                    bulletSc.BulletDamage(enemyDamage, 0, false, false);
                }

                isAttack = true;
            }               

            moveStop = true;
            useMove = 1;

            Vector2 scale = transform.localScale;
            scale.x *= -1;
            if (playerPos.x > 0 && transform.localScale.x != 1)
            {
                transform.localScale = scale;
                speed *= -1;
                isRight = true;
            }
            else if (playerPos.x < 0 && transform.localScale.x != -1)
            {
                transform.localScale = scale;
                speed *= -1;
                isRight = false;
            }

            if (isRight == true)
            {
                Vector3 shootAim = Vector3.right;
                float weaponAngle = Quaternion.FromToRotation(shootAim, playerPos).eulerAngles.z;
                enemyWeaponTrs.rotation = Quaternion.Euler(0, 0, weaponAngle);
            }
            else if (isRight == false)
            {
                Vector3 shootAim = Vector3.left;
                float weaponAngle = Quaternion.FromToRotation(shootAim, playerPos).eulerAngles.z;
                enemyWeaponTrs.rotation = Quaternion.Euler(0, 0, weaponAngle);
            }
        }
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        enemyBoxColl2D = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        enemyRen = GetComponent<SpriteRenderer>();

        enemyHp = enemyMaxHp;

        isAttackTimer = 0;

        hitTimer = 0.1f;

        isRight = true;
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        trashPreFab = TrashPreFab.Instance;

        gravity = gameManager.GravityScale();

        enemyTurn();
    }

    private void Update()
    {
        if (gameManager.GetGamePause() == true)
        {
            return;
        }

        enemyTimer();
        enemyAttackCollCheck();
        enemyGroundCheck();
        wallCheck();
        enemyMove();
        enemyGravity();
        enemyDamageHit();
        enemyDead();
        enemyAni();
    }

    private void enemyTimer()
    {
        if (isTurn == true)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer < 0)
            {
                isTurn = false;
                turnTimer = 0.2f;
            }
        }

        if (isAttack == true)
        {
            isAttackTimer += Time.deltaTime;
            if (isAttackTimer > attackDelay)
            {
                isAttack = false;
                isAttackTimer = 0;
            }
        }

        useMove -= Time.deltaTime;
        if (useMove < 0)
        {
            moveStop = false;
        }

        if (enemyHitDamage == true)
        {
            hitTimer -= Time.deltaTime;
            if (hitTimer < 0)
            {
                hitTimer = 0.1f;
                enemyHitDamage = false;
                enemyRen.color = Color.white;
            }
        }
    }

    private void enemyAttackCollCheck()
    {
        Collider2D attackColl = Physics2D.OverlapCircle(attackCheckColl.bounds.center,
            attackCheckColl.radius, LayerMask.GetMask("Player"));

        if (attackColl != null)
        {
            if (firstAttack == false)
            {
                isAttack = true;
                firstAttack = true;
            }
            attackTrigger(attackColl);
        }

        if (moveStop == false)
        {
            enemyWeaponTrs.rotation = Quaternion.identity;
        }
    }

    private void enemyGroundCheck()
    {
        isGround = false;

        if (gravityVelocity > 0) //gravityVelocity값이 0보다 클 경우 아래 코드가 실행을 멈춤
        {
            return;
        }

        hit = Physics2D.BoxCast(enemyBoxColl2D.bounds.center, enemyBoxColl2D.bounds.size,
            0, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));

        if (hit.transform != null && hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGround = true;
        }
    }

    private void wallCheck()
    {
        if (isGround == false)
        {
            return;
        }

        if (wallCheckColl.IsTouchingLayers(groundLayer) == true
            && jumpCheckColl.IsTouchingLayers(groundLayer) == false)
        {
            isJump = true;
        }
        else if (wallCheckColl.IsTouchingLayers(groundLayer) == true
            && jumpCheckColl.IsTouchingLayers(groundLayer) == true)
        {
            isTurn = true;
            enemyTurn();
        }
        else if (wallCheckColl.IsTouchingLayers(groundLayer) == false
            && jumpCheckColl.IsTouchingLayers(groundLayer) == false
            && groundCheckColl.IsTouchingLayers(groundLayer) == false)
        {
            isTurn = false;
            enemyTurn();
        }
        else if (groundCheckColl.IsTouchingLayers(trapLayer) == true)
        {
            isTurn = false;
            enemyTurn();
        }
    }

    /// <summary>
    /// 적군의 움직임을 담당하는 함수
    /// </summary>
    private void enemyMove()
    {
        if (moveStop == true)
        {
            moveVec.x = 0;
            return;
        }

        moveVec.x = speed;
        rigid.velocity = moveVec;
    }

    private void enemyTurn()
    {
        if (isTurn == false)
        {
            Vector3 turnVec = transform.localScale;
            turnVec.x *= -1;
            transform.localScale = turnVec;

            speed *= -1;

            if (speed > 0)
            {
                isRight = true;
            }
            else if (speed < 0)
            {
                isRight = false;
            }
        }
    }

    private void enemyGravity()
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

        if (moveStop == true)
        {
            return;
        }

        if (isJump == true)
        {
            gravityVelocity = enemyJumpPower;
            isJump = false;
        }
    }

    /// <summary>
    /// 적이 피격을 입었을 때 타격감을 주는 함수
    /// </summary>
    private void enemyDamageHit()
    {
        if (enemyHitDamage == true)
        {
            enemyRen.color = Color.red;
        }
    }

    /// <summary>
    /// 적의 피가 0이되면 작동하는 함수
    /// </summary>
    private void enemyDead()
    {
        if (enemyHp <= 0)
        {
            Player playerSc = gameManager.PlayerPrefab().GetComponent<Player>();
            playerSc.SetPlayerExp(SetExp);
            enemyHp = enemyMaxHp;
            firstAttack = false;
            isAttack = true;
            isAttackTimer = 0;
            hitTimer = 0.1f;
            enemyHitDamage = false;
            enemyRen.color = Color.white;
            gameObject.SetActive(false);
        }
    }

    private void enemyDpsCheck(int _dps, bool _trueDmg, bool _critical)
    {
        Vector3 enemyPos = transform.position;
        enemyPos.x += Random.Range(-0.4f, 0.4f);
        enemyPos.y += Random.Range(0.2f, 0.5f);
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
    /// 적군의 애니메이션을 담당하는 함수
    /// </summary>
    private void enemyAni()
    {
        if (isGround == false)
        {
            anim.SetInteger("isWalk", 0);
            return;
        }

        anim.SetInteger("isWalk", (int)moveVec.x);
    }

    public void EnemyHp(int _damage, bool _hit, bool _trueDam, bool _critical)
    {
        if (_trueDam == true)
        {
            enemyHp -= _damage;
            enemyHitDamage = _hit;
            enemyDpsCheck(_damage, _trueDam, _critical);
        }
        else if (_trueDam == false)
        {
            int dmgReduction = _damage - enemyArmor;
            if (dmgReduction <= 0)
            {
                enemyHp -= 1;
                enemyHitDamage = _hit;
                enemyDpsCheck(1, _trueDam, _critical);
            }
            else if (dmgReduction > 0)
            {
                enemyHp -= dmgReduction;
                enemyHitDamage = _hit;
                enemyDpsCheck(dmgReduction, _trueDam, _critical);
            }
        }
    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }
}
