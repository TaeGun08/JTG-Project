using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillUIExplanation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject explanation;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        explanation.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        explanation.SetActive(false);
    }

    private void Start()
    {
        explanation.SetActive(false);
    }
}
