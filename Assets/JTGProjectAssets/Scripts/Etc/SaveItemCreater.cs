using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveItemCreater : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private Player player;
    private Transform playerHand;

    [Header("게임씬에 소환할 오브젝트")]
    [SerializeField] private List<GameObject> weapons;
    [SerializeField] private List<GameObject> pet;

    private void Start()
    {
        gameManager = GameManager.Instance;

        playerHand = player.transform.Find("PlayerHand");

        weaponCreate();
        petCreate();
    }

    /// <summary>
    /// 저장한 무기 번호에 맞는 무기를 생성해주는 함수
    /// </summary>
    private void weaponCreate()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (player.GetPlayerWeaponA() == i + 1)
            {
                GameObject weaponA = Instantiate(weapons[i], playerHand.position, playerHand.rotation, playerHand);
                weaponA.GetComponent<BoxCollider2D>().enabled = false;
                Weapons getWeaponSc = weaponA.GetComponent<Weapons>();
                getWeaponSc.ShootingOn(true);
                getWeaponSc.PickUpImageOff(true);
                getWeaponSc.WeaponGravityOff(true);
                getWeaponSc.WeaponUseShooting(false);
                SpriteRenderer getWeaponRen = weaponA.GetComponent<SpriteRenderer>();
                getWeaponRen.sortingOrder = 2;
                player.SetWeaponList(weaponA);

                if (player.GetPlayerWeaponB() > 0)
                {
                    getWeaponSc.ShootingOn(false);
                    getWeaponRen.enabled = false;
                }
            }

            if (player.GetPlayerWeaponB() == i + 1)
            {
                GameObject weaponB = Instantiate(weapons[i], playerHand.position, playerHand.rotation, playerHand);
                weaponB.GetComponent<BoxCollider2D>().enabled = false;
                Weapons getWeaponSc = weaponB.GetComponent<Weapons>();
                getWeaponSc.ShootingOn(true);
                getWeaponSc.PickUpImageOff(true);
                getWeaponSc.WeaponGravityOff(true);
                getWeaponSc.WeaponUseShooting(false);
                SpriteRenderer getWeaponRen = weaponB.GetComponent<SpriteRenderer>();
                getWeaponRen.sortingOrder = 2;
                player.SetWeaponList(weaponB);
            }
        }
    }

    /// <summary>
    /// 저장한 펫의 번호에 맞는 펫을 생성해주는 함수
    /// </summary>
    private void petCreate()
    {
        for (int i = 0; i < pet.Count; i++)
        {
            if (player.GetPlayerPet() == i + 1)
            {
                GameObject petObj = Instantiate(pet[i], player.transform.position, Quaternion.identity, gameManager.ItemDropTrs());
                Pet petSc = petObj.GetComponent<Pet>();
                petSc.GetPetCheck(true);
                petSc.SetPetPassiveOn(true);
                player.SetPetList(petObj);
            }
        }
    }
}
