using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SaveObject;

public class Status : MonoBehaviour
{
    public class StatusData
    {
        public bool lv2click;
        public bool lv4click;
        public bool lv6click;
        public bool lv8click;
        public bool lv10click;
        public bool damage;
        public bool armor;
        public bool health;
    }

    private StatusData statusData = new StatusData();
    private bool dataSave = false;

    [SerializeField] private Player player;
    [SerializeField] private SaveObject saveObject;

    [Header("플레이어의 능력치를 올릴 버튼")]
    [SerializeField] private Button damageUp; //경험치 포인트를 이용하여 공격력을 상승 시키는 버튼
    [SerializeField] private Button armorUp; //경험치 포인트를 이용하여 방어력을 상승 시키는 버튼
    [SerializeField] private Button hpUp; //경험치 포인트를 이용하여 체력을 상승 시키는 버튼
    [SerializeField] private Button criticalUp; //경험치 포인트를 이용하여 치명타 확률을 상승 시키는 버튼

    [Header("플레이어의 패시브 스킬을 찍을 수 있는 버튼")]
    [SerializeField] private List<Button> lvpassiveSkills; //레벨 2부터 10까지 특정 레벨에 도달해야지만 찍을 수 있는 스킬트리
    private bool lv2click = false;
    private bool lv4click = false;
    private bool lv6click = false;
    private bool lv8click = false;
    private bool lv10click = false;
    private bool damage = false;
    private bool armor = false;
    private bool health = false;

    [Header("플레이어의 정보를 넣어줄 UI들")]
    [SerializeField] private List<TMP_Text> playerStatusText;
    [SerializeField] private List<Image> items;
    [SerializeField] private List<GameObject> itemsTextObj;

    private void Awake()
    {
        statusUpButton();
        passiveUpButton();
    }

    private void Start()
    {
        saveObject.PlayerObjectDataLoad();
    }

    private void Update()
    {
        setPlayerData();
        playerStatus();
        levelCheck();
        itemImage();
    }

    /// <summary>
    /// 플레이어의 스테이터스 데이터를 세이브하기 위한 함수
    /// </summary>
    private void setPlayerData()
    {
        if (dataSave == true)
        {
            statusData.lv2click = lv2click;
            statusData.lv4click = lv4click;
            statusData.lv6click = lv6click;
            statusData.lv8click = lv8click;
            statusData.lv10click = lv10click;
            statusData.damage = damage;
            statusData.armor = armor;
            statusData.health = health;

            saveObject.PlayerStatusSaveData(statusData);

            dataSave = false;
        }
    }

    /// <summary>
    /// 레벨 포인트를 이용해 능력치를 올리는 버튼
    /// </summary>
    private void statusUpButton()
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

    /// <summary>
    /// 특정 레벨에 도달하면 찍을 수 있는 패시브 스킬 버튼
    /// </summary>
    private void passiveUpButton()
    {
        lvpassiveSkills[0].onClick.AddListener(() =>
        {
            player.PlayerStatusDamage(2);
            player.PlayerStatusArmor(1);
            player.PlayerStatusHp(10);
            lv2click = true;
            lv4click = true;
            lvpassiveSkills[0].interactable = false;
        });

        lvpassiveSkills[1].onClick.AddListener(() =>
        {
            player.PlayerStatusDamage(5);
            damage = true;
            lv4click = false;
            lv6click = true;
            lvpassiveSkills[1].interactable = false;
        });

        lvpassiveSkills[2].onClick.AddListener(() =>
        {
            player.PlayerStatusArmor(2);
            armor = true;
            lv4click = false;
            lv6click = true;
            lvpassiveSkills[2].interactable = false;
        });

        lvpassiveSkills[3].onClick.AddListener(() =>
        {
            player.PlayerStatusHp(30);
            health = true;
            lv4click = false;
            lv6click = true;
            lvpassiveSkills[3].interactable = false;
        });

        lvpassiveSkills[4].onClick.AddListener(() =>
        {
            player.PlayerStatusCritical(10, 0);
            lv6click = false;
            lv8click = true;
            lvpassiveSkills[4].interactable = false;
        });

        lvpassiveSkills[5].onClick.AddListener(() =>
        {
            player.PlayerStatusArmor(2);
            lv6click = false;
            lv8click = true;
            lvpassiveSkills[5].interactable = false;
        });

        lvpassiveSkills[6].onClick.AddListener(() =>
        {
            player.PlayerStatusHp(30);
            lv6click = false;
            lv8click = true;
            lvpassiveSkills[6].interactable = false;
        });

        lvpassiveSkills[7].onClick.AddListener(() =>
        {
            player.PlayerStatusCritical(0, 0.2f);
            lv8click = false;
            lv10click = true;
            lvpassiveSkills[7].interactable = false;
        });

        lvpassiveSkills[8].onClick.AddListener(() =>
        {
            player.PlayerStatusArmor(2);
            lv8click = false;
            lv10click = true;
            lvpassiveSkills[8].interactable = false;
        });

        lvpassiveSkills[9].onClick.AddListener(() =>
        {
            player.PlayerStatusHp(30);
            lv8click = false;
            lv10click = true;
            lvpassiveSkills[9].interactable = false;
        });

        lvpassiveSkills[10].onClick.AddListener(() =>
        {
            player.PlayerStatusDamage(5);
            player.PlayerStatusCritical(10, 0.3f);
            lv10click = false;
            lvpassiveSkills[10].interactable = false;
        });

        lvpassiveSkills[11].onClick.AddListener(() =>
        {
            player.PlayerStatusArmor(2);
            lv10click = false;
            lvpassiveSkills[11].interactable = false;
        });

        lvpassiveSkills[12].onClick.AddListener(() =>
        {
            player.PlayerStatusHp(30);
            lv10click = false;
            lvpassiveSkills[12].interactable = false;
        });
    }

