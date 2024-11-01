using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SaveObject;

public class Player : MonoBehaviour
{
    public class PlayerData
    {
        public float playerDamage;
        public int playerArmor;
        public int playerHp;
        public int playerCurHp;
        public float playerCritical;
        public float playerCriDamage;
        public int playerLevel;
        public float playerMaxExp;
        public float playerExp;
        public int playerLevelPoint;
        public int playerWeaponA;
        public int playerWeaponB;
        public int playerPet;
    }

    private PlayerData playerData = new PlayerData();
    private SaveObject saveObject;
    private bool dataSave = false;

    public enum PlayerSkillType
    {
        skillTypeA,
        skillTypeB,
        skillTypeC,
        skillTypeD,
    }

    [SerializeField] private PlayerSkillType skillType;

    private Rigidbody2D rigid; //플레이어의 리지드바디
    private RaycastHit2D hit2D;
    private BoxCollider2D playerBoxColl2D; //플레이어의 박스 콜라이더
    private Camera mainCam; //메인 카메라
    private Animator anim;
    private SpriteRenderer playerRen;

    private PlayerUI playerUI;

    //플레이어에 가져올 매니저
    private GameManager gameManager; //게임매니저
    private KeyManager keyManager; //키매니저

    [SerializeField] private float gravity; //게임매니저에서 가져올 중력값을 저장할 변수

    [Header("캐릭터의 기본 설정")]
    [SerializeField] private float playerDamage; //플레이어의 공격력, 전체적인 범위에서 영구적인 영향을 미침
    [SerializeField] private float playerBuffDamageUp; //플레이어의 공격력, 전체적인 범위에서 잠깐의 영향을 미침
    [SerializeField, Range(0.0f, 100.0f)] private float playerCritical; //플레이어의 치명타 확률
    [SerializeField, Range(2.0f, 3.5f)] private float playerCriDamage; //플레이어의 치명타 데미지
    private bool buffDamageUp = false; //버프 데미지가 적용됐을 때 사용되는 변수
    private float buffDuration = 0.0f; //지속 시간 타이머
    [SerializeField] private int playerMaxHp = 100; //플레이어의 최대체력
    [SerializeField] private int playerCurHp = 0; //플레이어의 현재체력
    [SerializeField] private int playerArmor = 0; //플레이어의 방어력
    private bool playerHitDamage = false; //플레이어가 맞았을 시 스프라이트렌더러 색깔을 변경하기 위한 변수
    private float hitDamageTimer; //변경되고 다시 원래 색깔로 돌아오기 위한 시간
    [SerializeField] private int playerLevel;
    [SerializeField] private int levelPoint;
    private float playerExp;
    [SerializeField, Tooltip("플레이어가 이 경험치만큼 도달 시 레벨업")] private float playerMaxExp;

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
    private bool dashInvincibleOn = false; //대쉬를 사용할 시 무적을 위한 변수

    [Header("벽 타기 및 벽 슬라이딩")]
    [SerializeField, Tooltip("벽 점프를 위한 힘")] private float wallJumpPower = 0.5f; //벽 점프를 위한 힘
    [SerializeField, Tooltip("벽 점프 후 공중에 있는 시간")] private float wallJumpSky = 0.3f;
    [SerializeField, Tooltip("벽 슬라이딩 속도")] private float wallSlidingSpeed = 5.0f;
    private bool isWall = false; //벽에 닿았는지 확인해 줌
    private bool wallJumpTimerOn = false;
    private float wallJumpTimer = 0.0f; //벽 점프
    private bool useWallSliding = false; //벽 슬라이딩을 하기 위한 조건식


    [Header("무기 관련 설정")]
    [SerializeField, Tooltip("무기 변경 딜레이")] private float weaponsChangeCoolTime = 1.0f; //무기 변경을 대기 시간
    private float weaponsChangeCoolTimer = 0.0f; //무기 변경을 위한 타이머
    private bool weaponsChangeCoolOn = false; //무기가 변경이 가능하면 false
    [SerializeField, Tooltip("무기를 담아둘 공간")] private List<GameObject> weaponPrefabs = new List<GameObject>(); //무기를 담을 인벤토리 역할
    private GameObject getWeapon; //콜라이더에 닿은 오브젝트를 담아 올 변수
    private bool weaponSwap = false; //무기 변경을 담당하는 변수
    private bool weaponDrop = false;
    private float weaponDropTime = 0.0f;
    private Vector3 weaponLocalScale;
    private int playerWeaponA;
    private int playerWeaponB;
    private bool playerWeaponCheck= false;

    [Header("펫 관련 설정")]
    [SerializeField, Tooltip("펫을 담아둘 공간")] private List<GameObject> petPrefabs = new List<GameObject>();
    private Pet petSc; //펫의 스크립트 가져올 변수
    [SerializeField] private int playerPet;

    private bool optionOn = false; //플레이어가 옵션을 켰는지 안 켰는지 확인하기 위한 변수
    private bool statusOpen = false; //플레이어가 정보창을 켰는지 안 켰는지 확인하기 위한 변수

    private bool knockBack = false; //플레이어 넉백을 위한 변수
    private float knockBackTimer; //넉백이 지속되는 타이머

    [SerializeField] private FadeOut fadeSc;
    private bool playerDaed = false;
    private float deadSceneTimer;

