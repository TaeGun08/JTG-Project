using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MoveGround : MonoBehaviour
{
    private Vector3 moveVec;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveChangeTime;
    private float moveChangeTimer;
    [SerializeField] private Transform parentTrs;
    [SerializeField] private Transform backTrs;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject != null)
        {
            collision.gameObject.transform.SetParent(parentTrs);
        }
        else if (collision.gameObject.tag == "Enemy")
        {

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject != null)
        {
            collision.gameObject.transform.SetParent(backTrs);
        }
        else if (collision.gameObject.tag == "Enemy")
        {

        }
    }

    private void Awake()
    {
        moveVec = new Vector3(0, 1, 0);
    }

    private void Update()
    {
        moveChange();
        move();
    }

    /// <summary>
    /// 아이템 움직임을 일정 시간이 지나면 위아래를 바꿔주는 함수
    /// </summary>
    private void moveChange()
    {
        moveChangeTimer += Time.deltaTime;

        if (moveChangeTimer >= moveChangeTime)
        {
            moveChangeTimer = 0;
            moveVec.y *= -1f;
        }
    }

    private void move()
    {
        transform.position += moveVec * moveSpeed * Time.deltaTime;
    }
}
