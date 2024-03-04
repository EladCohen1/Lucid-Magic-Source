using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardAbilityScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Objects")]
    public GameObject Outline;
    public Color targetColor;
    public Color subTargetColor;
    public Color beneficialTargetColor;
    public Color subBeneficialTargetColor;

    [Header("Scripts")]
    private CardManagerScript cardManagerScript;

    // Abilities
    public List<Ability> abilities;
    // Covers will match abilities by order (order of abilityTriggerScripts array in cardmanager)
    public List<GameObject> abilityCovers;

    void Awake()
    {
        cardManagerScript = gameObject.GetComponent<CardManagerScript>();
        abilities = new List<Ability>();
        foreach (AbilityTriggerScript abilityTriggerScript in gameObject.GetComponent<CardManagerScript>().abilityTriggerScripts)
        {
            abilities.Add(new Ability(abilityTriggerScript.ability));
        }
    }
    void OnEnable()
    {
        AbilitySystemManagerScript.EndHover += AbilitySystemManagerScriptEndHover;
        InGameManagerScript.OnInGameStateChanged += InGameManagerOnInGameStateChanged;
    }
    void OnDisable()
    {
        AbilitySystemManagerScript.EndHover -= AbilitySystemManagerScriptEndHover;
        InGameManagerScript.OnInGameStateChanged += InGameManagerOnInGameStateChanged;
    }

    void Update()
    {
        if (InGameManagerScript.Instance.IsPlayerTypeTurn(cardManagerScript.Owner))
        {
            for (int i = 0; i < abilities.Count; i++)
            {
                if (!(abilities[i].didUse || ManaSystemManagerScript.Instance.GetCurrentPlayerMana() < abilities[i].type.GetAbilityManaCost()))
                {
                    abilityCovers[i].SetActive(false);
                }
                else
                {
                    abilityCovers[i].SetActive(true);
                }
            }
        }
    }

    private void AbilitySystemManagerScriptEndHover()
    {
        SetHoveringState(ColorType.NoColor);
    }

    // Turn Change
    private void InGameManagerOnInGameStateChanged(InGameState state)
    {
        switch (state)
        {
            case InGameState.PlayerTurn:
                if (cardManagerScript.Owner == PlayerType.player)
                {
                    ResetDidUseAbilities();
                }
                break;
            case InGameState.EnemyTurn:
                if (cardManagerScript.Owner == PlayerType.enemy)
                {
                    ResetDidUseAbilities();
                }
                break;
            default:
                break;
        }
    }

    // Events
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AbilitySystemManagerScript.Instance.ActiveAbility != AbilityType.noAbility
        && AbilitySystemManagerScript.Instance.User != null)
        {
            if (AbilitySystemManagerScript.Instance.ActiveAbility.CheckTargetEligibility(AbilitySystemManagerScript.Instance.User, gameObject))
            {
                AbilitySystemManagerScript.Instance.ActiveAbility.SetTargetHover(AbilitySystemManagerScript.Instance.User, gameObject);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AbilitySystemManagerScript.Instance.InvokeEndHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AbilitySystemManagerScript.Instance.ActiveAbility != AbilityType.noAbility
        && AbilitySystemManagerScript.Instance.User != null)
        {
            if (AbilitySystemManagerScript.Instance.ActiveAbility.CheckTargetEligibility(AbilitySystemManagerScript.Instance.User, gameObject))
            {
                AbilitySystemManagerScript.Instance.SetTarget(gameObject);
            }
        }
    }

    // Utils
    public void ResetDidUseAbilities()
    {
        foreach (Ability item in abilities)
        {
            item.didUse = false;
            item.timesUsedThisTurn = 0;
        }
    }
    public void SetHoveringState(ColorType colorType)
    {
        switch (colorType)
        {
            case ColorType.TargetColor:
                Outline.SetActive(true);
                Outline.GetComponent<Image>().color = targetColor;
                break;
            case ColorType.SubTargetColor:
                Outline.SetActive(true);
                Outline.GetComponent<Image>().color = subTargetColor;
                break;
            case ColorType.BeneficialTargetColor:
                Outline.SetActive(true);
                Outline.GetComponent<Image>().color = beneficialTargetColor;
                break;
            case ColorType.SubBeneficialTargetColor:
                Outline.SetActive(true);
                Outline.GetComponent<Image>().color = subBeneficialTargetColor;
                break;
            default:
                Outline.SetActive(false);
                break;
        }
    }
}