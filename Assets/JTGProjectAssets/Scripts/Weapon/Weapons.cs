using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Weapons : MonoBehaviour
{
    public enum WeaponType
    {
        weaponTypeA = 1,
        weaponTypeB,
        weaponTypeC,
        weaponTypeD,
    }

    [SerializeField] private WeaponType weaponType;

    private BoxCollider2D weaponBoxColl2D;

    //���⿡ ������ �Ŵ���
    private GameManager gameManager; //���ӸŴ���
    private KeyManager keyManager; //Ű�Ŵ���

    private TrashPreFab trashPreFab;
    private ItemPickUp itemPickUp;
    private WeaponSkill weaponSkill;
    private SpriteRenderer weaponRen;

    private Vector3 moveVec;
    private float moveSpeed;
    private bool weaponGravityOn = false;
    private bool gravityOff = false;

    [Header("���� ����")]
    [SerializeField, Tooltip("���� ������")] private float shootDelay = 0.5f;
    private float shootTimer;
    [SerializeField, Tooltip("������")] private bool shootingOn = false;
    [SerializeField, Tooltip("���� ���ݷ�")] private float weaponDamage;
    [SerializeField, Tooltip("���� ���� ���ݷ�")] private float weaponCurDamage;
    private float passiveDmgUp;
    private bool useShoot = false;
    private float buffUpDamage;
    private float playerCriticalPcent;
    private float playerCriticalDamage;
    private bool hitCriticalCheck = false;

    [Header("�Ѿ� ����")]
    [SerializeField, Tooltip("�Ѿ� ������")] private GameObject bullet;
    [SerializeField, Tooltip("�Ѿ��� ������ ��ġ")] private Transform bulletPos;
    [Space]
    [SerializeField, Tooltip("�ִ� źâ")] private int maxMagazine; //źâ�� ���� �ִ� �Ѿ� ��
    [SerializeField, Tooltip("���� źâ")] private int curMagazine; //���� ��â�� �������� �Ѿ� ��
    [SerializeField, Tooltip("������ �ð�")] private float reloadingTime; //�������� ���� �ð�
    private float reloadingTimer;
    private bool reloading = false;
    private float curReloadingSlider;
    private float autoReloadingTimer;

    [Header("�ݱ� Ű �̹���")]
    [SerializeField] private GameObject pickUpKeyImage;
    private bool imageOff = false;

    [Header("źâ UI")]
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
        gameManager = GameManager.Instance; //���� �Ŵ����� ������ gameManager�� ��� ��
        keyManager = KeyManager.instance; //Ű�Ŵ����� ������ keyManager ��� ��

        trashPreFab = TrashPreFab.Instance;

        curReloadingSlider = gameManager.ReloadingUI().value;

        pickUpKeyImage.SetActive(false);
    }

    private void Update()
    {
        if (gameManager.GetGamePause() == true)
        {
            return;
        }

        colliderCheck();
        weaponMove();
        autoReloading();
        skillDamageCheck();
        weaponCriticalCheck();

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
    /// �ڽ� �ݶ��̴��� ����� ���� �ݶ��̴� ������Ʈ�� Ȯ��
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
    /// ���Ⱑ �߷��� �ޱ����� �۵��ϴ� �Լ�
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

            moveVec.y = 2;
            transform.position += moveVec * Time.deltaTime;
            moveVec.y = gameManager.GravityScale();
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
    /// ���⸦ 3�� �̻� �տ� ��� ���� ������ �ڵ����� �������� �� �Ѿ��� ���� ä����
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
    /// ���� �������� ����ϴ� �Լ�
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

        if (reloading == true) //�������� true�� Ÿ�̸Ӹ� �۵��ð� UI�� Ȱ��ȭ��
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
    /// ���� �߻縦 ����ϴ� �Լ�
    /// </summary>
    private void shootWeapon()
    {
        if (curMagazine <= 0) //���� �Ѿ��� ���ٸ� �������� true�� �ٲٰ� �߻縦 �� �ϰ� ��
        {
            reloading = true;
            return;
        }

        if (shootTimer != 0.0f) //�߻� ������
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer < 0)
            {
                shootTimer = 0.0f;
            }
        }

        if ((Input.GetKeyDown(keyManager.PlayerAttackKey()) && shootTimer == 0.0f && reloading == false) //���콺�� ���� �� ���� �߻�
            || (Input.GetKey(keyManager.PlayerAttackKey()) && shootTimer == 0.0f && reloading == false)) //���콺�� ������ ������ �߻�
        {
            useShoot = false;
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
    /// �Ѿ� ������ ����ϴ� �Լ�
    /// </summary>
    /// <param name="_rot"></param>
    private void shootBullet()
    {
        if (weaponType.ToString() == "weaponTypeA" || weaponType.ToString() == "weaponTypeC")
        {
            GameObject bulletObj = Instantiate(bullet, bulletPos.position, bulletPos.rotation, trashPreFab.transform);
            Bullet bulletSc = bulletObj.GetComponent<Bullet>();
            if (weaponSkill.SkillAOn() == false)
            {
                bulletSc.BulletDamage(weaponCurDamage, 0, false, hitCriticalCheck);
            }
            else if (weaponSkill.SkillAOn() == true)
            {
                bulletSc.BulletDamage(weaponCurDamage, 1.2f, true, hitCriticalCheck);
            }
        }
        else if (weaponType.ToString() == "weaponTypeB")
        {
            GameObject bulletObjA = Instantiate(bullet, bulletPos.position, bulletPos.rotation, trashPreFab.transform);
            GameObject bulletObjB = Instantiate(bullet, bulletPos.position, bulletPos.rotation * Quaternion.Euler(new Vector3(0, 0, 15)), trashPreFab.transform);
            GameObject bulletObjC = Instantiate(bullet, bulletPos.position, bulletPos.rotation * Quaternion.Euler(new Vector3(0, 0, -15)), trashPreFab.transform);
            Bullet bulletScA = bulletObjA.GetComponent<Bullet>();
            Bullet bulletScB = bulletObjB.GetComponent<Bullet>();
            Bullet bulletScC = bulletObjC.GetComponent<Bullet>();
            bulletScA.BulletDamage(weaponCurDamage, 0, false, hitCriticalCheck);
            bulletScB.BulletDamage(weaponCurDamage, 0, false, hitCriticalCheck);
            bulletScC.BulletDamage(weaponCurDamage, 0, false, hitCriticalCheck);
        }
    }

    private void weaponCriticalCheck()
    {
        if (useShoot == false)
        {
            float ciritical = Random.Range(0.0f, 100.0f);
            if (ciritical <= playerCriticalPcent)
            {
                hitCriticalCheck = true;
                weaponCurDamage = ((weaponDamage + passiveDmgUp) * playerCriticalDamage) + buffUpDamage;
                useShoot = true;
            }
            else if (ciritical > playerCriticalPcent)
            {
                hitCriticalCheck = false;
                weaponCurDamage = (weaponDamage + passiveDmgUp) + buffUpDamage;
                useShoot = true;
            }
        }
    }

    private void skillDamageCheck()
    {
        weaponSkill.GetSkillDamage(weaponCurDamage);
    }

    /// <summary>
    /// ���� �������� ����ϴ� �Լ�, ������ �� �� �ִ��� ������
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
    /// �ҷ� �������� ���� �����͸� �޾ƿ��� ���� �Լ�
    /// </summary>
    /// <returns></returns>
    public GameObject CurBullet()
    {
        return bullet;
    }

    /// <summary>
    /// �ٸ� ��ũ��Ʈ���� �Ѿ� ������ �ϱ� ���� ����ϴ� �Լ�
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

    /// <summary>
    /// ������ ���� ���ݷ� ����� �����ִ� �Լ�
    /// </summary>
    /// <param name="_buffDamage"></param>
    /// <param name="_damageUp"></param>
    public void BuffDamage(float _buffDamage)
    {
        buffUpDamage = _buffDamage;
    }

    /// <summary>
    /// �нú�� ���� ���ݷ� ����� �����ִ� �Լ�
    /// </summary>
    public void PassiveDamageUp(float _passiveUp)
    {
        passiveDmgUp = _passiveUp;
    }

    public void WeaponUseShooting(bool _useShoot)
    {
        useShoot = _useShoot;
    }

    public bool WeaponUseShootingCheck()
    {
        return useShoot;
    }

    public void GetPlayerCriticalPcent(float _critical, float _criDamage)
    {
        playerCriticalPcent = _critical;
        playerCriticalDamage = _criDamage;
    }

    public void ReturnDamage()
    {
         weaponCurDamage = weaponDamage;
    }

    public bool HitCriticalCheck()
    {
        return hitCriticalCheck;
    }

    public WeaponType GetWeaponType()
    {
        return weaponType;
    }
}