    /// <summary>
    /// 스킬트리의 중복 처리를 방지하기 위한 함수
    /// </summary>
    private void levelCheck()
    {
        if (player.PlayerLevelReturn() >= 2 && lv2click == false)
        {
            lvpassiveSkills[0].interactable = true;
        }
        else if (player.PlayerLevelReturn() < 2 || lv2click == true)
        {
            lvpassiveSkills[0].interactable = false;
        }

        if (player.PlayerLevelReturn() >= 4 && lv4click == true)
        {
            lvpassiveSkills[1].interactable = true;
            lvpassiveSkills[2].interactable = true;
            lvpassiveSkills[3].interactable = true;
        }
        else if (player.PlayerLevelReturn() < 4 || lv4click == false)
        {
            lvpassiveSkills[1].interactable = false;
            lvpassiveSkills[2].interactable = false;
            lvpassiveSkills[3].interactable = false;
        }

        if (player.PlayerLevelReturn() >= 6 && lv6click == true)
        {
            if (damage == true)
            {
                lvpassiveSkills[4].interactable = true;
                lvpassiveSkills[5].interactable = false;
                lvpassiveSkills[6].interactable = false;
            }
            else if (armor == true)
            {
                lvpassiveSkills[4].interactable = false;
                lvpassiveSkills[5].interactable = true;
                lvpassiveSkills[6].interactable = false;
            }
            else if (health == true)
            {
                lvpassiveSkills[4].interactable = false;
                lvpassiveSkills[5].interactable = false;
                lvpassiveSkills[6].interactable = true;
            }
        }
        else if (player.PlayerLevelReturn() < 6 || lv6click == false)
        {
            lvpassiveSkills[4].interactable = false;
            lvpassiveSkills[5].interactable = false;
            lvpassiveSkills[6].interactable = false;
        }

        if (player.PlayerLevelReturn() >= 8 && lv8click == true)
        {
            if (damage == true)
            {
                lvpassiveSkills[7].interactable = true;
                lvpassiveSkills[8].interactable = false;
                lvpassiveSkills[9].interactable = false;
            }
            else if (armor == true)
            {
                lvpassiveSkills[7].interactable = false;
                lvpassiveSkills[8].interactable = true;
                lvpassiveSkills[9].interactable = false;
            }
            else if (health == true)
            {
                lvpassiveSkills[7].interactable = false;
                lvpassiveSkills[8].interactable = false;
                lvpassiveSkills[9].interactable = true;
            }
        }
        else if (player.PlayerLevelReturn() < 8 || lv8click == false)
        {
            lvpassiveSkills[7].interactable = false;
            lvpassiveSkills[8].interactable = false;
            lvpassiveSkills[9].interactable = false;
        }

        if (player.PlayerLevelReturn() >= 10 && lv10click == true)
        {
            if (damage == true)
            {
                lvpassiveSkills[10].interactable = true;
                lvpassiveSkills[11].interactable = false;
                lvpassiveSkills[12].interactable = false;
            }
            else if (armor == true)
            {
                lvpassiveSkills[10].interactable = false;
                lvpassiveSkills[11].interactable = true;
                lvpassiveSkills[12].interactable = false;
            }
            else if (health == true)
            {
                lvpassiveSkills[10].interactable = false;
                lvpassiveSkills[11].interactable = false;
                lvpassiveSkills[12].interactable = true;
            }
        }
        else if (player.PlayerLevelReturn() < 10 || lv10click == false)
        {
            lvpassiveSkills[10].interactable = false;
            lvpassiveSkills[11].interactable = false;
            lvpassiveSkills[12].interactable = false;
        }
    }

