using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEnemy : MonoBehaviour
{
    private GameManager gameManager;

    private bool isCreate = false;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        enemyCreate();
    }

    private void enemyCreate()
    {
        if (isCreate == false)
        {
            Instantiate(gameManager.EnemyPrefab()[0], gameManager.EnemyCreatePos()[0]);
            isCreate = true;
        }
    }
}
