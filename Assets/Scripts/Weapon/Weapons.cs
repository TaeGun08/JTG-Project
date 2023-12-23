using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapons : MonoBehaviour
{
    public enum WeaponType
    {
        weaponA,
        weaponB, 
        weaponC, 
        weaponD,
    }

    [SerializeField] WeaponType weaponType;

    //무기에 가져올 매니저
    private GameManager gameManager; //게임매니저
    private KeyManager keyManager; //키매니저

    //private int weaponNum = 0;

    [Header("공격 설정")]
    [SerializeField, Tooltip("공격 딜레이")] private float shootDelay = 0.5f;
    private float shootTimer;

    [Header("총알 설정")]
    [SerializeField, Tooltip("총알 프리팹")] private GameObject bullet;
    [SerializeField, Tooltip("총알이 나가는 위치")] private Transform bulletPos;
    [SerializeField, Tooltip("총알 프리팹들이 위치할 오브젝트")] private Transform bulletObj;
    [Space]
    [SerializeField, Tooltip("최대 탄창")] private int maxMagazine; //탄창에 들어가는 최대 총알 수
    [SerializeField, Tooltip("현재 탄창")] private int curMagazine; //현재 탕창에 보유중인 총알 수
    [SerializeField, Tooltip("재장전 시간")] private float reroadingTime; //재장전을 위한 시간
    private float reroadingTimer;
    private bool reroading = false;
    [Space]
    [SerializeField, Tooltip("재장전 UI 오브젝트")] private GameObject reroadingUI; //재장전을 시각화 시켜주기 위한 UI 오브젝트
    [SerializeField, Tooltip("재장전 UI 슬라이더")] private Slider reroadingSlider;
    private float curReroadingSlider;

    private void Start()
    {
        gameManager = GameManager.Instance; //게임 매니저를 가져와 gameManager에 담아 줌
        keyManager = KeyManager.instance; //키매니저를 가져와 keyManager 담아 줌

        curReroadingSlider = reroadingSlider.value;
    }

    private void Update()
    {
        reroadingWeapon();
        shootWeapon();
    }

    /// <summary>
    /// 총의 재장전을 담당하는 함수
    /// </summary>
    private void reroadingWeapon()
    {
        if (reroading == true) //재장전이 true면 타이머를 작동시고 UI를 활성화함
        {
            reroadingTimer -= Time.deltaTime;
            reroadingSlider.value = reroadingTimer;
            reroadingUI.SetActive(true);
            if (reroadingTimer < 0)
            {
                reroadingUI.SetActive(false);
                reroadingSlider.value = 1.0f;
                reroadingTimer = reroadingTime;
                curMagazine = maxMagazine;
                reroading = false;
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
            reroading = true;
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

        if (Input.GetKeyDown(keyManager.PlayerAttackKey()) && shootTimer == 0.0f && reroading == false) //마우스를 누를 때 마다 발사
        {
            shootBullet();
            curMagazine--;
            shootTimer = shootDelay;
        }
        else if (Input.GetKey(keyManager.PlayerAttackKey()) && shootTimer == 0.0f && reroading == false) //마우스를 누르고 있으면 발사
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
        Instantiate(bullet, bulletPos.position, bulletPos.rotation, bulletObj);
    }
}
