using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("플레이어의 능력치를 올릴 버튼")]
    [SerializeField] private Button damageUp; //경험치 포인트를 이용하여 공격력을 상승 시키는 버튼
    [SerializeField] private Button armorUp; //경험치 포인트를 이용하여 방어력을 상승 시키는 버튼
    [SerializeField] private Button hpUp; //경험치 포인트를 이용하여 체력을 상승 시키는 버튼
    [SerializeField] private Button criticalUp; //경험치 포인트를 이용하여 치명타 확률을 상승 시키는 버튼

    [Header("플레이어의 정보를 넣어줄 UI들")]
    [SerializeField] private TMP_Text playerLevel;
    [SerializeField] private TMP_Text playerExp;
    [SerializeField] private TMP_Text playerLevelPoint;
    [SerializeField] private TMP_Text playerDamage;
    [SerializeField] private TMP_Text playerArmor;
    [SerializeField] private TMP_Text playerCurHp;
    [SerializeField] private TMP_Text playerCritical;
    [SerializeField] private TMP_Text playerCriDamage;

    private void Awake()
    {
        damageUp.onClick.AddListener(() => 
        {
            player.playerPointDamageUp();
        });

        armorUp.onClick.AddListener(() =>
        {
            player.playerPointArmorUp();
        });

        hpUp.onClick.AddListener(() =>
        {
            player.playerPointHpUp();
        });

        criticalUp.onClick.AddListener(() =>
        {
            player.playerPointCriticalUp();
        });
    }

    private void Update()
    {
        playerStatus();
    }

    private void playerStatus()
    {
        if (player.PlayerStatusOpen() == true)
        {
            playerLevel.text = $"LV: {player.PlayerLevelReturn()}";
            playerExp.text = $"Exp: {player.PlayerExpReturn()} / {player.PlayerMaxExpReturn()}";
            playerLevelPoint.text = $"포인트: {player.PlayerLevelPointReturn()}";
            playerDamage.text = $"공격력: {player.PlayerDamageReturn()}";
            if (player.PlayerBuffDamageReturn() <= 0)
            {
                playerDamage.text = $"공격력: {player.PlayerDamageReturn()}";
            }
            else if (player.PlayerBuffDamageReturn() > 0)
            {
                playerDamage.text = $"공격력: {player.PlayerDamageReturn()} <color=red>+{player.PlayerBuffDamageReturn()}</color>"; //리치텍스트 기억하기
            }
            playerArmor.text = $"방어력: {player.PlayerArmorReturn()}";
            playerCurHp.text = $"체력: {player.PlayerCrHpReturn()} / {player.PlayerMaxHpReturn()}";
            playerCritical.text = $"치명타확률: {player.PlayerCriticalReturn().ToString("F1")}%";
            playerCriDamage.text = $"치명타데미지: {(int)player.PlayerCriDamageReturn() * 100}%";
        }
    }
}
