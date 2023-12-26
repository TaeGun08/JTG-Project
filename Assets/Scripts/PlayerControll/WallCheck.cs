using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
    [SerializeField] private Player playerSc;

    private bool isWallHit = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerSc == null)
        {
            return;
        }

        isWallHit  = true; //박스 콜라이더에 벽이 닿았다면 true
        playerSc.playerWallCheck(isWallHit, collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerSc == null)
        {
            return;
        }

        isWallHit = false;//박스 콜라이더에 벽이 닿지 않았다면 false
        playerSc.playerWallCheck(isWallHit, collision);
    }
}
