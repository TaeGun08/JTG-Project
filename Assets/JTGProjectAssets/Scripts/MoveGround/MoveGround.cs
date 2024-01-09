using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MoveGround : MonoBehaviour
{
    [SerializeField] private Vector3 moveVec;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveChangeTime;
    private float moveChangeTimer;
    [SerializeField] private Transform parentTrs;
    [SerializeField] private Transform playerBackTrs;
    [SerializeField] private Transform enemyBackTrs;
    [SerializeField] private Transform petBackTrs;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && collision.gameObject != null)
        {
            collision.gameObject.transform.SetParent(parentTrs);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {

        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Pet"))
        {
            collision.gameObject.transform.SetParent(parentTrs);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && collision.gameObject != null)
        {
            collision.gameObject.transform.SetParent(playerBackTrs);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {

        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Pet"))
        {
            collision.gameObject.transform.SetParent(petBackTrs);
        }
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
            moveVec *= -1f;
        }
    }

    private void move()
    {
        transform.position += moveVec * moveSpeed * Time.deltaTime;
    }
}
