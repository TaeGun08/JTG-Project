using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEnemy : MonoBehaviour
{
    [Header("적 생성 관련 설정")]
    [SerializeField, Tooltip("생성할 적의 프리팹")] private List<GameObject> enemyPrefab;
    [SerializeField, Tooltip("적을 생성할 위치")] private List<Transform> enemyCreatePosA;
    [SerializeField, Tooltip("적을 생성할 위치")] private List<Transform> enemyCreatePosB;
    [SerializeField, Tooltip("적을 생성할 위치")] private List<Transform> enemyCreatePosC;
    [SerializeField, Tooltip("적을 생성할 위치")] private List<Transform> enemyCreatePosD;
    [SerializeField, Tooltip("적을 생성할 위치")] private List<Transform> enemyCreatePosE;
    [SerializeField, Tooltip("처음으로 생성한 오브젝트가 죽으면 재사용")] private int enemyPhase;
    [SerializeField, Tooltip("닿았을 때 생성을 위한 콜라이더")] private List<BoxCollider2D> boxCollider2D;
    private int phase;
    private List<GameObject> enemyPrefabList = new List<GameObject>();

    private TrashPreFab trashPreFab;

    private void Awake()
    {
        phase = enemyPhase;
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
        if (phase != -1)
        {
            if (phase == enemyPhase)
            {
                for (int i = 0; i < enemyPrefab.Count; i++)
                {
                    GameObject enemyObj = Instantiate(enemyPrefab[i], enemyCreatePosA[i].position, Quaternion.identity, trashPreFab.transform);
                    enemyPrefabList.Add(enemyObj);
                }
                phase--;
            }
            else if (phase < enemyPhase)
            {
                for (int i = 0; i < enemyPrefabList.Count; i++)
                {
                    enemyPrefabList[i].transform.position = enemyCreatePosB[i].position;
                    enemyPrefab[i].SetActive(true);
                }
                phase--;
            }
        }
    }
}