    private void OnDrawGizmos() //박스캐스트를 씬뷰에서 눈으로 확인이 가능하게 보여줌
    {
        if (playerBoxColl2D != null) //콜라이더가 null이 아니라면 박스레이 범위를 씬뷰에서 확인할 수 있게
        {
            Gizmos.color = Color.red;
            Vector3 pos = playerBoxColl2D.bounds.center - new Vector3(0, 0.1f, 0);
            Gizmos.DrawWireCube(pos, playerBoxColl2D.bounds.size);
        }
    }

    private void pickUpItem(Collider2D _collision) //아이템을 줍기 위한 함수
    {
        if (_collision.gameObject.tag == "Item" || _collision.gameObject.tag == "Weapon") //플레이어에 닿은 오브젝트 태그가 아이템 또는 무기인지 체크
        {
            ItemPickUp itemPickUpSc = _collision.gameObject.GetComponent<ItemPickUp>();  //플레이어에 닿은 오브젝트의 아이템 스크립트를 가져옴
            ItemPickUp.ItemType itemPickUpType = itemPickUpSc.GetItemType();

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (itemPickUpType == ItemPickUp.ItemType.Weapon) //가져온 아이템 타입과 스크립트의 아이템 타입이 일치하면 작동
                {
                    if (weaponPrefabs.Count > 1)
                    {
                        return;
                    }

                    int count = weaponPrefabs.Count;

                    if (weaponPrefabs.Count > 0) //무기 카운트가 0보다 크면 자신을 제외한 나머지 오브젝트를 비활성화 시켜 줌
                    {
                        for (int i = 0; i < count; i++)
                        {
                            SpriteRenderer weaponRen = weaponPrefabs[i].GetComponent<SpriteRenderer>();
                            weaponRen.enabled = false;
                            Weapons weaponSc = weaponPrefabs[i].GetComponent<Weapons>();
                            weaponSc.ShootingOn(false);
                        }
                    }

                    getWeapon = _collision.gameObject;
                    getWeapon.transform.SetParent(playerHand); //자식 오브젝트로 넣는 코드
                    getWeapon.transform.position = playerHand.transform.position;
                    getWeapon.transform.rotation = playerHand.transform.rotation;
                    weaponLocalScale = getWeapon.transform.localScale;

                    if (getWeapon.transform.localScale.x < 0)
                    {
                        weaponLocalScale.x *= -1;
                        getWeapon.transform.localScale = weaponLocalScale;
                    }

                    getWeapon.GetComponent<BoxCollider2D>().enabled = false;

                    Weapons getWeaponSc = getWeapon.GetComponent<Weapons>();
                    getWeaponSc.ShootingOn(true);
                    getWeaponSc.PickUpImageOff(true);
                    getWeaponSc.WeaponGravityOff(true);
                    getWeaponSc.WeaponUseShooting(false);
                    SpriteRenderer getWeaponRen = getWeapon.GetComponent<SpriteRenderer>();
                    getWeaponRen.sortingOrder = 2;

                    weaponPrefabs.Add(getWeapon); //무기를 인벤토리 역할을 하는 배열에 추가함

                    playerWeaponCheck = true;

                    if (playerWeaponCheck == true && weaponPrefabs != null)
                    {
                        if (weaponPrefabs.Count == 1)
                        {
                            playerWeaponA = (int)getWeaponSc.GetWeaponType();
                        }
                        else if (weaponPrefabs.Count == 2)
                        {
                            playerWeaponB = (int)getWeaponSc.GetWeaponType();
                        }
                    }
                }
                else if (itemPickUpType == ItemPickUp.ItemType.Pet)
                {
                    if (petPrefabs.Count > 0)
                    {
                        return;
                    }

                    petSc = _collision.gameObject.GetComponent<Pet>();
                    petSc.GetPetCheck(true);
                    petPrefabs.Add(_collision.gameObject);

                    playerPet = (int)petSc.GetPetType();
                }
            }
            else if (itemPickUpType == ItemPickUp.ItemType.Buff)
            {
                GameObject buffObj = _collision.gameObject;
                Buff buffSc = buffObj.GetComponent<Buff>();
                Buff.BuffType buffType = buffSc.GetBuffType();
                if (buffType == Buff.BuffType.damage)
                {
                    playerBuffDamageUp = buffSc.BuffTypeValue();
                    buffDuration = 10;
                    buffDamageUp = true;
                }
                else if (buffType == Buff.BuffType.armor)
                {
                    playerArmor = (int)buffSc.BuffTypeValue();
                }
                else if (buffType == Buff.BuffType.heal)
                {
                    playerCurHp += (int)buffSc.BuffTypeValue();
                }
                DestroyImmediate(_collision.gameObject);
            }
        }
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>(); //플레이어 자신의 리지드바디를 가져옴
        playerBoxColl2D = GetComponent<BoxCollider2D>(); //플레이어 자신의 박스콜라이더2D를 가져옴
        anim = GetComponent<Animator>(); //플레이어의 애니메이션을 가져옴
        playerHand = transform.Find("PlayerHand");
        playerRen = GetComponent<SpriteRenderer>();
        playerUI = GetComponent<PlayerUI>();
        saveObject = GetComponent<SaveObject>();

        playerCurHp = playerMaxHp;

        dashCoolTimer = dashCoolTime;

        weaponDropTime = 0.5f;

        hitDamageTimer = 0.1f;

