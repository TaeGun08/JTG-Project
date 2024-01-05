using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private int trapDamage;
    [SerializeField] private bool isHit = false;
    [SerializeField] private float hitTimer;

    private Player playerSc;
    private Enemy enemySc;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerSc = collision.gameObject.GetComponent<Player>();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemySc = collision.gameObject.GetComponent<Enemy>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isHit = true;
            playerSc = null;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            isHit = true;
            enemySc = null;
        }
    }

    private void Awake()
    {
        hitTimer = 0.5f;
    }

    private void Update()
    {
        hitDamageTimer();
        hitDamage();
    }
   

    private void hitDamageTimer()
    {
        hitTimer -= Time.deltaTime;
        if (hitTimer < 0)
        {
            isHit = false;
            hitTimer = 0.5f;
        }
    }

    private void hitDamage()
    {
        if (isHit == false && playerSc != null)
        {
            isHit = true;
            playerSc.PlayerCurHp(trapDamage, isHit, true);
            playerSc.GravityVelocityValue(3);
        }
        else if (isHit == false && enemySc != null)
        {
            isHit = true;
            enemySc.EnemyHp(trapDamage, isHit, true, false);
        }
    }
}
