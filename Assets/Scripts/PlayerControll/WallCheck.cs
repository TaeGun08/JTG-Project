using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
    public enum WallCheckType
    {
        wallHitOn,
        wallHitOff,
    }

    [SerializeField] private WallCheckType checkType;

    private bool isWallHit = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isWallHit  = true; //박스 콜라이더에 벽이 닿았다면 true
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isWallHit = false;//박스 콜라이더에 벽이 닿지 않았다면 false
    }

    /// <summary>
    /// isWallHit의 값을 받기 위한 함수
    /// </summary>
    /// <returns></returns>
    public bool WallHit()
    {
        return isWallHit;
    } 
    
    public WallCheckType wallCheckType()
    {
        return checkType;
    }
}
