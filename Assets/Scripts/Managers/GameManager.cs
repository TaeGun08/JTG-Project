using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("게임 중지")]
    [SerializeField] private bool gamePause = false;

    [Header("중력")]
    [SerializeField] private float gravity;

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

    public bool GamePause()
    {
        return gamePause;
    }

    public float gravityScale()
    {
        return gravity;
    }
}
