using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public enum ItemType
    {
        Weapon,
        Buff,
        Pet
    }

    [SerializeField] ItemType itemType;

    public ItemType GetItemType() //아이템 타입을 반환하기 위한 함수
    {
        return itemType;
    }
}
