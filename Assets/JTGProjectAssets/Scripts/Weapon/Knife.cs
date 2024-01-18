using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Vector2 moveForce;
    private Vector3 trsRotate;
    private bool isRight = false;

    [SerializeField] private float knifeDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Enemy enemySc = collision.gameObject.GetComponent<Enemy>();
            enemySc.EnemyHp((int)knifeDamage, true, true, false);
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Boss"))
        {
            Boss bossSc = collision.gameObject.GetComponent<Boss>();
            bossSc.BossHp((int)knifeDamage, true, true, false);
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rigid.AddForce(moveForce, ForceMode2D.Impulse);
    }

    private void Update()
    {
        objRotate();
    }

    private void objRotate()
    {
        trsRotate = new Vector3(0, 0, -360);
        if (isRight == false)
        {
            trsRotate = new Vector3(0, 0, 360);
        }

        transform.Rotate(trsRotate * Time.deltaTime * 2);

        Destroy(gameObject, 0.5f);
    }

    public void KnifeForce(Vector2 _moveForce, bool _isRight)
    {
        moveForce = _moveForce;
        isRight = _isRight;

        if (isRight == false)
        {
            moveForce = -_moveForce;
        }
    }

    public void KnifeDamage(float _damage)
    {
        knifeDamage = _damage;
    }
}
