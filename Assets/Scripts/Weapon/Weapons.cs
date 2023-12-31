using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Weapons : MonoBehaviour
{
    public enum WeaponType
    {
        weaponTypeA,
        weaponTypeB,
        weaponTypeC,
        weaponTypeD,
    }

    [SerializeField] private WeaponType weaponType;

    private BoxCollider2D weaponBoxColl2D;

    //무기에 가져올 매니저
    private GameManager gameManager; //게임매니저
    private KeyManager keyManager; //키매니저

    private TrashPreFab trashPreFab;
    private ItemPickUp itemPickUp;
    private WeaponSkill weaponSkill;
    private SpriteRenderer weaponRen;

    private Vector3 moveVec;
    private float moveSpeed;
    private bool weaponGravityOn = false;
    private bool gravityOff = false;

    [Header("공격 설정")]
    [SerializeField, Tooltip("공격 딜레이")] private float shootDelay = 0.5f;
    private float shootTimer;
    [SerializeField, Tooltip("조정간")] private bool shootingOn = false;
    [SerializeField, Tooltip("총의 공격력")] private float weaponDamage;
    [SerializeField, Tooltip("총의 현재 공격력")] private float weaponCurDamage;

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
    private float autoReloadingTimer;

    [Header("줍기 키 이미지")]
    [SerializeField] private GameObject pickUpKeyImage;
    private bool imageOff = false;

    [Header("탄창 UI")]
    [SerializeField] TMP_Text magazineText;

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

    private void triggerCheck(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            weaponGravityOn = false;
        }
    }

    private void Awake()
    {
        itemPickUp = GetComponent<ItemPickUp>();
        weaponBoxColl2D = GetComponent<BoxCollider2D>();
        weaponSkill = GetComponent<WeaponSkill>();
        weaponRen = GetComponent<SpriteRenderer>();

        weaponCurDamage = weaponDamage;

        autoReloadingTimer = 3.0f;

        weaponGravityOn = true;
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
        colliderCheck();
        weaponMove();
        autoReloading();

        magazineText.text = $"{curMagazine} / {maxMagazine}";

        if (shootingOn == false || itemPickUp.GetItemType().ToString() != "Weapon" || weaponSkill.UseSkill() == true)
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
    /// 박스 콜라이더를 만들어 닿은 콜라이더 오브젝트를 확인
    /// </summary>
    private void colliderCheck()
    {
        Collider2D groundColl = Physics2D.OverlapBox(weaponBoxColl2D.bounds.center,
                weaponBoxColl2D.bounds.size, 0f, LayerMask.GetMask("Ground"));

        if (groundColl != null)
        {
            triggerCheck(groundColl);
        }
        else if (groundColl == null)
        {
            weaponGravityOn = true;
        }
    }

    /// <summary>
    /// 무기가 중력을 받기위해 작동하는 함수
    /// </summary>
    private void weaponMove()
    {
        if (gravityOff == true)
        {
            return;
        }

        if (weaponGravityOn == true)
        {
            if (moveSpeed > 5)
            {
                moveSpeed = 5;
            }

            moveVec.y = 7;
            transform.position += moveVec * Time.deltaTime;
            moveVec.y = gameManager.gravityScale();
            moveSpeed += 0.01f;
            transform.position -= moveVec * moveSpeed * Time.deltaTime;
        }
        else if (weaponGravityOn == false)
        {
            moveVec.y = 0;
            moveSpeed = 0;
        }
    }

    /// <summary>
    /// 무기를 3초 이상 손에 들고 있지 않으면 자동으로 재장전을 해 총알을 전부 채워줌
    /// </summary>
    private void autoReloading()
    {
        if (weaponRen.enabled == false && curMagazine != maxMagazine)
        {
            autoReloadingTimer -= Time.deltaTime;
            if (autoReloadingTimer < 0.0f)
            {
                autoReloadingTimer = 3.0f;
                gameManager.ReloadingObj().SetActive(false);
                gameManager.ReloadingUI().value = 1.0f;
                reloadingTimer = reloadingTime;
                curMagazine = maxMagazine;
                reloading = false;
            }
        }
    }

    /// <summary>
    /// 총의 재장전을 담당하는 함수
    /// </summary>
    private void reloadingWeapon()
    {
        if (weaponSkill.SkillAOn() == true)
        {
            gameManager.ReloadingObj().SetActive(false);
            gameManager.ReloadingUI().value = 1.0f;
            reloadingTimer = reloadingTime;
            curMagazine = maxMagazine;
            reloading = false;
            return;
        }

        if (Input.GetKeyDown(keyManager.ReloadingKey()) && curMagazine != maxMagazine && reloading == false)
        {
            curMagazine = 0;
        }

        if (reloading == true) //재장전이 true면 타이머를 작동시고 UI를 활성화함
        {
            reloadingTimer -= Time.deltaTime;
            gameManager.ReloadingUI().value = reloadingTimer / reloadingTime;
            gameManager.ReloadingObj().SetActive(true);
            if (reloadingTimer < 0)
            {
                gameManager.ReloadingObj().SetActive(false);
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

        if ((Input.GetKeyDown(keyManager.PlayerAttackKey()) && shootTimer == 0.0f && reloading == false) //마우스를 누를 때 마다 발사
            || (Input.GetKey(keyManager.PlayerAttackKey()) && shootTimer == 0.0f && reloading == false)) //마우스를 누르고 있으면 발사
        {
            shootBullet();
            shootTimer = shootDelay;

            if (weaponSkill.SkillAOn() == true)
            {
                return;
            }

            curMagazine--;
        }
    }

    /// <summary>
    /// 총알 생성을 담당하는 함수
    /// </summary>
    /// <param name="_rot"></param>
    private void shootBullet(float _rot = 0.0f)
    {
        if (weaponType.ToString() == "weaponTypeA")
        {
            GameObject bulletObjA = Instantiate(bullet, bulletPos.position, bulletPos.rotation, trashPreFab.transform);
            Bullet bulletScA = bulletObjA.GetComponent<Bullet>();
            if (weaponSkill.SkillAOn() == false)
            {
                bulletScA.BulletDamage(weaponCurDamage, 0, false);
            }
            else if (weaponSkill.SkillAOn() == true)
            {
                bulletScA.BulletDamage(weaponCurDamage, 1.2f, true);
            }
        }
        else if (weaponType.ToString() == "weaponTypeB")
        {
            GameObject bulletObjA = Instantiate(bullet, bulletPos.position, bulletPos.rotation, trashPreFab.transform);
            GameObject bulletObjB = Instantiate(bullet, bulletPos.position, bulletPos.rotation * Quaternion.Euler(new Vector3(0, 0, 5)), trashPreFab.transform);
            GameObject bulletObjC = Instantiate(bullet, bulletPos.position, bulletPos.rotation * Quaternion.Euler(new Vector3(0, 0, -5)), trashPreFab.transform);
            Bullet bulletScA = bulletObjA.GetComponent<Bullet>();
            Bullet bulletScB = bulletObjB.GetComponent<Bullet>();
            Bullet bulletScC = bulletObjC.GetComponent<Bullet>();
            bulletScA.BulletDamage(weaponCurDamage, 0, false);
            bulletScB.BulletDamage(weaponCurDamage, 0, false);
            bulletScC.BulletDamage(weaponCurDamage, 0, false);
        }
    }

    /// <summary>
    /// 총의 조정간을 담당하는 함수, 공격을 할 수 있는지 없는지
    /// </summary>
    /// <param name="_shooting"></param>
    /// <returns></returns>
    public void ShootingOn(bool _shooting)
    {
        shootingOn = _shooting;
    }

    public bool ShootingOnCheck()
    {
        return shootingOn;
    }

    public void PickUpImageOff(bool _ImageOff)
    {
        imageOff = _ImageOff;
    }

    /// <summary>
    /// 불렛 프리팹의 원본 데이터를 받아오기 위한 함수
    /// </summary>
    /// <returns></returns>
    public GameObject CurBullet()
    {
        return bullet;
    }

    /// <summary>
    /// 다른 스크립트에서 총알 변경을 하기 위해 사용하는 함수
    /// </summary>
    /// <param name="_bullet"></param>
    public void BulletChange(GameObject _bullet)
    {
        bullet = _bullet;
    }

    public void WeaponGravityOff(bool _gravityOff)
    {
        gravityOff = _gravityOff;
    }

    public float WeaponCurDamage()
    {
        return weaponCurDamage;
    }

    /// <summary>
    /// 버프로 인한 공격력 상승을 시켜주는 함수
    /// </summary>
    /// <param name="_buffDamage"></param>
    /// <param name="_damageUp"></param>
    public void BuffDamage(float _buffDamage, bool _damageUp)
    {
        if (_buffDamage >= weaponCurDamage && _damageUp == true)
        {
            weaponCurDamage += _buffDamage;
        }
        else if (weaponDamage <= weaponCurDamage && _damageUp == false)
        {
            weaponCurDamage = weaponDamage;
        }
    }
}
