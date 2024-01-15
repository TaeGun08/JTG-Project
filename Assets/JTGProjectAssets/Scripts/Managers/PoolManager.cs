using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [Header("풀링을 이용해 생성할 프리팹")]
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] private List<GameObject> pools = new List<GameObject>();

    public class poolingObj
    {
        public GameObject poolObject;
        public string sTag;
    }

    private TrashPreFab trashPreFab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        trashPreFab = TrashPreFab.Instance;
    }

    /// <summary>
    /// 풀링을 하기 위한 함수
    /// </summary>
    /// <param name="_pool"></param>
    /// <param name="_trs"></param>
    public void PoolingPrefab(int _pool, Transform _trs, string _type)
    {
        GameObject poolObj = null;

        if (pools != null)
        {
            for (int i = 0; i < pools.Count; i++)
            {
                if (pools[i].tag == "Enemy")
                {
                    Enemy enemySc = pools[i].GetComponent<Enemy>();
                    Enemy.EnemyType enemyType = enemySc.GetEnemyType();
                    if (pools[i].activeSelf == false && enemyType.ToString() == _type)
                    {
                        poolObj = pools[i];
                        poolObj.transform.position = _trs.position;
                        poolObj.SetActive(true);
                        break;
                    }
                }
            }
        }

        if (poolObj == null)
        {
            poolObj = Instantiate(prefabs[_pool], _trs.position, Quaternion.identity, trashPreFab.transform);
            pools.Add(poolObj);
        }
    }
}
