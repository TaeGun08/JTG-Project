using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEnemy : MonoBehaviour
{
    [Header("利 积己 包访 汲沥")]
    [SerializeField] private List<GameObject> enemyPrefab;
    [SerializeField] private List<Transform> enemyCreatePos;

    private TrashPreFab trashPreFab;

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
        trashPreFab = TrashPreFab.instance;
    }

    private void enemyCreate()
    {
        if (isCreate == false)
        {
            for (int i = 0; i < enemyPrefab.Count; i++)
            {
                Instantiate(enemyPrefab[i], enemyCreatePos[i].position, Quaternion.identity, trashPreFab.transform);
            }
            isCreate = true;
        }
    }
}
