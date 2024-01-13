using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Pet : MonoBehaviour
{
    public enum PetType
    {
        petTypeA = 1,
        petTypeB,
        petTypeC,
        petTypeD,
    }

    [Header("펫이 올려줄 플레이어 능력치 타입")]
    [SerializeField] private PetType petType;

    [Header("특정 오브젝트를 체크하기 위한 콜라이더")]
    [SerializeField] private CircleCollider2D playerCheck;
    [SerializeField] private CircleCollider2D enemyCheck;

    private Rigidbody2D rigid;
    private BoxCollider2D petBoxColl2D;
    private RaycastHit2D hit;
    private Vector3 moveVec;
    private Animator anim;

    private GameManager gameManager; //게임매니저

    private TrashPreFab trashPreFab;

    private float gravity; //펫의 중력
    private bool isGround = false; //땅인이 아닌지 확인을 위한 변수
    private float gravityVelocity; //리지드바디의 Y값을 제어하기 위한 변수

    private Player playerSc; //플레이어 스크립트
    private Transform playerTrs; //플레이어의 트랜스폼
    private Vector3 playerPos; //플레이어의 포지션
    private float followPosX; //펫이 따라갈 x포지션
    private float followPosY; //펫이 확인할 Y포지션
    private Enemy enemySc;

    [SerializeField] private GameObject pickUpKeyImage;

    [Header("펫의 기본 설정")]
    [SerializeField, Tooltip("펫의 이동속도")] private float speed;
    [SerializeField, Tooltip("플레이어가 펫을 얻었는지 체크해주는 변수")] private bool getPet = false;
    private bool playerIn = false; //플레이어가 자신의 영역안에 있는지 체크
    private bool petRun = false; //펫이 달리는지 안 달리는지를 체크
    private bool isIdle = false; //펫이 가만히 있는지를 체크
    private float motionTimer; //모션을 재생하기 위한 타이머
    private bool motionOn = false; //모션이 재생되었는지를 확인할 변수
    private bool moveOff = false; //모션이 재생되었을 때 움직임을 멈춰줄 변수
    private float moveOnTimer; //다시 움직이게 만들어주는 타이머
    private bool runStop = false; //달리는 애니메이션을 제어해줄 변수
    private float runStopTimer; //달리는 모션을 멈추게 하는 타이머

    [Header("펫의 능력치 증가 패시브")]
    [SerializeField, Tooltip("패시브 효과 공격력 증가")] private int petDamageEffect;
    [Space]
    [SerializeField, Tooltip("패시브 효과 방어력 증가")] private int petArmorEffect;
    [Space]
    [SerializeField, Tooltip("패시브 효과 체력 증가")] private int petHpEffect;
    [Space]
    [SerializeField, Tooltip("패시브 효과 치명타확률 증가")] private float petCriPcentEffect;
    [SerializeField, Tooltip("패시브 효과 치명타데미지 증가")] private float petCriDamageEffect;
    private bool petPassiveOn = false;

    [Header("펫의 능력")]
    [SerializeField, Tooltip("펫의 공격력")] private float petDamage;
    [SerializeField, Tooltip("펫의 공격 프리팹")] private GameObject petAttackpreFab;
    [SerializeField, Tooltip("펫이 공격시 펫에게 생성되는 이펙트")] private GameObject petSkillEffect;
    [SerializeField, Tooltip("펫의 스킬쿨타임")] private float petSkillTime;
    [SerializeField, Tooltip("펫이 공격시 데미지가 들어가는 딜레이")] private float attackDelayTime;
    private bool uesPetSkill = false;
    private float petSkillTimer;
    private float attackDelayTimer;
    private bool isAttack;
    private GameObject petSkilAttacklObj;
    private GameObject petSkillEfObj;

    private void OnDrawGizmos() //박스캐스트를 씬뷰에서 눈으로 확인이 가능하게 보여줌
    {
        if (petBoxColl2D != null) //콜라이더가 null이 아니라면 박스레이 범위를 씬뷰에서 확인할 수 있게
        {
            Gizmos.color = Color.red;
            Vector3 pos = petBoxColl2D.bounds.center - new Vector3(0, 0.1f, 0);
            Gizmos.DrawWireCube(pos, petBoxColl2D.bounds.size);
        }
    }

    private void checkTrigger(Collider2D _collision)
    {
        if (_collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerIn = true;

            playerSc = _collision.gameObject.GetComponent<Player>();
            playerTrs = _collision.gameObject.transform;
            playerPos = _collision.gameObject.transform.position;
            followPosX = playerPos.x - transform.position.x;
            followPosY = playerPos.y - transform.position.y;

            Vector3 scale = transform.localScale;
            scale.x *= -1;

            if (transform.localScale.x != 1 && followPosX > 0 && moveOff == false && motionOn == false)
            {
                transform.localScale = scale;
            }
            else if (transform.localScale.x != -1 && followPosX < 0 && moveOff == false && motionOn == false)
            {
                transform.localScale = scale;
            }
        }
        else if (_collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (petType.ToString() == "petTypeA")
            {
                enemySc = _collision.gameObject.GetComponent<Enemy>();
                if (uesPetSkill == false)
                {
                    petSkilAttacklObj = Instantiate(petAttackpreFab, _collision.gameObject.transform.position, Quaternion.identity, trashPreFab.transform);
                    petSkilAttacklObj.transform.SetParent(_collision.gameObject.transform);

                    Vector3 petPos = transform.position;
                    petPos.y += 0.8f;

                    petSkillEfObj = Instantiate(petSkillEffect, petPos, Quaternion.identity, trashPreFab.transform);
                    petSkillEfObj.transform.SetParent(gameObject.transform);
                    uesPetSkill = true;
                    isAttack = true;
                }
            }
            else if (petType.ToString() == "petTypeB")
            {

            }
            else if (petType.ToString() == "petTypeC")
            {

            }
            else if (petType.ToString() == "petTypeD")
            {

            }
        }
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        petBoxColl2D = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        motionTimer = 0;

        petSkillTimer = petSkillTime;

        attackDelayTimer = attackDelayTime;
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        trashPreFab = TrashPreFab.instance;

        gravity = gameManager.GravityScale();
    }

    private void Update()
    {
        if (gameManager.GetGamePause() == true)
        {
            return;
        }

        petTimer();
        colliderCheck();
        petGroundCheck();
        petPos();
        petMove();
        petGravity();
        petPassiveEffect();
        petAni();
    }

    private void petTimer()
    {
        if (isIdle == true)
        {
            motionTimer += Time.deltaTime;
            if (motionTimer >= 6)
            {
                motionOn = true;
                moveVec.x = 0;
            }
        }
        else if (isIdle == false && motionOn == true)
        {
            motionOn = false;
            moveOff = true;
        }

        if (moveOff == true)
        {
            moveOnTimer += Time.deltaTime;
            if (moveOnTimer > 1)
            {
                moveOff = false;
                moveOnTimer = 0;
            }
        }

        if (runStop == true)
        {
            runStopTimer += Time.deltaTime;
            if (runStopTimer > 0.3f)
            {
                petRun = false;
            }
        }

        if (uesPetSkill == true)
        {
            petSkillTimer -= Time.deltaTime;
            if (petSkillTimer < 0)
            {
                uesPetSkill = false;
                petSkillTimer = petSkillTime;
            }
        }

        if (isAttack == true)
        {
            attackDelayTimer -= Time.deltaTime;
            if (attackDelayTimer < 0)
            {
                enemySc.EnemyHp((int)petDamage, true, true, false);
                Destroy(petSkillEfObj);
                attackDelayTimer = attackDelayTime;
                isAttack = false;
            }
        }
    }

    private void colliderCheck()
    {
        if (getPet == false)
        {
            pickUpKeyImage.SetActive(true);
            return;
        }
        else if (getPet == true)
        {
            pickUpKeyImage.SetActive(false);
        }

        playerIn = false;

        Collider2D playerColl = Physics2D.OverlapCircle(playerCheck.bounds.center,
        playerCheck.radius, LayerMask.GetMask("Player"));

        Collider2D enemyColl = Physics2D.OverlapCircle(enemyCheck.bounds.center,
        enemyCheck.radius, LayerMask.GetMask("Enemy"));

        if (playerColl != null)
        {
            checkTrigger(playerColl);
        }

        if (enemyColl != null)
        {
            checkTrigger(enemyColl);
        }
    }

    private void petGroundCheck()
    {
        isGround = false;

        if (gravityVelocity > 0) //gravityVelocity값이 0보다 클 경우 아래 코드가 실행을 멈춤
        {
            return;
        }

        hit = Physics2D.BoxCast(petBoxColl2D.bounds.center, petBoxColl2D.bounds.size,
            0, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));

        if (hit.transform != null && hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGround = true;
        }
    }

    private void petPos()
    {
        if (getPet == false)
        {
            return;
        }

        if (playerIn == false)
        {
            Vector3 telposPos = playerTrs.position;
            telposPos.x = playerTrs.position.x - 1f;
            transform.position = telposPos;
            gravityVelocity = 0;
        }
    }

    private void petMove()
    {
        if (getPet == false)
        {
            return;
        }

        if (moveOff == true)
        {
            moveVec.x = 0;
            return;
        }

        if (playerSc != null)
        {
            if (followPosX > 1)
            {
                isIdle = false;
                motionTimer = 0;
                moveVec.x = 1;
            }
            else if (followPosX < -1)
            {
                isIdle = false;
                motionTimer = 0;
                moveVec.x = -1;
            }
            else if (followPosX <= 1 || followPosX >= -1)
            {
                isIdle = true;
                moveVec.x = 0;
                moveOnTimer = 0;
            }

            if (followPosX > 3)
            {
                speed = 8;
                petRun = true;
                runStop = false;
                runStopTimer = 0;
            }
            else if (followPosX < -3)
            {
                speed = 8;
                petRun = true;
                runStop = false;
                runStopTimer = 0;
            }
            else if (followPosX <= 3 || followPosX >= -3)
            {
                speed = 2;
                runStop = true;
            }

            moveVec.x *= speed;
            rigid.velocity = moveVec;
        }
    }

    private void petGravity()
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

    private void petPassiveEffect()
    {
        if (playerSc != null)
        {
            if (petPassiveOn == false)
            {
                if (petType.ToString() == "petTypeA")
                {
                    playerSc.PlayerStatusHp(petHpEffect);                 
                    petPassiveOn = true;
                }
                else if (petType.ToString() == "petTypeB")
                {
                    playerSc.PlayerStatusArmor(petArmorEffect);
                    petPassiveOn = true;
                }
                else if (petType.ToString() == "petTypeC")
                {
                    playerSc.PlayerStatusDamage(petDamageEffect);
                    petPassiveOn = true;
                }
                else if (petType.ToString() == "petTypeD")
                {
                    playerSc.PlayerStatusCritical(petCriPcentEffect, petCriDamageEffect);
                    petPassiveOn = true;
                }
            }            
        }
    }

    private void petAni()
    {
        if (followPosY > 1)
        {
            isIdle = false;
            motionTimer = 0;
        }
        else if (followPosY < -1)
        {
            isIdle = false;
            motionTimer = 0;
        }

        anim.SetInteger("isWalk", (int)moveVec.x);
        anim.SetBool("isSleep", motionOn);
        anim.SetBool("isRun", petRun);
    }

    /// <summary>
    /// 펫을 얻었는지 체크하기 위한 함수
    /// </summary>
    /// <param name="_get"></param>
    public void GetPetCheck(bool _get)
    {
        getPet = _get;
    }

    /// <summary>
    /// 펫의 정보를 정하기 위해 타입을 플레이어에게 전달할 함수
    /// </summary> 
    /// <returns></returns>
    public PetType GetPetType()
    {
        return petType;
    }

    public void SetPetPassiveOn(bool _on)
    {
        petPassiveOn = _on;
    }
}
