using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEnemy : MonoBehaviour
{
    [Header("利 积己 包访 汲沥")]
    [SerializeField] private List<GameObject> enemyPrefab = new List<GameObject>();
    [SerializeField] private Transform enemyCreatePos;

    [Header("积己且 利 橇府普")]
    [SerializeField] private int enemyPrefabNember;

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
        if (isCreate == false && (enemyPrefabNember <= enemyPrefab.Count - 1))
        {
            Instantiate(enemyPrefab[enemyPrefabNember], enemyCreatePos.position, Quaternion.identity, trashPreFab.transform);
            isCreate = true;
        }
    }
}
