using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("플레이어 UI 관련 설정")]
    [SerializeField, Tooltip("플레이어 스킬 쿨 판넬")] private GameObject playerSkillCool;
    [SerializeField, Tooltip("플레이어 스킬 쿨 텍스트")] private GameObject playerSkillCoolText;
    [SerializeField, Tooltip("플레이어 대쉬 쿨 판넬")] private GameObject playerDashCool;
    [SerializeField, Tooltip("플레이어 스킬 쿨 텍스트")] private GameObject playerDashCoolText;
    [SerializeField, Tooltip("플레이어 체력 슬라이더")] private Slider playerHp;
    [SerializeField, Tooltip("플레이어 체력 텍스트")] private TMP_Text playerHpText;
    [SerializeField, Tooltip("옵션창")] private GameObject option;

    /// <summary>
    /// 플레이어 스킬과 관련된 오브젝트를 활성화 또는 비활성화를 담당
    /// </summary>
    /// <param name="_objOn"></param>
    public void PlayerSkiilCool(bool _objOn)
    {
        playerSkillCool.SetActive(_objOn);
        playerSkillCoolText.SetActive(_objOn);
    }

    /// <summary>
    /// 플레이어 스킬 쿨타임 값과 텍스트 값을 받아와 화면에 보여줌
    /// </summary>
    /// <param name="_imageFill"></param>
    /// <param name="_textValue"></param>
    public void SetPlayerSkillCool(float _imageFill, string _textValue)
    {
        Image skillCoolImage = playerSkillCool.GetComponent<Image>();
        skillCoolImage.fillAmount = _imageFill;

        TMP_Text skillCoolText = playerSkillCoolText.GetComponent<TMP_Text>();
        skillCoolText.text = _textValue;
    }

    /// <summary>
    /// 플레이어 대쉬와 관련된 오브젝트를 활성화 또는 비활성화를 담당
    /// </summary>
    /// <param name="_objOn"></param>
    public void PlayerDashCool(bool _objOn)
    {
        playerDashCool.SetActive(_objOn);
        playerDashCoolText.SetActive(_objOn);
    }

    /// <summary>
    /// 플레이어 대쉬 쿨타임 값과 텍스트 값을 받아와 화면에 보여줌
    /// </summary>
    /// <param name="_imageFill"></param>
    /// <param name="_textValue"></param>
    public void SetPlayerDashCool(float _imageFill, string _textValue)
    {
        Image skillDashImage = playerDashCool.GetComponent<Image>();
        skillDashImage.fillAmount = _imageFill;

        TMP_Text skillDashText = playerDashCoolText.GetComponent<TMP_Text>();
        skillDashText.text = _textValue;
    }

    /// <summary>
    /// 플레이어 체력과 관련된 값을 받아와 슬라이더와 텍스트에 넣어 화면에 보여줌
    /// </summary>
    /// <param name="_hpValue"></param>
    /// <param name="_textValue"></param>
    public void SetPlayerHp(int _hpValue, int _hpMaxValue, string _textValue)
    {
        Slider hpSlider = playerHp.GetComponent<Slider>();
        hpSlider.value = _hpValue;
        hpSlider.maxValue = _hpMaxValue;

        TMP_Text HpText = playerHpText.GetComponent<TMP_Text>();
        HpText.text = _textValue;
    }

    public void OptionOn(bool _on)
    {
        option.SetActive(_on);
    }
}
