using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEnemy : MonoBehaviour
{
    [Header("利 积己 包访 汲沥")]
    [SerializeField] private List<GameObject> enemyPrefab = new List<GameObject>();
    [SerializeField] private List<Transform> enemyCreatePos = new List<Transform>();

    private TrashPreFab trashPreFab;

    private bool isCreate = false;

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
        if (isCreate == false)
        {
            Instantiate(enemyPrefab[0], enemyCreatePos[0].position, Quaternion.identity, trashPreFab.transform);
            isCreate = true;
        }
    }
}
