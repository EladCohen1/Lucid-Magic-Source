using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardReviveCompanionScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Objects")]
    public GameObject Outline;
    public Color color;

    [Header("Scripts")]
    private CardManagerScript cardManagerScript;

    // Game Variables
    private bool isHovering = false;
    public bool IsHovering
    {
        get { return isHovering; }
        set
        {
            isHovering = value;
            if (isHovering)
            {
                Outline.SetActive(true);
                Outline.GetComponent<Image>().color = color;
                cardManagerScript.DisplayReviveStats();
            }
            else
            {
                Outline.SetActive(false);
                cardManagerScript.UpdateDisplay();
            }
        }
    }

    void Awake()
    {
        cardManagerScript = gameObject.GetComponent<CardManagerScript>();
    }
    void OnEnable()
    {
        HeroicActionSystemManagerScript.DoCancelReviveCompanion += HeroicActionSystemManagerCancelReviveCompanion;
    }
    void OnDisable()
    {
        HeroicActionSystemManagerScript.DoCancelReviveCompanion -= HeroicActionSystemManagerCancelReviveCompanion;
    }

    private void HeroicActionSystemManagerCancelReviveCompanion()
    {
        IsHovering = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (HeroicActionSystemManagerScript.Instance.isSearchingForReviveTarget)
        {
            HeroicActionSystemManagerScript.Instance.SetReviveTarget(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (HeroicActionSystemManagerScript.Instance.isSearchingForReviveTarget
        && cardManagerScript.IsDead
        && InGameManagerScript.Instance.IsPlayerTypeTurn(cardManagerScript.Owner))
        {
            IsHovering = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovering = false;
    }
}
