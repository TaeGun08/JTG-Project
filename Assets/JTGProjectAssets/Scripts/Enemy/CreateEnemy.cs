using System.Collections.Generic;
using UnityEngine;

public class CreateEnemy : MonoBehaviour
{
    private PoolManager poolManager;

    [Header("적 생성 관련 설정")]
    [SerializeField, Tooltip("생성할 적의 번호")] private int enemyCreateNumber;
    [SerializeField, Tooltip("적을 생성할 위치")] private List<Transform> enemyCreatePos;
    [SerializeField, Tooltip("생성할 적의 타입")] private string enemyCreateType;
    private bool isCreate = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            enemyCreate();
        }
    }

    private void Start()
    {
        poolManager = PoolManager.Instance;
    }

    private void enemyCreate()
    {
        if (isCreate == false)
        {
            for (int i = 0; i < enemyCreatePos.Count; i++)
            {
                poolManager.PoolingPrefab(enemyCreateNumber, enemyCreatePos[i], enemyCreateType);
            }
            isCreate = true;
        }
    }
}
