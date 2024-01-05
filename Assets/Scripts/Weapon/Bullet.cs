using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletType
    {
        playerBullet,
        playerSkillBullet,
        enemyBullet,
    }

    [SerializeField] BulletType bulletType;

    [Header("총알 설정")]
    [SerializeField, Tooltip("총알이 날아가는 속도")] private float bulletSpeed = 1.0f;
    [SerializeField, Tooltip("총알의 공격력")] private float bulletDamage = 1.0f;

    private bool damageUpOn = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (bulletType.ToString() == "playerBullet")
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Enemy enemySc = collision.gameObject.GetComponent<Enemy>();
                enemySc.EnemyHp((int)bulletDamage, true, false);
                Destroy(gameObject);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Destroy(gameObject);
            }
        }
        else if (bulletType.ToString() == "playerSkillBullet")
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Enemy enemySc = collision.gameObject.GetComponent<Enemy>();
                enemySc.EnemyHp((int)bulletDamage, true, false);
            }
        }
        else if (bulletType.ToString() == "enemyBullet")
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Player playerSc = collision.gameObject.GetComponent<Player>();
                playerSc.PlayerCurHp((int)bulletDamage, true, false);
                Destroy(gameObject);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Destroy(gameObject);
            }
        }
    }

    private void Update()
    {
        shootBullet();
        Destroy(gameObject, 1);
    }

    private void shootBullet()
    {
        Vector3 bulletMove = transform.up * bulletSpeed * Time.deltaTime;
        transform.position += bulletMove;
    }

    public void BulletDamage(float _bulletDamage, float _bulletUpDamage, bool _damageUpOn)
    {
        damageUpOn = _damageUpOn;

        if (damageUpOn == false)
        {
            bulletDamage = _bulletDamage;
        }
        else if (damageUpOn == true)
        {
            bulletDamage = _bulletDamage * _bulletUpDamage;
        }
    }
}
