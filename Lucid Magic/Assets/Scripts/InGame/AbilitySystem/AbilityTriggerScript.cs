using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class AbilityTriggerScript : MonoBehaviour, IPointerClickHandler
{
    // Game Variables
    public AbilityType ability;
    public GameObject ThisCard;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            AbilitySystemManagerScript.Instance.SetAbility(ThisCard, ability);
            ThisCard.GetComponent<CardAbilityScript>().OnPointerEnter(null);
        }
    }
}
