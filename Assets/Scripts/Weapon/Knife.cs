using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Vector2 moveForce;
    private Vector3 trsRotate;
    private bool isRight = false;

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
}
