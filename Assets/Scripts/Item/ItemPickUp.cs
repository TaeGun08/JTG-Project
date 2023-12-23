using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public enum ItemType
    {
        WeaponA,
        WeaponB,
        WeaponC,
        WeaponD,
    }

    [SerializeField] ItemType itemType;

    public ItemType GetItemType()
    {
        return itemType;
    }
}
