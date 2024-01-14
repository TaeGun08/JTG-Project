using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPhaseEnemy : MonoBehaviour
{
    [Header("적을 일정 수 처치했을 때 투명벽 비활성화")]
    [SerializeField, Tooltip("처치해야하는 적의 수")] private int enemyKill;
    private int enemyCount;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemyCount--;
        }
    }

    private void Awake()
    {
        enemyCount = enemyKill;
    }

    private void Update()
    {
        nextPhaseOn();
    }

    /// <summary>
    /// 일정 수의 적을 처치 시 투명벽 해제
    /// </summary>
    private void nextPhaseOn()
    {
        if (enemyCount <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