        mouseAimRight = true;
    }

    private void Start()
    {
        gameManager = GameManager.Instance; //게임 매니저를 가져와 gameManager에 담아 줌
        keyManager = KeyManager.instance; //키매니저를 가져와 keyManager 담아 줌

        mainCam = Camera.main; //메인 카메라를 가져와 mainCam에 담아 줌

        gravity = gameManager.GravityScale();

        playerUI.OptionOn(false);

        playerUI.SetPlayerHp(playerCurHp, playerMaxHp, "");

        playerUI.StatusOpen(false);

        saveObject.PlayerObjectDataLoad();
    }

    private void Update()
    {
        if (playerDaed == true)
        {
            fadeSc.FadeInOut(true);
            deadSceneTimer += Time.deltaTime;
            if (deadSceneTimer > 1.0f)
            {
                SceneManager.LoadSceneAsync("DeadScene");
            }
            return;
        }

        setPlayerData();
        playerOption();
        playerStatusOpen();

        if (gameManager.GetGamePause() == true)
        {
            return;
        }

        if (playerCurHp > playerMaxHp)
        {
            playerCurHp = playerMaxHp;
        }

        timers();
        itmeColliderCheck();
        playerCheckGround();
        playerAim();
        playerMove();
        playerGravity();
        playerDash();
        playerWallSkill();
        playerWeaponChange();
        playerWeaponDrop();
        playerBuffDamage();
        playerDamageHit();
        playerDead();
        playerWeaponCritical();
        petPassiveBuff();
        playerLevelUpCheck();
        playerAni();
    }

    /// <summary>
    /// 플레이어의 데이터를 세이브하기 위한 함수
    /// </summary>
    private void setPlayerData()
    {
        if (dataSave == true)
        {
            playerData.playerDamage = playerDamage;
            playerData.playerArmor = playerArmor;
            playerData.playerHp = playerMaxHp;
            playerData.playerCurHp = playerCurHp;
            playerData.playerCritical = playerCritical;
            playerData.playerCriDamage = playerCriDamage;
            playerData.playerLevel = playerLevel;
            playerData.playerMaxExp = playerMaxExp;
            playerData.playerExp = playerExp;
            playerData.playerLevelPoint = levelPoint;
            playerData.playerWeaponA = playerWeaponA;
            playerData.playerWeaponB = playerWeaponB;
            playerData.playerPet = playerPet;

            saveObject.PlayerObjectSaveData(playerData);

            dataSave = false;
        }
    }

    /// <summary>
    /// 플레이어가 옵션창을 켜고 끌 수 있게 제어하도록 도와주는 함수
    /// </summary>
    private void playerOption()
    {
        if (Input.GetKeyDown(keyManager.OptionKey()) && optionOn == false && statusOpen == false)
        {
            optionOn = true;
            playerUI.OptionOn(optionOn);
        }
        else if (Input.GetKeyDown(keyManager.OptionKey()) && optionOn == true)
        {
            optionOn = false;
            playerUI.OptionOn(optionOn);
        }

        if (optionOn == false && statusOpen == false)
        {
            gameManager.SetGamePause(false);
        }
        else if (optionOn == true || statusOpen == true)
        {
            gameManager.SetGamePause(true);
        }
    }

    /// <summary>
    /// 플레이어가 옵션창을 켜고 끌 수 있게 제어하도록 도와주는 함수
    /// </summary>
    private void playerStatusOpen()
    {
        if (Input.GetKeyDown(keyManager.StatuswindowKey()) && statusOpen == false && optionOn == false)
        {
            statusOpen = true;
            playerUI.StatusOpen(statusOpen);
        }
        else if ((Input.GetKeyDown(keyManager.StatuswindowKey()) || 
            Input.GetKeyDown(keyManager.OptionKey())) && statusOpen == true)
        {
            statusOpen = false;
            playerUI.StatusOpen(statusOpen);
        }
    }

    /// <summary>
    /// 함수에서 사용할 타이머 모음
    /// </summary>
    private void timers()
    {
        if (isGround == false && isJump == true) //isGround가 false고, isJump가 true 더블 점프를 하기 위한 시간이 작동
        {
            doubleJumpTimer += Time.deltaTime;
        }

        if (isDash == true) //대쉬의 지속시간
        {
            dashRangeTimer += Time.deltaTime;
        }

        if (dashCoolOn == true) //대쉬를 하기 위한 쿨타임
        {
            dashCoolTimer -= Time.deltaTime;
            if (dashCoolTimer >= 1)
            {
                string timerTextInt = $"{(int)dashCoolTimer}";
                playerUI.SetPlayerDashCool(dashCoolTimer / dashCoolTime, timerTextInt);
            }
            else if (dashCoolTimer < 1)
            {
                string timerTextInt = $"{dashCoolTimer.ToString("F1")}";
                playerUI.SetPlayerDashCool(dashCoolTimer / dashCoolTime, timerTextInt);
            }
        }

        if (wallJumpTimerOn == true) //벽 점프가 실행이되면 다시 중력을 받기 위한 타이머
        {
            wallJumpTimer += Time.deltaTime;
        }

        if (weaponsChangeCoolOn == true) //무기 변경 타이머가 true면 작동
        {
            weaponsChangeCoolTimer += Time.deltaTime;
        }

        if (animIsJump == true) //점프 애니메이션이 동작되는 시간
        {
            animTimer += Time.deltaTime;
        }

        if (weaponDrop == true) //무기를 버렸을 때 다시 버릴 수 있는 대기 시간
        {
            weaponDropTime -= Time.deltaTime;
        }

        if (buffDamageUp == true)
        {
            buffDuration -= Time.deltaTime;
            if (buffDuration < 0)
            {
                buffDamageUp = false;
                playerBuffDamageUp = 0;
            }
        }

        if (playerHitDamage == true)
        {
            hitDamageTimer -= Time.deltaTime;
            if (hitDamageTimer < 0)
            {
                hitDamageTimer = 0.1f;
                playerHitDamage = false;
                playerRen.color = Color.white;
            }
        }

        if (knockBack == true)
        {
            knockBackTimer += Time.deltaTime;
            if (knockBackTimer > 0.3f)
            {
                knockBackTimer = 0;
                knockBack = false;
            }
        }
    }

    /// <summary>
    /// 아이템 콜라이더 체크를 담당하는 함수
    /// </summary>
    private void itmeColliderCheck()
    {
        Collider2D weaponColl = Physics2D.OverlapBox(playerBoxColl2D.bounds.center,
                playerBoxColl2D.bounds.size, 0f, LayerMask.GetMask("Weapon")); //플레이어 콜라이더에 닿은 레이어를 확인해  weaponColl에 넣는다

        Collider2D itemColl = Physics2D.OverlapBox(playerBoxColl2D.bounds.center,
                playerBoxColl2D.bounds.size, 0f, LayerMask.GetMask("Item")); //플레이어 콜라이더에 닿은 레이어를 확인해 itemColl에 넣는다

        Collider2D petColl = Physics2D.OverlapBox(playerBoxColl2D.bounds.center,
                playerBoxColl2D.bounds.size, 0f, LayerMask.GetMask("Pet")); //플레이어 콜라이더에 닿은 레이어를 확인해 petColl에 넣는다

        if (weaponColl != null)
        {
            pickUpItem(weaponColl);
        }

        if (itemColl != null)
        {
            pickUpItem(itemColl);
        }

        if (petColl != null)
        {
            pickUpItem(petColl);
        }

        //Collider2D[] colls = Physics2D.OverlapBoxAll(playerBoxColl2D.bounds.center, 
        //    playerBoxColl2D.bounds.size, 0f, LayerMask.GetMask("Weapon"));
        //int count = colls.Length;
        //for (int iNum = 0; iNum < count; ++iNum)
        //{
        //    Collider2D coll = colls[iNum];
        //    pickUpItem(coll);
        //    colls[iNum] = null;
        //}     
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
        if (isDash == true || wallJumpTimerOn == true || knockBack == true)
        {
            return;
        }

        leftKey = Input.GetKey(keyManager.PlayerLeftKey()); //키 매니저에서 왼쪽 키를 받아와서 사용 
        rightKey = Input.GetKey(keyManager.PlayerRightKey()); //키 매니저에서 오른쪽 키를 받아와서 사용        

        if (leftKey == true) //왼쪽 키를 눌렀을 경우 왼쪽으로
        {
            moveVec.x = -1;
        }
        else if (rightKey == true) //오른쪽 키를 눌렀을 경우 오른쪽으로
        {
            moveVec.x = 1;
        }
        else
        {
            moveVec.x = 0;
        }

        moveVec.x *= speed;
        rigid.velocity = moveVec;
    }

    /// <summary>
    /// 플레이어의 점프를 담당하는 함수
    /// </summary>
    private void playerJump()
    {
        if (isDash == true || knockBack == true)
        {
            return;
        }

        jumpKey = Input.GetKeyDown(keyManager.PlayerJumpKey());

        if (noAirJump == true && isGround == false)
        {
            isJump = true;
            noAirJump = false;
        }

        if (jumpKey == true && isGround == true && isJump == false)
        {
            gravityVelocity = jumpPower;
            animIsJump = true;
            isJump = true;
            useWallSliding = true;
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
        if (isDash == true || knockBack == true)
        {
            return;
        }

        if (isGround == false)
        {
            gravityVelocity -= gravity * Time.deltaTime; //지속적으로 받는 중력 
            if (gravityVelocity > gravity)
            {
                gravityVelocity = gravity;
            }
        }
        else
        {
            gravityVelocity = -1; //isGround가 true일 경우 중력을 -1로 만들어 땅에 붙게 만듦
        }

        moveVec.y = gravityVelocity; //moveVec에 중력을 넣음
        rigid.velocity = moveVec;


        playerJump();
    }

    /// <summary>
    /// 플레이어의 대쉬를 담당하는 함수
    /// </summary>
    private void playerDash()
    {
        if (knockBack == true)
        {
            return;
        }

        dashKey = Input.GetKeyDown(keyManager.PlayerDashKey());

        if (dashRangeTimer >= dashRange)
        {
            isDash = false;
            dashInvincibleOn = false;
            dashRangeTimer = 0.0f;
            playerRen.color = Color.white;
        }

        if (dashCoolTimer < 0)
        {
            dashCoolOn = false;
            dashCoolTimer = dashCoolTime;
            playerUI.PlayerDashCool(false);
        }

        if (dashKey == true && isDash == false && dashCoolOn == false)
        {
            isDash = true;

            dashCoolOn = true;

            dashInvincibleOn = true;

            playerUI.PlayerDashCool(true);

            gravityVelocity = 0.0f;

            playerRen.color = Color.black;

            if (moveVec.x > 0 && isWall == false) //moveVec.x 가 0보다 크면 오른쪽으로 대쉬
            {
                moveVec.x = dashPower;
            }
            else if (moveVec.x < 0 && isWall == false) //moveVec.x 가 0보다 작으면 왼쪽으로 대쉬
            {
                moveVec.x = -dashPower;
            }
            else if (moveVec.x == 0 && isWall == false) //moveVec.x 가 0이면 마우스 에임이 있는 쪽으로 대쉬
            {
                moveVec.x = dashPower;
                if (mouseAimRight == false)
                {
                    moveVec.x = -dashPower;
                }
            }

            rigid.velocity = moveVec;
        }
    }

    /// <summary>
    /// 벽에 관련된 상호작용 및 기능을 하기 위한 함수
    /// </summary>
    private void playerWallSkill()
    {
        if (wallJumpTimer >= wallJumpSky)
        {
            wallJumpTimerOn = false;
            wallJumpTimer = 0.0f;
        }

        if (jumpKey == true && isWall == true && isGround == false && moveVec.x != 0) //벽 점프를 하기 위한 조건식
        {
            gravityVelocity = wallJumpPower;
            moveVec.x *= -0.8f;
            rigid.velocity = moveVec;
            wallJumpTimerOn = true;
            useWallSliding = true;
            animIsJump = true;
        }
        else if (jumpKey == false && isWall == true && isGround == false && moveVec.x != 0 && useWallSliding == false)  //벽 슬라이딩을 하기 위한 조건식
        {
            gravityVelocity = -wallSlidingSpeed;
        }
        else if ((moveVec.x == 0 && isWall == false) || isWall == false || isGround == true)
        {
            if (moveVec.x != 0 && isGround == true)
            {
                return;
            }

            useWallSliding = false;
        }
    }

    /// <summary>
    /// 플레이어가 가지고 있는 무기중 하나로 변경을 담당하는 함수
    /// </summary>
    private void playerWeaponChange()
    {
        if (weaponPrefabs.Count < 2) //무기 배열에 아무것도 없거나, 2보다 적으면 실행을 멈춤
        {
            return;
        }

        if (weaponPrefabs.Count > 2) //무기 배열의 길이가 2를 넘어가면 그 후에 생기는 배열은 삭제
        {
            weaponPrefabs.RemoveAt(2);
        }

        if (weaponsChangeCoolTimer >= weaponsChangeCoolTime)
        {
            weaponsChangeCoolOn = false;
            weaponsChangeCoolTimer = 0.0f;
        }

        if (Input.GetKeyDown(keyManager.WeaponChangeKey()))
        {
            int count = weaponPrefabs.Count;

            if (weaponSwap == false && weaponsChangeCoolOn == false)
            {
                for (int i = 0; i < count; i++)
                {
                    SpriteRenderer weaponRen = weaponPrefabs[i].GetComponent<SpriteRenderer>();
                    Weapons weaponSc = weaponPrefabs[i].GetComponent<Weapons>();

                    if (i == 0)
                    {
                        weaponRen.enabled = true;
                        weaponSc.ShootingOn(true);
                    }
                    else if (i == 1)
                    {
                        weaponRen.enabled = false;
                        weaponSc.ShootingOn(false);
                    }
                }

                weaponSwap = true;
                weaponsChangeCoolOn = true;
                gameManager.ReloadingObj().SetActive(false);
            }
            else if (weaponSwap == true && weaponsChangeCoolOn == false)
            {
                for (int i = 0; i < count; i++)
                {
                    SpriteRenderer weaponRen = weaponPrefabs[i].GetComponent<SpriteRenderer>();
                    Weapons weaponSc = weaponPrefabs[i].GetComponent<Weapons>();

                    if (i == 0)
                    {
                        weaponRen.enabled = false;
                        weaponSc.ShootingOn(false);
                    }
                    else if (i == 1)
                    {
                        weaponRen.enabled = true;
                        weaponSc.ShootingOn(true);
                    }
                }

                weaponSwap = false;
                weaponsChangeCoolOn = true;
                gameManager.ReloadingObj().SetActive(false);
            }
        }
    }

    /// <summary>
    /// 플레이어가 무기를 버릴 수 있게 담당하는 함수
    /// </summary>
    private void playerWeaponDrop()
    {
        if (weaponPrefabs.Count < 1 || weaponPrefabs == null) //무기 배열에 아무것도 없거나, 1보다 적으면 실행을 멈춤
        {
            return;
        }

        if (weaponDropTime < 0.0f)
        {
            weaponDrop = false;
            weaponDropTime = 0.5f;
        }

        if (Input.GetKeyDown(keyManager.DropItemKey()) && weaponDrop == false)
        {
            int count = weaponPrefabs.Count;
            int removeIndex = 0;
            for (int i = 0; i < count; i++)
            {
                SpriteRenderer weaponRen = weaponPrefabs[i].GetComponent<SpriteRenderer>();
                BoxCollider2D weaponBoxColl = weaponPrefabs[i].GetComponent<BoxCollider2D>();
                Weapons weaponSc = weaponPrefabs[i].GetComponent<Weapons>();
                WeaponSkill weaponSkillSc = weaponPrefabs[i].GetComponent<WeaponSkill>();

                if (weaponRen.enabled == true)
                {
                    weaponDrop = true; //떨어뜨렸다는 것을 확인
                    weaponSc.ShootingOn(false); //조정간 안전
                    weaponSc.PickUpImageOff(false); //아이템 줍기 키 이미지 재사용
                    weaponSc.WeaponGravityOff(false); //무기 아이템의 중력을 활성화
                    weaponSc.PassiveDamageUp(0); //무기에 적용된 패시브효과 삭제
                    weaponSc.ReturnDamage(); //무기를 기본값으로 되돌림
                    weaponSc.GetPlayerCriticalPcent(0, 1);
                    weaponSkillSc.WeaponSkillOff(false); //스킬 이미지 비활성화
                    weaponBoxColl.enabled = true; //무기의 박스콜라이더를 활성화
                    weaponRen.sortingOrder = -2;  //스프라이트의 오더 인 레이어를 -2로 변경
                    gameManager.ReloadingObj().SetActive(false);  //리로딩 UI 비활성화
                    weaponPrefabs[i].transform.SetParent(gameManager.ItemDropTrs());   //무기를 지정한 위치의 자식으로 넣어줌
                    weaponPrefabs[i].transform.position = gameObject.transform.position + new Vector3(0.0f, 0.1f, 0.0f);  //무기의 포지션은 플레이어의 포지션
                    weaponPrefabs[i].transform.rotation = gameManager.ItemDropTrs().rotation;  //회전 값은 지정한 위치의 회전 값을 받아옴
                    weaponPrefabs[i].transform.localScale = weaponLocalScale;  //지정한 스케일 수치만큼 변경
                    removeIndex = i;
                }
            }

            if (weaponPrefabs.Count == 1)
            {
                Weapons weaponScA = weaponPrefabs[0].GetComponent<Weapons>();

                weaponScA.ShootingOn(false);
                weaponScA.BuffDamage(0);
                playerWeaponA = 0;
            }
            else if (weaponPrefabs.Count == 2)
            {
                SpriteRenderer weaponRenA = weaponPrefabs[0].GetComponent<SpriteRenderer>();
                Weapons weaponScA = weaponPrefabs[0].GetComponent<Weapons>();
                SpriteRenderer weaponRenB = weaponPrefabs[1].GetComponent<SpriteRenderer>();
                Weapons weaponScB = weaponPrefabs[1].GetComponent<Weapons>();

                if (weaponRenA.enabled == true)
                {
                    weaponRenB.enabled = true;
                    weaponScA.ShootingOn(false);
                    weaponScB.ShootingOn(true);
                    weaponSwap = true;
                    weaponScA.BuffDamage(0);
                    playerWeaponA = 0;
                }
                else if (weaponRenB.enabled == true)
                {
                    weaponRenA.enabled = true;
                    weaponScA.ShootingOn(true);
                    weaponScB.ShootingOn(false);
                    weaponSwap = false;
                    weaponScB.BuffDamage(0);
                    playerWeaponB = 0;
                }
            }
            weaponPrefabs.RemoveAt(removeIndex);
        }
    }

    /// <summary>
    /// 플레이어가 받은 버프 공격력을 무기에 적용시키기 위한 함수
    /// </summary>
    private void playerBuffDamage()
    {
        if (weaponPrefabs.Count == 1)
        {
            Weapons weaponSc = weaponPrefabs[0].GetComponent<Weapons>();

            if (buffDamageUp == true)
            {
                weaponSc.BuffDamage(playerBuffDamageUp);
            }
            if (buffDamageUp == false)
            {
                weaponSc.BuffDamage(0);
            }
        }
        else if (weaponPrefabs.Count == 2)
        {
            int count = weaponPrefabs.Count;
            for (int i = 0; i < count; i++)
            {
                Weapons weaponSc = weaponPrefabs[i].GetComponent<Weapons>();
                weaponSc.BuffDamage(playerBuffDamageUp);

                if (buffDamageUp == true)
                {
                    weaponSc.BuffDamage(playerBuffDamageUp);
                }
                if (buffDamageUp == false)
                {
                    weaponSc.BuffDamage(0);
                }
            }
        }
    }

    /// <summary>
    /// 플레이어가 피격을 입었을 때 타격감을 주는 함수
    /// </summary>
    private void playerDamageHit()
    {
        if (playerHitDamage == true)
        {
            playerRen.color = Color.red;
        }
    }

    /// <summary>
    /// 플레이어의 피가 0이되면 작동하는 함수
    /// </summary>
    private void playerDead()
    {
        if (playerCurHp <= 0)
        {
            string hpText = $"0 / {playerMaxHp}";
            playerUI.SetPlayerHp(0, playerMaxHp, hpText);
            playerDaed = true;
        }
        else if (playerCurHp > 0 && transform.gameObject != null)
        {
            string hpText = $"{playerCurHp} / {playerMaxHp}";
            playerUI.SetPlayerHp(playerCurHp, playerMaxHp, hpText);
        }
    }

    /// <summary>
    /// 플레이어의 크리티컬을 담당하는 함수
    /// </summary>
    private void playerWeaponCritical()
    {
        if (weaponPrefabs != null)
        {
            int count = weaponPrefabs.Count;
            for (int i = 0; i < count; i++)
            {
                SpriteRenderer weaponRen = weaponPrefabs[i].GetComponent<SpriteRenderer>();
                Weapons weaponSc = weaponPrefabs[i].GetComponent<Weapons>();

                if (weaponRen.enabled == true)
                {
                    weaponSc.GetPlayerCriticalPcent(playerCritical, playerCriDamage);
                }
            }
        }
    }

    /// <summary>
    /// 펫이 주는 효과 버프
    /// </summary>
    private void petPassiveBuff()
    {
        if (weaponPrefabs != null || petSc != null)
        {
            int count = weaponPrefabs.Count;
            for (int i = 0; i < count; i++)
            {
                Weapons weaponSc = weaponPrefabs[i].GetComponent<Weapons>();
                weaponSc.PassiveDamageUp(playerDamage);
            }
        }
    }

    /// <summary>
    /// 플레이어가 레벨업을 할 수 있게 도와주는 함수
    /// </summary>
    private void playerLevelUpCheck()
    {
        if (playerExp >= playerMaxExp)
        {
            playerLevel++;
            levelPoint++;
            playerExp -= playerMaxExp;
            playerMaxExp *= 2;
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

        if (animTimer >= animJumpTime)
        {
            animIsJump = false;
            animTimer = 0.0f;
        }
    }

    /// <summary>
    ///  플레이어가 벽 점프를 할 수 있는 상황인지 아닌지 체크를 하기 위한 함수
    /// </summary>
    public void playerWallCheck(bool _wallHit, Collider2D _collision)
    {
        if (_wallHit == true && _collision.gameObject.layer == LayerMask.NameToLayer("Ground")
            && _collision.gameObject.tag == "JumpWall")
        {
            isWall = true;
        }
        else if (_wallHit == false && _collision.gameObject.layer == LayerMask.NameToLayer("Ground")
            && _collision.gameObject.tag == "JumpWall")
        {
            isWall = false;
        }

        if (_wallHit == true && _collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            knockBackTimer = 0;
            knockBack = false;
        }
    }

    /// <summary>
    /// 플레이어스킬 스크립트에서 받아올 스킬 타입
    /// </summary>
    /// <returns></returns>
    public PlayerSkillType SkillType()
    {
        return skillType;
    }

    /// <summary>
    /// 스킬 방향을 확인하기 위해 마우스 에임을 방향을 반환
    /// </summary>
    /// <returns></returns>
    public bool PlayerMouseAimRight()
    {
        return mouseAimRight;
    }

    /// <summary>
    /// 다른 스크립트에서 플레이어 Hp에 넣어진 값을 받아오기 위한 함수
    /// </summary>
    public void PlayerCurHp(int _damage, bool _hit, bool _trueDmg)
    {
        if (_trueDmg == true)
        {
            if (dashInvincibleOn == false)
            {
                playerCurHp -= _damage;
                playerHitDamage = _hit;
            }
        }
        else if (_trueDmg == false)
        {
            if (dashInvincibleOn == false)
            {
                int dmgReduction = _damage - playerArmor;
                if (dmgReduction <= 0)
                {
                    playerCurHp -= 1;
                    playerHitDamage = _hit;
                }
                else if (dmgReduction > 0)
                {
                    playerCurHp -= _damage - playerArmor;
                    playerHitDamage = _hit;
                }
            }
        }
    }

    public void GravityVelocityValue(float _gravityVelocity)
    {
        gravityVelocity = _gravityVelocity;
    }

    public float PlayerBuffDamage()
    {
        return playerBuffDamageUp;
    }

    public Vector3 PlayerMoveVec()
    {
        return moveVec;
    }

    /// <summary>
    /// 다른 스크립트에서 옵션창을 끄기 위한 함수
    /// </summary>
    public void OptionOff(bool _off)
    {
        optionOn = _off;
    }

    /// <summary>
    /// 플레이어 공격력을 상승시키는 함수
    /// </summary>
    /// <param name="_damageUp"></param>
    /// <param name="_armorUp"></param>
    /// <param name="_hpUp"></param>
    public void PlayerStatusDamage(int _damageUp)
    {
        playerDamage += _damageUp;
    }

    /// <summary>
    /// 플레이어 방어력을 상승시키는 함수
    /// </summary>
    /// <param name="_armorUp"></param>
    /// <param name="_armorUpPercent"></param>
    public void PlayerStatusArmor(int _armorUp)
    {
        playerArmor += _armorUp;
    }

    /// <summary>
    /// 플레이어 체력을 상승시키는 함수
    /// </summary>
    /// <param name="_hpUp"></param>
    /// <param name="_hpUpPercent"></param>
    public void PlayerStatusHp(int _hpUp)
    {
        playerMaxHp += _hpUp;
    }

    /// <summary>
    /// 플레이어 크리티컬확률과 데미지를 상승시키는 함수
    /// </summary>
    /// <param name="_criticalPercentage"></param>
    /// <param name="_criDamage"></param>
    public void PlayerStatusCritical(float _criticalPercentage, float _criDamage)
    {
        playerCritical += _criticalPercentage;
        playerCriDamage += _criDamage;
    }


    /// <summary>
    /// 세이브데이터를 받아와 플레이어에게 넣어 주는 함수
    /// </summary>
    /// <param name="_savedObject"></param>
    public void PlayerSavedData(SavedObjectData _savedObject)
    {
        playerDamage = _savedObject.playerDamage;
        playerArmor = _savedObject.playerArmor;
        playerMaxHp = _savedObject.playerHp;
        playerCurHp = _savedObject.playerCurHp;
        playerCritical = _savedObject.playerCritical;
        playerCriDamage = _savedObject.playerCriDamage;
        playerLevel = _savedObject.playerLevel;
        playerMaxExp = _savedObject.playerMaxExp;
        playerExp = _savedObject.playerExp;
        levelPoint = _savedObject.playerLevelPoint;
        playerWeaponA = _savedObject.playerWeaponA;
        playerWeaponB = _savedObject.playerWeaponB;
        playerPet = _savedObject.playerPet;
    }

    /// <summary>
    /// 적이 죽었을 때 받아올 경험치
    /// </summary>
    /// <param name="_exp"></param>
    public void SetPlayerExp(float _exp)
    {
        playerExp += _exp;
    }

    /// <summary>
    /// 플레이어가 데이터를 저장할 때 저장이 가능한지 불러올 변수
    /// </summary>
    /// <param name="_save"></param>
    public void PlayerSaveOn(bool _save)
    {
        dataSave = _save;
    }

    /// <summary>
    /// 레벨포인트를 이용해서 공격력을 증가시키는 함수
    /// </summary>
    public void playerPointDamageUp()
    {
        if (levelPoint > 0)
        {
            playerDamage += 1;
            levelPoint--;
        }
        else
        {
            levelPoint = 0;
        }
    }

    /// <summary>
    /// 레벨포인트를 이용해서 방어력을 증가시키는 함수
    /// </summary>
    public void playerPointArmorUp()
    {
        if (levelPoint > 0)
        {
            playerArmor += 1;
            levelPoint--;
        }
        else
        {
            levelPoint = 0;
        }
    }

    /// <summary>
    /// 레벨포인트를 이용해서 체력을 증가시키는 함수
    /// </summary>
    public void playerPointHpUp()
    {
        if (levelPoint > 0)
        {
            playerMaxHp += 10;
            levelPoint--;
        }
        else
        {
            levelPoint = 0;
        }
    }

    /// <summary>
    /// 레벨포인트를 이용해서 치명타확률을 증가시키는 함수
    /// </summary>
    public void playerPointCriticalUp()
    {
        if (levelPoint > 0)
        {
            playerCritical += 1;
            levelPoint--;
        }
        else
        {
            levelPoint = 0;
        }
    }

    /// <summary>
    /// 무기의 번호를 저장해 생성하기 위한 함수
    /// </summary>
    /// <returns></returns>
    public int GetPlayerWeaponA()
    {
        return playerWeaponA;
    }

    /// <summary>
    /// 무기의 번호를 저장해 생성하기 위한 함수
    /// </summary>
    /// <returns></returns>
    public int GetPlayerWeaponB()
    {
        return playerWeaponB;
    }

    /// <summary>
    /// 펫의 번호를 저장해 생성하기 위한 함수
    /// </summary>
    /// <returns></returns>
    public int GetPlayerPet()
    {
        return playerPet;
    }

    /// <summary>
    /// 다른 스키립트에서 이용할 무기 배열
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetWeaponPrefabs()
    {
        return weaponPrefabs;
    }

    public List<GameObject> GetPetPreFabs()
    {
        return petPrefabs;
    }

    /// <summary>
    /// 저장된 데이터로 생성한 무기를 배열에 다시 추가하는 함수
    /// </summary>
    /// <param name="_weapons"></param>
    public void SetWeaponList(GameObject _weapons)
    {
        weaponPrefabs.Add(_weapons);
    }

    public void SetPetList(GameObject _pet)
    {
        petPrefabs.Add(_pet);
    }

    public void PlayerKnockBack(float _xValue, float _yValue)
    {
        if (dashInvincibleOn == false)
        {
            knockBack = true;
            if (knockBack == true)
            {
                moveVec.x = _xValue;
                moveVec.y = _yValue;
            }
            else if (knockBack == false)
            {
                moveVec.x = 0;
                moveVec.y = 0;
            }
            rigid.velocity = moveVec;
        }
    }

    public void SetWeaponLocalScale(Vector3 _scale)
    {
        weaponLocalScale = _scale;
    }

    #region 스테이터스 스크립트에서 사용할 플레이어 능력치를 반환
    public float PlayerDamageReturn()
    {
        return playerDamage;
    }

    public float PlayerBuffDamageReturn()
    {
        return playerBuffDamageUp;
    }

    public int PlayerArmorReturn()
    {
        return playerArmor;
    }

    public int PlayerMaxHpReturn()
    {
        return playerMaxHp;
    }

    public int PlayerCrHpReturn()
    {
        return playerCurHp;
    }

    public float PlayerCriticalReturn()
    {
        return playerCritical;
    }

    public float PlayerCriDamageReturn()
    {
        return playerCriDamage;
    }

    public int PlayerLevelReturn()
    {
        return playerLevel;
    }

    public float PlayerMaxExpReturn()
    {
        return playerMaxExp;
    }

    public float PlayerExpReturn()
    {
        return playerExp;
    }

    public float PlayerLevelPointReturn()
    {
        return levelPoint;
    }

    public bool PlayerStatusOpen()
    {
        return statusOpen;
    }
    #endregion
}
