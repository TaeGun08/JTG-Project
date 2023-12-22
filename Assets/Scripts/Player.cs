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
    [SerializeField] private float speed = 1.0f; //플레이어의 이동속도
    private Vector2 moveVec; //플레이어의 움직임을 위한 벡터
    private bool leftKey; //왼쪽 키를 눌렀을 때
    private bool rightKey; //오른쪽 키를 눌렀을 때

    private bool isGround = false; //플레이어가 땅에 붙어있지 않으면 false
    private float gravityVelocity;  //중력과 관련된 값을 받아오기 위한 변수

    [Header("점프")]
    [SerializeField] private float jumpPower = 1.0f; //점프를 하기 위한 힘
    private bool isJump = false; //점프를 했는지
    private bool jumpKey; //점프 키를 눌렀을 때
    private bool animIsJump = false;
    [SerializeField] private float animJumpTime = 1.0f;
    private float animTimer = 0.0f;

    private void OnDrawGizmos() //박스캐스트를 씬뷰에서 눈으로 확인이 가능하게 보여줌
    {
        if (playerBoxColl2D != null)
        {
            Gizmos.color = Color.red;
            Vector3 pos = playerBoxColl2D.bounds.center - new Vector3(0, 0.1f, 0);
            Gizmos.DrawWireCube(pos, playerBoxColl2D.bounds.size);
        }
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>(); //플레이어 자신의 리지드바디를 가져옴
        playerBoxColl2D = GetComponent<BoxCollider2D>(); //플레이어 자신의 박스콜라이더2D를 가져옴
        anim = GetComponent<Animator>(); //플레이어의 애니메이션을 가져옴
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        keyManager = KeyManager.instance;

        mainCam = Camera.main;
    }

    private void Update()
    {
        if (gameManager.GamePause() == true)
        {
            return;
        }

        playerCheckGround();
        playrAim();
        playerMove();
        playerGravity();
        playerAni();
    }

    /// <summary>
    /// 플레이어가 땅인지 아닌지 확인을 담당하는 함수
    /// </summary>
    private void playerCheckGround()
    {
        isGround = false;

        if (gravityVelocity > 0)
        {
            return;
        }

        hit2D = Physics2D.BoxCast(playerBoxColl2D.bounds.center, playerBoxColl2D.bounds.size,
            0.0f, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));

        if (hit2D.transform != null && hit2D.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGround = true;
            isJump = false;
        }
    }

    /// <summary>
    /// 플레이어의 마우스 에임
    /// </summary>
    private void playrAim()
    {
        Vector3 mouseInputPos = Input.mousePosition;
        mouseInputPos.z = -mainCam.transform.position.z;
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(mouseInputPos);

        Vector3 mouseDistance = mouseWorldPos - transform.position;

        Vector3 scale = transform.localScale;
        scale.x *= -1;

        if (mouseDistance.x > 0 && transform.localScale.x != 1)
        {
            transform.localScale = scale;
        }
        else if (mouseDistance.x < 0 && transform.localScale.x != -1)
        {
            transform.localScale = scale;
        }
    }

    /// <summary>
    /// 플레이어를 좌우로 움직이는걸 담당하는 함수
    /// </summary>
    private void playerMove()
    {
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
        moveVec.y = gravityVelocity;
        rigid.velocity = moveVec;
    }

    /// <summary>
    /// 플레이어의 점프를 담당하는 함수
    /// </summary>
    private void playerJump()
    {
        jumpKey = Input.GetKeyDown(keyManager.PlayerJumpKey());

        if (jumpKey == true && isGround == true)
        {
            gravityVelocity = jumpPower;
            animIsJump = true;
            //isJump = true;
        }
    }

    /// <summary>
    /// 플레이어의 중력을 담당하는 함수
    /// </summary>
    private void playerGravity()
    {
        if (isGround == false)
        {
            gravityVelocity -= gameManager.gravityScale() * Time.deltaTime; //지속적으로 받는 중력         
        }
        else
        {
            gravityVelocity = -1;
        }

        playerJump();
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
