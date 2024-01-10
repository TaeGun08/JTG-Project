using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class SaveObject : MonoBehaviour
{
    public class SavedObjectData
    {
        public float playerDamage;
        public int playerArmor;
        public int playerHp;
        public int playerCurHp;
        public float playerCritical;
        public float playerCriDamage;
    }

    private SavedObjectData savedObj = new SavedObjectData();

    private Player player;

    [SerializeField] private bool dataReset = false;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        playerObjectResetData();
    }

    /// <summary>
    /// 플레이어가 저장데이터를 삭제했을 때 받아올 초기데이터
    /// </summary>
    private void playerObjectResetData()
    {
        if (dataReset == true)
        {
            savedObj.playerDamage = 0;
            savedObj.playerArmor = 0;
            savedObj.playerHp = 50;
            savedObj.playerCurHp = 50;
            savedObj.playerCritical = 0;
            savedObj.playerCriDamage = 2;

            saveData();

            string savedData = PlayerPrefs.GetString("playerDataSave");

            savedObj = JsonConvert.DeserializeObject<SavedObjectData>(savedData);

            player.PlayerSavedData(savedObj);

            dataReset = false;
        }
    }

    /// <summary>
    /// 플레이어가 씬을 넘어갈 때마다 저장되는 데이터
    /// </summary>
    /// <param name="_playerData"></param>
    public void PlayerObjectSaveData(PlayerData _playerData)//타 스크립트에서 저장하면 된다고 알려주는 함수
    {
        if (dataReset == false)
        {
            savedObj.playerDamage = _playerData.playerDamage;
            savedObj.playerArmor = _playerData.playerArmor;
            savedObj.playerHp = _playerData.playerHp;
            savedObj.playerCurHp = _playerData.playerCurHp;
            savedObj.playerCritical = _playerData.playerCritical;
            savedObj.playerCriDamage = _playerData.playerCriDamage;
        }

        saveData();
    }

    private void saveData()//Json으로 저장
    {
        string savedData = JsonConvert.SerializeObject(savedObj);
        PlayerPrefs.SetString("playerDataSave", savedData);
    }

    /// <summary>
    /// 세이브 데이터를 불러와 플레이어 데이터에 덮어 씌우는 함수
    /// </summary>
    public void PlayerObjectDataLoad()
    {
        string savedData = PlayerPrefs.GetString("playerDataSave");

        if (savedData == string.Empty)
        {
            return;
        }

        savedObj = JsonConvert.DeserializeObject<SavedObjectData>(savedData);

        player.PlayerSavedData(savedObj);
    }

    public void PlayerDataResetOn(bool _reset)
    {
        dataReset = _reset;
    }
}
