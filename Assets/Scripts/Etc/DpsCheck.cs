using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DpsCheck : MonoBehaviour
{
    private Vector3 moveVec;

    [SerializeField] private float moveSpeed;

    private void Awake()
    {
        moveVec.y = 1;
    }

    private void Update()
    {
        move();
        Destroy(gameObject, 1);
    }

    private void move()
    {
        transform.position += moveVec * moveSpeed * Time.deltaTime;
    }
}
