using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletType
    {
        playerBullet,
        enemyBullet
    }

    [SerializeField] BulletType bulletType;

    [Header("총알 속도")]
    [SerializeField, Tooltip("총알이 날아가는 속도")] private float bulletSpeed = 1.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "Weapon" 
            && collision.gameObject.tag != "Bullet")
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        shootBullet();
        Destroy(gameObject, 1f);
    }

    private void shootBullet()
    {
        Vector3 bulletMove = transform.up * bulletSpeed * Time.deltaTime;
        transform.position += bulletMove;
    }
}
