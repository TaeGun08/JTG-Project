using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rigid; //플레이어의 리지드바디
    private RaycastHit2D hit2D;
    private BoxCollider2D playerBoxColl2D; //플레이어의 박스 콜라이더
    private Camera mainCam; //메인 카메라
    private Animator anim;

    //플레이어에 가져올 매니저
    private GameManager gameManager; //게임매니저
    private KeyManager keyManager; //키매니저

    [Header("이동")]
    [SerializeField, Tooltip("플레이어의 이동속도")] private float speed = 1.0f; //플레이어의 이동속도
    private Vector2 moveVec; //플레이어의 움직임을 위한 벡터
    private bool leftKey; //왼쪽 키를 눌렀을 때
    private bool rightKey; //오른쪽 키를 눌렀을 때

    private bool isGround = false; //플레이어가 땅에 붙어있지 않으면 false
    private float gravityVelocity;  //중력과 관련된 값을 받아오기 위한 변수

    [Header("점프")]
    [SerializeField, Tooltip("점프를 하기 위한 힘")] private float jumpPower = 1.0f; //점프를 하기 위한 힘
    private bool isJump = false; //점프를 했는지
    [SerializeField, Tooltip("더블 점프를 하기 위한 시간")] private float doubleJumpTime = 1.0f; //더블 점프를 하기 위한 기준시간
    private float doubleJumpTimer = 1.0f; //더블 점프를 하기 위한 지연시간
    private bool noAirJump = false; //점프 키를 누르지 않고 공중에 있을 경우를 체크
    private bool jumpKey; //점프 키를 눌렀을 때
    private bool animIsJump = false; //점프 키를 눌러 점프를 했는지 안 했는지 체크, 했으면 애니메이션 실행
    [SerializeField, Tooltip("점프 애니메이션이 지속되는 시간")] private float animJumpTime = 1.0f; //점프를 했을 때 애니메이션이 작동을 멈추는 기준시간
    private float animTimer = 0.0f; //점프를 했을 때 애니메이션을 작동하기 지속시간

    private Transform playerHand; //플레이어의 손 위치
    private bool mouseAimRight = false;

    [Header("대쉬")]
    [SerializeField, Tooltip("대쉬 힘")] private float dashPower = 1.0f;
    [SerializeField, Tooltip("대쉬 길이")] private float dashRange = 1.0f;
    [SerializeField, Tooltip("대쉬 재사용대기 시간")] private float dashCoolTime = 1.0f;
    private float dashCoolTimer = 0.0f;
    private float dashRangeTimer = 0.0f;
    private bool dashCoolOn = false;
    private bool isDash = false;
    private bool dashKey;
    private TrailRenderer dashEffect;

    [Header("무기 관련 설정")]
    [SerializeField, Tooltip("무기 변경 딜레이")] private float weaponsChangeCoolTime = 1.0f; //무기 변경을 대기 시간
    private float weaponsChangeCoolTimer = 0.0f; //무기 변경을 위한 타이머
    private bool weaponsChangeCoolOn = false; //무기가 변경이 가능하면 false
    private List<GameObject> weaponPrefabs = new List<GameObject>(); //무기를 담을 인벤토리 역할
    private GameObject getWeapon; //콜라이더에 닿은 오브젝트를 담아 올 변수
    private bool weaponSwap = false; //무기 변경을 담당하는 변수
    private int weaponCount = 0; //중복으로 만들어지지않게 제어해주는 변수

    private void OnDrawGizmos() //박스캐스트를 씬뷰에서 눈으로 확인이 가능하게 보여줌
    {
        if (playerBoxColl2D != null) //콜라이더가 null이 아니라면 박스레이 범위를 씬뷰에서 확인할 수 있게
        {
            Gizmos.color = Color.red;
            Vector3 pos = playerBoxColl2D.bounds.center - new Vector3(0, 0.1f, 0);
            Gizmos.DrawWireCube(pos, playerBoxColl2D.bounds.size);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item" || collision.gameObject.tag == "Weapon") //플레이어에 닿은 오브젝트 태그가 아이템 또는 무기인지 체크
        {
            ItemPickUp itemPickUpSc = collision.gameObject.GetComponent<ItemPickUp>();  //플레이어에 닿은 오브젝트의 아이템 스크립트를 가져옴
            ItemPickUp.ItemType itemPickUpType = itemPickUpSc.GetItemType(); //
            if (itemPickUpType == ItemPickUp.ItemType.Weapon) //가져온 아이템 타입과 스크립트의 아이템 타입이 일치하면 작동
            {           
                if (Input.GetKey(keyManager.PickUpItemKey())) //E키를 누르면 작동
                {
                    if (weaponPrefabs.Count > 1) //무기 카운트가 1보다 크면 자신을 제외한 나머지 오브젝트를 비활성화 시켜 줌
                    {
                        int count = weaponPrefabs.Count;
                        for (int i = 0; i < count - 1; i++)
                        {
                            weaponPrefabs[i].SetActive(false);
                        }
                    }

                    Weapons weaponsSc = collision.gameObject.GetComponent<Weapons>();
                    weaponsSc.ShootingOn(true);
                    if (weaponCount < 2) //weaponCount를 이용해서 무기가 중복으로 생성되지 않게 막아줌
                    {
                        getWeapon = Instantiate(collision.gameObject, playerHand.position, playerHand.rotation, playerHand);
                        weaponCount++;
                    }
                    Destroy(collision.gameObject); //무기가 복제가 된 후 화면에 남아있는 무기를 지움
                    weaponPrefabs.Add(getWeapon); //무기를 인벤토리 역할을 하는 배열에 추가함
                }
            }
        }
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>(); //플레이어 자신의 리지드바디를 가져옴
        playerBoxColl2D = GetComponent<BoxCollider2D>(); //플레이어 자신의 박스콜라이더2D를 가져옴
        anim = GetComponent<Animator>(); //플레이어의 애니메이션을 가져옴
        playerHand = transform.Find("PlayerHand");
        dashEffect = GetComponent<TrailRenderer>();

        dashEffect.enabled = false;
    }

    private void Start()
    {
        gameManager = GameManager.Instance; //게임 매니저를 가져와 gameManager에 담아 줌
        keyManager = KeyManager.instance; //키매니저를 가져와 keyManager 담아 줌

        mainCam = Camera.main; //메인 카메라를 가져와 mainCam에 담아 줌
    }

    private void Update()
    {
        if (gameManager.GamePause() == true) //게임매니저에서 gamePause가 true라면 플레이어 동작을 멈춤
        {
            return;
        }

        playerCheckGround();
        playerAim();
        playerMove();
        playerGravity();
        playerDash();
        playerWeaponChange();
        playerAni();
    }

    /// <summary>
    /// 플레이어가 땅인지 아닌지 확인을 담당하는 함수
    /// </summary>
    private void playerCheckGround()
    {
        isGround = false; //다른 조건식이 없는 경우 항상 isGround를 false로 만듦

        if (gravityVelocity > 0) //gravityVelocity값이 0보다 클 경우 아래 코드가 실행을 멈춤
        {
            return;
        }

        hit2D = Physics2D.BoxCast(playerBoxColl2D.bounds.center, playerBoxColl2D.bounds.size,
            0.0f, Vector2.down, 0.1f, LayerMask.GetMask("Ground")); //플레이어의 박스 콜라이더의 크기를 가져와서 박스캐스트를 만듦

        if (hit2D.transform != null && 
            hit2D.transform.gameObject.layer == LayerMask.NameToLayer("Ground")) //캐스트에 닿은 트랜스폼이 null이 아니고, 레이어가 Ground면 작동
        {
            isGround = true;
            isJump = false;
            noAirJump = true;
            doubleJumpTimer = 0.0f;
        }
    }

    /// <summary>
    /// 플레이어의 마우스 에임
    /// </summary>
    private void playerAim()
    {
        Vector3 mouseInputPos = Input.mousePosition; //입력된 마우스포지션을 받아옴
        mouseInputPos.z = -mainCam.transform.position.z; //마우스포지션 값에 메인카메라의 포지션z값을 받아옴
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(mouseInputPos);

        Vector3 mouseDistance = mouseWorldPos - transform.position;

        Vector3 scale = transform.localScale; //플레이어의 스프라이트 좌우변경을 위해 스케일을 받아옴
        scale.x *= -1;

        if (mouseDistance.x > 0 && transform.localScale.x != 1) //마우스 에임이 오른쪽이면 캐릭터도 오른쪽을 바라봄
        {
            transform.localScale = scale;
            mouseAimRight = true;
        }
        else if (mouseDistance.x < 0 && transform.localScale.x != -1) //마우스 에임이 왼쪽이면 캐릭터도 왼쪽으로 바라봄
        {
            transform.localScale = scale;
            mouseAimRight = false;
        }

        Vector3 mouseAim = Vector3.right; //항상 오른쪽 기준으로 설정 함
        if (mouseAimRight == false) //왼쪽으로 바라보면 에임도 왼쪽으로 변경
        {
            mouseAim = Vector3.left;
        }

        float angle = Quaternion.FromToRotation(mouseAim, mouseDistance).eulerAngles.z;

        playerHand.rotation = Quaternion.Euler(playerHand.rotation.x, playerHand.rotation.y, angle);
    }

    /// <summary>
    /// 플레이어를 좌우로 움직이는걸 담당하는 함수
    /// </summary>
    private void playerMove()
    {
        if (isDash == true)
        {
            return;
        }

        leftKey = Input.GetKey(keyManager.PlayerLeftKey()); //키 매니저에서 왼쪽 키를 받아와서 사용 
        rightKey = Input.GetKey(keyManager.PlayerRightKey()); //키 매니저에서 오른쪽 키를 받아와서 사용        

        if (leftKey == true) //왼쪽 키를 눌렀을 경우 왼쪽으로
        {
            moveVec.x = -1 * speed;
        }
        else if (rightKey == true) //오른쪽 키를 눌렀을 경우 오른쪽으로
        {
            moveVec.x = 1 * speed;
        }
        else
        {
            moveVec.x = 0;
        }

        moveVec.y = gravityVelocity; //moveVec에 중력을 넣음
        rigid.velocity = moveVec;
    }

    /// <summary>
    /// 플레이어의 점프를 담당하는 함수
    /// </summary>
    private void playerJump()
    {
        if (isDash == true)
        {
            return;
        }

        jumpKey = Input.GetKeyDown(keyManager.PlayerJumpKey());

        if (noAirJump == true && isGround == false)
        {
            isJump = true;
            noAirJump = false;
        }

        if (isGround == false && isJump == true) //isGround가 false고, isJump가 true 더블 점프를 하기 위한 시간이 작동
        {
            doubleJumpTimer += Time.deltaTime;
        }

        if (jumpKey == true && isGround == true && isJump == false)
        {
            gravityVelocity = jumpPower;
            animIsJump = true;
            isJump = true;
        }
        else if (jumpKey == true && isGround == false && isJump == true && doubleJumpTimer >= doubleJumpTime) //점프 사용 후 시간이 지나면 한번 더 가능
        {
            gravityVelocity = jumpPower;
            animIsJump = true;
            isJump = false;
            doubleJumpTimer = 0.0f;
        }
    }

    /// <summary>
    /// 플레이어의 중력을 담당하는 함수
    /// </summary>
    private void playerGravity()
    {
        if (isDash == true)
        {
            return;
        }

        if (isGround == false)
        {
            gravityVelocity -= gameManager.gravityScale() * Time.deltaTime; //지속적으로 받는 중력         
        }
        else
        {
            gravityVelocity = -1; //isGround가 true일 경우 중력을 -1로 만들어 땅에 붙게 만듦
        }

        playerJump();
    }

    /// <summary>
    /// 플레이어의 대쉬를 담당하는 함수
    /// </summary>
    private void playerDash()
    {
        dashKey = Input.GetKeyDown(keyManager.PlayerDashKey());

        if (isDash == true) //대쉬의 지속시간
        {
            dashRangeTimer += Time.deltaTime;
            if (dashRangeTimer >= dashRange)
            {
                isDash = false;
                dashRangeTimer = 0.0f;
                dashEffect.Clear();
                dashEffect.enabled = false;
            }
        }

        if (dashCoolOn == true) //대쉬를 하기 위한 쿨타임
        {
            dashCoolTimer += Time.deltaTime;
            if (dashCoolTimer >= dashCoolTime)
            {
                dashCoolOn = false;
                dashCoolTimer = 0.0f;
            }
        }

        if (dashKey == true && isDash == false && dashCoolOn == false)
        {
            isDash = true;

            dashCoolOn = true;

            gravityVelocity = 0.0f;

            if (moveVec.x > 0) //moveVec.x 가 0보다 크면 오른쪽으로 대쉬
            {
                moveVec.x = dashPower;
            }
            else if (moveVec.x < 0) //moveVec.x 가 0보다 작으면 왼쪽으로 대쉬
            {
                moveVec.x = -dashPower;
            }
            else if (moveVec.x == 0) //moveVec.x 가 0이면 마우스 에임이 있는 쪽으로 대쉬
            {
                moveVec.x = dashPower;
                if (mouseAimRight == false)
                {
                    moveVec.x = -dashPower;
                }
            }

            rigid.velocity = moveVec;

            dashEffect.enabled = true;
        }
    }

    /// <summary>
    /// 플레이어가 가지고 있는 무기중 하나로 변경을 담당하는 함수
    /// </summary>
    private void playerWeaponChange()
    {
        if (weaponPrefabs == null || weaponPrefabs.Count < 2) //무기 배열에 아무것도 없거나, 2보다 적으면 실행을 멈춤
        {
            return;
        }

        if (weaponPrefabs.Count > 2) //무기 배열의 길이가 2를 넘어가면 그 후에 생기는 배열은 삭제
        {
            weaponPrefabs.RemoveAt(2);
        }

        if (weaponsChangeCoolOn == true) //무기 변경 타이머가 true면 작동
        {
            weaponsChangeCoolTimer += Time.deltaTime;
            if (weaponsChangeCoolTimer >= weaponsChangeCoolTime)
            {
                weaponsChangeCoolOn = false;
                weaponsChangeCoolTimer = 0.0f;
            }
        }

        if (Input.GetKeyDown(keyManager.WeaponChangeKey()) && weaponSwap == false && weaponsChangeCoolOn == false) 
        {
            weaponPrefabs[0].SetActive(true);
            weaponPrefabs[1].SetActive(false);
            weaponSwap = true;
            weaponsChangeCoolOn = true;
        }
        else if (Input.GetKeyDown(keyManager.WeaponChangeKey()) && weaponSwap == true && weaponsChangeCoolOn == false)
        {
            weaponPrefabs[0].SetActive(false);
            weaponPrefabs[1].SetActive(true);
            weaponSwap = false;
            weaponsChangeCoolOn = false;
        }
    }

    /// <summary>
    /// 플레이어의 애니메이션을 담당하는 함수
    /// </summary>
    private void playerAni()
    {
        anim.SetInteger("isWalk", (int)moveVec.x);
        anim.SetBool("isJump", animIsJump);
        anim.SetBool("isGround", isGround);

        if (animIsJump == true)
        {
            animTimer += Time.deltaTime;
        }

        if (animTimer >= animJumpTime)
        {
            animIsJump = false;
            animTimer = 0.0f;
        }
    }
}
