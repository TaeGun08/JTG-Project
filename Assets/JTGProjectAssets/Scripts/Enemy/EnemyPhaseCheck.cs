using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPhaseCheck : MonoBehaviour
{
    [SerializeField, Tooltip("적 생성 스크립트")] private CreateEnemy createEnemy;

    [SerializeField, Tooltip("다음 페이즈 오브젝트")] private GameObject nextPhaseObj;
    private int enemyCount;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemyCount--;
        }
    }

    private void Start()
    {
        enemyCount = createEnemy.GetCreatePosCount();
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
            createEnemy.gameObject.SetActive(false);
            nextPhaseObj.SetActive(true);
        }
    }
}
