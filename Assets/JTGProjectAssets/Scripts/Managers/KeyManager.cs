using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public static KeyManager instance;

    [Header("플레이어 키 설정")]
    [SerializeField] KeyCode playerMoveLeftKey;
    [SerializeField] KeyCode playerMoveRightKey;
    [SerializeField] KeyCode playerJumpKey;
    [SerializeField] KeyCode playerAttackKey;
    [SerializeField] KeyCode playerSpecialAttackKey;
    [SerializeField] KeyCode playerDashKey;
    [SerializeField] KeyCode reroadingKey;
    [SerializeField] KeyCode weaponChangeKey;
    [SerializeField] KeyCode weaponSkiilKey;
    [SerializeField] KeyCode pickUpItemKey;
    [SerializeField] KeyCode dropItemKey;
    [SerializeField] KeyCode interactionKey;
    [SerializeField] KeyCode optionKey;
    [SerializeField] KeyCode statuswindowKey;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public KeyCode PlayerLeftKey()
    {
        return playerMoveLeftKey;
    }

    public KeyCode PlayerRightKey()
    {
        return playerMoveRightKey;
    }

    public KeyCode PlayerJumpKey()
    {
        return playerJumpKey;
    }

    public KeyCode PlayerAttackKey()
    {
        return playerAttackKey;
    }

    public KeyCode PlayerSpecialAttackKey()
    {
        return playerSpecialAttackKey;
    }

    public KeyCode PlayerDashKey()
    {
        return playerDashKey;
    }

    public KeyCode ReloadingKey()
    {
        return reroadingKey;
    }

    public KeyCode WeaponChangeKey()
    {
        return weaponChangeKey;
    }

    public KeyCode WeaponSkiilKey()
    {
        return weaponSkiilKey;
    }

    public KeyCode PickUpItemKey()
    {
        return pickUpItemKey;
    }

    public KeyCode DropItemKey()
    {
        return dropItemKey;
    }

    public KeyCode InteractionKey()
    {
        return interactionKey;
    }

    public KeyCode OptionKey()
    {
        return optionKey;
    }

    public KeyCode StatuswindowKey()
    {
        return statuswindowKey;
    }
}
