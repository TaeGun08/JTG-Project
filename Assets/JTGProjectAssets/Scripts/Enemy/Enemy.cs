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

    [Header("�� Ÿ��")]
    [SerializeField] private EnemyType enemyType;

    [Header("üũ�� ���̾�")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask trapLayer;

    private Rigidbody2D rigid; //�÷��̾��� ������ٵ�
    private RaycastHit2D hit;
    private BoxCollider2D enemyBoxColl2D;
    private Animator anim;
    private SpriteRenderer enemyRen;

    private bool enemyHitDamage = false;
    private float hitTimer;

    //������ ������ �Ŵ���
    private GameManager gameManager; //���ӸŴ���

    private TrashPreFab trashPreFab;

    [SerializeField] private float gravity; //���ӸŴ������� ������ �߷°��� ������ ����
    private bool isGround = false;
    private float gravityVelocity;

    [Header("���� �⺻ ����")]
    [SerializeField, Tooltip("���� ü��")] private int enemyHp;
    [SerializeField, Tooltip("���� �ִ�ü��")] private int enemyMaxHp;
    [SerializeField, Tooltip("���� ���ݷ�")] private int enemyDamage;
    [SerializeField, Tooltip("���� ����")] private int enemyArmor;

    [Header("�̵�")]
    [SerializeField, Tooltip("���� �̵��ӵ�")] private float speed = 1.0f; //�÷��̾��� �̵��ӵ�
    private Vector2 moveVec; //������ �������� ���� ����
    private bool isTurn = false;
    private float turnTimer;
    private bool moveStop = false;
    private float useMove;
    private bool isRight = false;

    [Header("����")]
    [SerializeField, Tooltip("������ �ϱ� ���� ��")] private float enemyJumpPower = 5.0f;
    private bool isJump = false;

    [Header("���� ����")]
    [SerializeField, Tooltip("���� �Ѿ�")] private GameObject enemyBullet;
    [SerializeField, Tooltip("�Ѿ��� �߻�� ��ġ")] private Transform bulletTrs;
    [SerializeField, Tooltip("������ ������ ��")] private Transform enemyWeaponTrs;
    [SerializeField, Tooltip("���� ���� ������")] private float attackDelay;
    private bool isAttack = false;
    private float isAttackTimer;
    private bool firstAttack = false;

    [Header("���� üũ�� ���� �ݶ��̴�")]
    [SerializeField, Tooltip("���� üũ�� ���� �ݶ��̴�")] private Collider2D jumpCheckColl;
    [SerializeField, Tooltip("�� üũ�� ���� �ݶ��̴�")] private Collider2D wallCheckColl;
    [SerializeField, Tooltip("�� üũ�� ���� �ݶ��̴�")] private Collider2D groundCheckColl;
    [SerializeField, Tooltip("������ ���� �ݶ��̴�")] private CircleCollider2D attackCheckColl;

    [Header("DPS������ ���� �ؽ�Ʈ")]
    [SerializeField] private GameObject dpsObj;

    [Header("���� �׾��� �� �÷��̾�� �� ����ġ")]
    [SerializeField] private float SetExp;

    private void OnDrawGizmos() //�ڽ�ĳ��Ʈ�� ���信�� ������ Ȯ���� �����ϰ� ������
    {
        if (enemyBoxColl2D != null) //�ݶ��̴��� null�� �ƴ϶�� �ڽ����� ������ ���信�� Ȯ���� �� �ְ�
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

        if (gravityVelocity > 0) //gravityVelocity���� 0���� Ŭ ��� �Ʒ� �ڵ尡 ������ ����
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
    /// ������ �������� ����ϴ� �Լ�
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
    /// ���� �ǰ��� �Ծ��� �� Ÿ�ݰ��� �ִ� �Լ�
    /// </summary>
    private void enemyDamageHit()
    {
        if (enemyHitDamage == true)
        {
            enemyRen.color = Color.red;
        }
    }

    /// <summary>
    /// ���� �ǰ� 0�̵Ǹ� �۵��ϴ� �Լ�
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
    /// ������ �ִϸ��̼��� ����ϴ� �Լ�
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
