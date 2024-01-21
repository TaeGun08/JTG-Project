using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetAttack : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;

    private float attackTimer;
    private float skillDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Enemy enemySc = collision.gameObject.GetComponent<Enemy>();
            enemySc.EnemyHp((int)skillDamage, true, true, false);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Boss"))
        {
            Boss bossSc = collision.gameObject.GetComponent<Boss>();
            bossSc.BossHp((int)skillDamage, true, true, false);
        }
    }

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        boxCollider2D.enabled = false;

        attackTimer = 0;
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer > 0.3f && attackTimer <= 0.4f)
        {
            boxCollider2D.enabled = true;
        }
        else if (attackTimer > 0.4f)
        {
            boxCollider2D.enabled = false;
        }
    }

    public void PetSkillDamage(float _damage)
    {
        skillDamage = _damage;
    }
}