    /// <summary>
    /// 플레이어의 정보를 시각적으로 확인 시켜주기 위한 함수
    /// </summary>
    private void playerStatus()
    {
        if (player.PlayerStatusOpen() == true)
        {
            playerStatusText[0].text = $"LV: {player.PlayerLevelReturn()}";
            playerStatusText[1].text = $"Exp: {player.PlayerExpReturn()} / {player.PlayerMaxExpReturn()}";
            playerStatusText[2].text = $"포인트: {player.PlayerLevelPointReturn()}";
            playerStatusText[3].text = $"공격력: {player.PlayerDamageReturn()}";
            if (player.PlayerBuffDamageReturn() <= 0)
            {
                playerStatusText[3].text = $"공격력: {player.PlayerDamageReturn()}";
            }
            else if (player.PlayerBuffDamageReturn() > 0)
            {
                playerStatusText[3].text = $"공격력: {player.PlayerDamageReturn()} <color=red>+{player.PlayerBuffDamageReturn()}</color>"; //리치텍스트 기억하기
            }
            playerStatusText[4].text = $"방어력: {player.PlayerArmorReturn()}";
            playerStatusText[5].text = $"체력: {player.PlayerCrHpReturn()} / {player.PlayerMaxHpReturn()}";
            playerStatusText[6].text = $"치명타확률: {player.PlayerCriticalReturn().ToString("F1")}%";
            playerStatusText[7].text = $"치명타데미지: {(int)player.PlayerCriDamageReturn() * 100}%";
        }
    }

    /// <summary>
    /// 플레이어의 아이템 배열에 있는 오브젝트의 이미지를 가져와 스테이터스에 보여줌
    /// </summary>
    private void itemImage()
    {
        if (player.GetWeaponPrefabs().Count == 1)
        {
            SpriteRenderer weaponARen = player.GetWeaponPrefabs()[0].GetComponent<SpriteRenderer>();
            items[0].enabled = true;
            items[1].enabled = false;
            itemsTextObj[0].SetActive(false);
            itemsTextObj[1].SetActive(true);
            items[0].sprite = weaponARen.sprite;
        }
        else if (player.GetWeaponPrefabs().Count == 2)
        {
            SpriteRenderer weaponBRen = player.GetWeaponPrefabs()[1].GetComponent<SpriteRenderer>();
            items[1].enabled = true;
            itemsTextObj[1].SetActive(false);
            items[1].sprite = weaponBRen.sprite;
        }
        else if (player.GetWeaponPrefabs().Count == 0)
        {
            items[0].enabled = false;
            items[1].enabled = false;
            itemsTextObj[0].SetActive(true);
            itemsTextObj[1].SetActive(true);
        }

        if (player.GetPetPreFabs().Count == 1)
        {
            SpriteRenderer petRen = player.GetPetPreFabs()[0].GetComponent<SpriteRenderer>();
            items[2].enabled = true;
            itemsTextObj[2].SetActive(false);
            items[2].sprite = petRen.sprite;
        }
        else if (player.GetPetPreFabs().Count == 0)
        {
            items[2].enabled = false;
            itemsTextObj[2].SetActive(true);
        }
    }

    public void PlayerStatusSavedData(SaveStatusData _saveStatusData)
    {
        lv2click = _saveStatusData.lv2click;
        lv4click = _saveStatusData.lv4click;
        lv6click = _saveStatusData.lv6click;
        lv8click = _saveStatusData.lv8click;
        lv10click = _saveStatusData.lv10click;
        damage = _saveStatusData.damage;
        armor = _saveStatusData.armor;
        health = _saveStatusData.health;
    }

    public void PlayerStatusSaveOn(bool _save)
    {
        dataSave = _save;
    }
}
