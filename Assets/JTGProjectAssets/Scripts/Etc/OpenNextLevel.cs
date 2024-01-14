using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenNextLevel : MonoBehaviour
{
    [Header("적을 일정 수 처치했을 때 활성화되는 오브젝트")]
    [SerializeField] private GameObject nextLevel;
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
        nextLevelOn();
    }

    /// <summary>
    /// 일정 수의 적을 처치 시 다음 레벨의 문이 열림
    /// </summary>
    private void nextLevelOn()
    {
        if (enemyCount <= 0)
        {
            nextLevel.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
