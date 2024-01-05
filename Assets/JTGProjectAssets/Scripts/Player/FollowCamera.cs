using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FollowCamera : MonoBehaviour
{
    private Vector3 position;

    private GameManager gameManager; //게임매니저

    [Header("카메라가 따라갈 오브젝트")]
    [SerializeField] private GameObject player;


    private void Start()
    {
        gameManager = GameManager.Instance;

        player = gameManager.PlayerPrefab();
    }
    private void Update()
    {
        cameraMoving();
    }

    private void cameraMoving()
    {
        if (player == null)
        {
            return;
        }

        position = player.transform.position;
        position.y = player.transform.position.y + 2f;
        position.z = transform.position.z;
        transform.position = position;
    }
}
