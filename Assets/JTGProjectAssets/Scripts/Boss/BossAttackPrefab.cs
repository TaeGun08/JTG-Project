using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackPrefab : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;

    private float attackTimer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player playerSc = collision.gameObject.GetComponent<Player>();
            playerSc.PlayerCurHp(30, true, true);
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
        if (attackTimer > 0.6f)
        {
            boxCollider2D.enabled = true;
        }
    }

    public void AttackEnd()
    {
        Destroy(gameObject);
    }
}
