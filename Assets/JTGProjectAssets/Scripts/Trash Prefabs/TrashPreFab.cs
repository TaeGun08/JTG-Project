using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashPreFab : MonoBehaviour
{
    public static TrashPreFab Instance; //사라질 프리팹들을 담을 스크립트

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
}
