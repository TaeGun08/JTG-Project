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
            isCreate = true;
        }
    }

    private void Start()
    {
        trashPreFab = TrashPreFab.instance;
    }

    private void Update()
    {
        enemyCreate();
    }

    private void enemyCreate()
    {
        if (isCreate == true)
        {
            for (int i = 0; i < enemyPrefab.Count; i++)
            {
                Instantiate(enemyPrefab[i], enemyCreatePos[i].position, Quaternion.identity, trashPreFab.transform);
            }
            isCreate = false;
        }
    }
}
