using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public enum BuffType
    {
        damage,
        armor,
        heal,
    }

    [Header("버프 능력")]
    [SerializeField] BuffType buffType;

    [Header("버프 수치")]
    [SerializeField] private float buffValue;

    [Header("움직임 설정")]
    [SerializeField, Tooltip("버프 아이템의 움직임")] private float buffMove;
    private Vector3 moveVec;
    private Vector3 moveRot;
    private float changeTimer;

    private void Awake()
    {
        moveVec.y = 1f;
        moveRot.y = 360;
    }

    private void Update()
    {
        moveChangeTime();
        itmeMove();
    }

    /// <summary>
    /// 아이템 움직임을 일정 시간이 지나면 위아래를 바꿔주는 함수
    /// </summary>
    private void moveChangeTime()
    {
        changeTimer += Time.deltaTime;

        if (changeTimer >= 0.5f)
        {
            changeTimer = 0;
            moveVec.y *= -1f;
        }
    }

    /// <summary>
    /// 아이템의 움직임을 담당하는 함수
    /// </summary>
    private void itmeMove()
    {

        transform.position += moveVec * buffMove * Time.deltaTime;
        transform.Rotate(moveRot * Time.deltaTime * 0.5f);
    }

    public float BuffTypeValue()
    {
        float bufftypeValue = 0;
        if (buffType.ToString() == "damage")
        {
            bufftypeValue = buffValue;
        }
        else if (buffType.ToString() == "armor")
        {
            bufftypeValue = buffValue;
        }
        else if (buffType.ToString() == "heal")
        {
            bufftypeValue = buffValue;
        }
        return bufftypeValue;
    }

    public BuffType GetBuffType()
    {
        return buffType;
    }
}
