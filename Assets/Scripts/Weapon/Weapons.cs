using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Weapons : MonoBehaviour
{
    public enum weaponSkillType
    {
        skillTypeA,
        skillTypeB,
        skillTypeC,
        skillTypeD,
    }

    [SerializeField] private weaponSkillType skillType;

    private BoxCollider2D weaponBoxColl2D;

    //무기에 가져올 매니저
    private GameManager gameManager; //게임매니저
    private KeyManager keyManager; //키매니저

    private TrashPreFab trashPreFab;

    private ItemPickUp itemPickUp;
    //private int weaponNum = 0;

    [Header("공격 설정")]
    [SerializeField, Tooltip("공격 딜레이")] private float shootDelay = 0.5f;
    private float shootTimer;
    [SerializeField, Tooltip("조정간")] private bool shootingOn = false;

    [Header("총알 설정")]
    [SerializeField, Tooltip("총알 프리팹")] private GameObject bullet;
    [SerializeField, Tooltip("총알이 나가는 위치")] private Transform bulletPos;
    [Space]
    [SerializeField, Tooltip("최대 탄창")] private int maxMagazine; //탄창에 들어가는 최대 총알 수
    [SerializeField, Tooltip("현재 탄창")] private int curMagazine; //현재 탕창에 보유중인 총알 수
    [SerializeField, Tooltip("재장전 시간")] private float reloadingTime; //재장전을 위한 시간
    private float reloadingTimer;
    private bool reloading = false;
    private float curReloadingSlider;

    [Header("줍기 키 이미지")]
    [SerializeField] private GameObject pickUpKeyImage;
    private bool imageOff = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (imageOff == true)
        {
            return;
        }

        if (collision.gameObject.tag == "Player")
        {
            pickUpKeyImage.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (imageOff == true)
        {
            return;
        }

        if (collision.gameObject.tag == "Player")
        {
            pickUpKeyImage.SetActive(false);
        }
    }

    private void Awake()
    {
        itemPickUp = GetComponent<ItemPickUp>();
        weaponBoxColl2D = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance; //게임 매니저를 가져와 gameManager에 담아 줌
        keyManager = KeyManager.instance; //키매니저를 가져와 keyManager 담아 줌

        trashPreFab = TrashPreFab.instance;

        curReloadingSlider = gameManager.ReloadingUI().value;

        pickUpKeyImage.SetActive(false);
    }

    private void Update()
    {
        if (shootingOn == false || itemPickUp.GetItemType().ToString() != "Weapon")
        {
            return;
        }

        if (imageOff == true)
        {
            pickUpKeyImage.SetActive(false);
        }

        reloadingWeapon();
        shootWeapon();
    }

    /// <summary>
    /// 총의 재장전을 담당하는 함수
    /// </summary>
    private void reloadingWeapon()
    {
        if (Input.GetKeyDown(keyManager.ReloadingKey()))
        {
            curMagazine = 0;
        }

        if (reloading == true) //재장전이 true면 타이머를 작동시고 UI를 활성화함
        {
            reloadingTimer -= Time.deltaTime;
            gameManager.ReloadingUI().value = reloadingTimer;
            gameManager.ReloadingObj().SetActive(true);
            if (reloadingTimer < 0)
            {
                gameManager.ReloadingObj().SetActive(false);
                gameManager.ReloadingUI().value = 1.0f;
                reloadingTimer = reloadingTime;
                curMagazine = maxMagazine;
                reloading = false;
            }
        }
    }

    /// <summary>
    /// 총을 발사를 담당하는 함수
    /// </summary>
    private void shootWeapon()
    {
        if (curMagazine <= 0) //현재 총알이 없다면 재장전을 true로 바꾸고 발사를 못 하게 함
        {
            reloading = true;
            return;
        }

        if (shootTimer != 0.0f) //발사 딜레이
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer < 0)
            {
                shootTimer = 0.0f;
            }
        }

        if (Input.GetKeyDown(keyManager.PlayerAttackKey()) && shootTimer == 0.0f && reloading == false) //마우스를 누를 때 마다 발사
        {
            shootBullet();
            curMagazine--;
            shootTimer = shootDelay;
        }
        else if (Input.GetKey(keyManager.PlayerAttackKey()) && shootTimer == 0.0f && reloading == false) //마우스를 누르고 있으면 발사
        {
            shootBullet();
            curMagazine--;
            shootTimer = shootDelay;
        }
    }

    /// <summary>
    /// 총알 생성을 담당하는 함수
    /// </summary>
    /// <param name="_rot"></param>
    private void shootBullet(float _rot = 0.0f)
    {
        Instantiate(bullet, bulletPos.position, bulletPos.rotation, trashPreFab.transform);
    }

    /// <summary>
    /// 총의 조정간을 담당하는 함수, 공격을 할 수 있는지 없는지
    /// </summary>
    /// <param name="_shooting"></param>
    /// <returns></returns>
    public bool ShootingOn(bool _shooting)
    {
        return shootingOn = _shooting;
    }

    public bool PickUpImageOff(bool _ImageOff)
    {
        return imageOff = _ImageOff;
    }
}
