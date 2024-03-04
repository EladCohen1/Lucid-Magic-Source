using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public enum EffectType
{
    Harvest,
    ActiveHarvest,
    RoyalBlood,
    Rage,
    KnightlyCommand,
    NaturesBlessing
}

public static class EffectTypeMethods
{
    // Activate
    public static void ActivateEffect(this EffectType effect, GameObject effectedCard, Ability abilityReference)
    {
        switch (effect)
        {
            case EffectType.Harvest:
                Harvest(effectedCard, abilityReference);
                break;
            case EffectType.ActiveHarvest:
                ActiveHarvest(effectedCard, abilityReference);
                break;
            case EffectType.RoyalBlood:
                RoyalBlood(effectedCard, abilityReference);
                break;
            case EffectType.Rage:
                Rage(effectedCard, abilityReference);
                break;
            case EffectType.KnightlyCommand:
                KnightlyCommand(effectedCard, abilityReference);
                break;
            case EffectType.NaturesBlessing:
                NaturesBlessing(effectedCard, abilityReference);
                break;
            default:
                break;
        }
    }

    private static void Harvest(GameObject effectedCard, Ability abilityReference)
    {
        effectedCard.GetComponent<CardManagerScript>().Effects.Add(new Effect(EffectType.Harvest, abilityReference, effectedCard));
    }
    private static void ActiveHarvest(GameObject effectedCard, Ability abilityReference)
    {
        int counter = 0;
        List<Effect> cardEffects = effectedCard.GetComponent<CardManagerScript>().Effects;
        for (int i = cardEffects.Count() - 1; i >= 0; i--)
        {
            if (cardEffects[i].type == EffectType.Harvest)
            {
                counter++;
                EffectType.Harvest.Remove(effectedCard, i);
            }
        }
        for (int i = 0; i < counter; i++)
        {
            cardEffects.Add(new Effect(EffectType.ActiveHarvest, abilityReference, effectedCard));
            CardManagerScript effectedCardManager = effectedCard.GetComponent<CardManagerScript>();
            effectedCardManager.IncreaseMaxHealth(1);
            effectedCardManager.IncreaseCurrentAttack(1);
            effectedCardManager.IncreaseCurrentHealth(1);
        }
    }
    private static void RoyalBlood(GameObject effectedCard, Ability abilityReference)
    {
        effectedCard.GetComponent<CardManagerScript>().Effects.Add(new Effect(EffectType.RoyalBlood, abilityReference, effectedCard));
    }
    private static void Rage(GameObject effectedCard, Ability abilityReference)
    {
        effectedCard.GetComponent<CardManagerScript>().Effects.Add(new Effect(EffectType.Rage, abilityReference, effectedCard));
    }
    private static void KnightlyCommand(GameObject effectedCard, Ability abilityReference)
    {
        CardManagerScript effectedCardManager = effectedCard.GetComponent<CardManagerScript>();
        effectedCardManager.Effects.Add(new Effect(EffectType.KnightlyCommand, abilityReference, effectedCard));
        effectedCardManager.IncreaseCurrentAttack(1);
        effectedCardManager.IncreaseMaxHealth(1);
        if (!effectedCardManager.IsDead)
        {
            effectedCardManager.IncreaseCurrentHealth(1);
        }
    }
    private static void NaturesBlessing(GameObject effectedCard, Ability abilityReference)
    {
        effectedCard.GetComponent<CardManagerScript>().Effects.Add(new Effect(EffectType.NaturesBlessing, abilityReference, effectedCard));
    }

    // Remove
    public static void RemoveAll(this EffectType effect, GameObject effectedCard)
    {
        List<Effect> cardEffects = effectedCard.GetComponent<CardManagerScript>().Effects;
        for (int i = cardEffects.Count() - 1; i >= 0; i--)
        {
            if (cardEffects[i].type == effect)
            {
                GameObject prevActiveOn = cardEffects[i].abilityReference.ActiveOn;
                if (GameObject.ReferenceEquals(cardEffects[i].abilityReference.ActiveOn, effectedCard))
                {
                    cardEffects[i].abilityReference.ActiveOn = null;
                }
                if (cardEffects[i].Icon != null)
                {
                    int multiplier = cardEffects[i].Icon.GetComponent<ToolTipTriggerScript>().multiplier;
                    if (multiplier <= 1)
                    {
                        EffectIconsDictionary.Instance.DestroyIconOfEffect(cardEffects[i].Icon);
                    }
                    else
                    {
                        cardEffects[i].Icon.GetComponent<ToolTipTriggerScript>().multiplier--;
                    }
                }
                cardEffects.RemoveAt(i);
                effect.RemoveStatChangesOfEffect(effectedCard, prevActiveOn);
            }
        }
    }
    public static void RemoveLast(this EffectType effect, GameObject effectedCard)
    {
        List<Effect> cardEffects = effectedCard.GetComponent<CardManagerScript>().Effects;
        for (int i = cardEffects.Count() - 1; i >= 0; i--)
        {
            if (cardEffects[i].type == effect)
            {
                GameObject prevActiveOn = cardEffects[i].abilityReference.ActiveOn;
                if (GameObject.ReferenceEquals(cardEffects[i].abilityReference.ActiveOn, effectedCard))
                {
                    cardEffects[i].abilityReference.ActiveOn = null;
                }
                if (cardEffects[i].Icon != null)
                {
                    int multiplier = cardEffects[i].Icon.GetComponent<ToolTipTriggerScript>().multiplier;
                    if (multiplier <= 1)
                    {
                        EffectIconsDictionary.Instance.DestroyIconOfEffect(cardEffects[i].Icon);
                    }
                    else
                    {
                        cardEffects[i].Icon.GetComponent<ToolTipTriggerScript>().multiplier--;
                    }
                }
                cardEffects.RemoveAt(i);
                effect.RemoveStatChangesOfEffect(effectedCard, prevActiveOn);
                return;
            }
        }
    }
    public static void RemoveAll(GameObject effectedCard)
    {
        List<Effect> cardEffects = effectedCard.GetComponent<CardManagerScript>().Effects;
        for (int i = cardEffects.Count() - 1; i >= 0; i--)
        {
            GameObject prevActiveOn = cardEffects[i].abilityReference.ActiveOn;
            if (GameObject.ReferenceEquals(cardEffects[i].abilityReference.ActiveOn, effectedCard))
            {
                cardEffects[i].abilityReference.ActiveOn = null;
            }
            if (cardEffects[i].Icon != null)
            {
                int multiplier = cardEffects[i].Icon.GetComponent<ToolTipTriggerScript>().multiplier;
                if (multiplier <= 1)
                {
                    EffectIconsDictionary.Instance.DestroyIconOfEffect(cardEffects[i].Icon);
                }
                else
                {
                    cardEffects[i].Icon.GetComponent<ToolTipTriggerScript>().multiplier--;
                }
            }
            cardEffects.RemoveAt(i);
            cardEffects[i].type.RemoveStatChangesOfEffect(effectedCard, prevActiveOn);
        }
    }

    // Remove Given Effect Index
    public static void Remove(this EffectType effect, GameObject effectedCard, int effectindex)
    {
        List<Effect> cardEffects = effectedCard.GetComponent<CardManagerScript>().Effects;
        if (cardEffects[effectindex].type == effect)
        {
            GameObject prevActiveOn = cardEffects[effectindex].abilityReference.ActiveOn;
            if (GameObject.ReferenceEquals(cardEffects[effectindex].abilityReference.ActiveOn, effectedCard))
            {
                cardEffects[effectindex].abilityReference.ActiveOn = null;
            }
            if (cardEffects[effectindex].Icon != null)
            {
                int multiplier = cardEffects[effectindex].Icon.GetComponent<ToolTipTriggerScript>().multiplier;
                if (multiplier <= 1)
                {
                    EffectIconsDictionary.Instance.DestroyIconOfEffect(cardEffects[effectindex].Icon);
                }
                else
                {
                    cardEffects[effectindex].Icon.GetComponent<ToolTipTriggerScript>().multiplier--;
                }
            }
            cardEffects.RemoveAt(effectindex);
            effect.RemoveStatChangesOfEffect(effectedCard, prevActiveOn);
        }
    }

    // Utils
    private static void RemoveStatChangesOfEffect(this EffectType effect, GameObject effectedCard, GameObject prevActiveOn)
    {
        switch (effect)
        {
            case EffectType.KnightlyCommand:
                CardManagerScript effectedCardManager = effectedCard.GetComponent<CardManagerScript>();
                if (GameObject.ReferenceEquals(prevActiveOn, effectedCard))
                {
                    GameObject[] availableTargetsArray = PreGameManagerScript.Instance.GetSelectedCardsByOwner(effectedCardManager.Owner).ToArray();
                    foreach (GameObject item in availableTargetsArray)
                    {
                        if (!GameObject.ReferenceEquals(item, effectedCard))
                        {
                            EffectType.KnightlyCommand.RemoveLast(item);
                        }
                    }
                }
                effectedCardManager.ReduceCurrentAttack(1);
                effectedCardManager.ReduceMaxHealth(1);
                break;
            case EffectType.ActiveHarvest:
                effectedCard.GetComponent<CardManagerScript>().ReduceMaxHealth(1);
                effectedCard.GetComponent<CardManagerScript>().ReduceCurrentAttack(1);
                break;
            default:
                break;
        }
    }
    public static bool IsAura(this EffectType effect)
    {
        switch (effect)
        {
            case EffectType.Harvest:
                return false;
            case EffectType.RoyalBlood:
                return false;
            case EffectType.Rage:
                return false;
            case EffectType.KnightlyCommand:
                return true;
            case EffectType.NaturesBlessing:
                return false;
            default:
                return false;
        }
    }
    public static bool IsRemovedOnDeath(Effect effect, GameObject target)
    {
        switch (effect.type)
        {
            case EffectType.Harvest:
                return true;
            case EffectType.ActiveHarvest:
                return true;
            case EffectType.RoyalBlood:
                return true;
            case EffectType.Rage:
                return true;
            case EffectType.KnightlyCommand:
                return GameObject.ReferenceEquals(effect.abilityReference.ActiveOn, target);
            case EffectType.NaturesBlessing:
                return true;
            default:
                return true;
        }
    }
}

public class Effect
{
    public EffectType type;
    public Ability abilityReference;
    public GameObject Icon;

    public int turnsActive = 0;

    public Effect(EffectType type, Ability abilityReference)
    {
        this.type = type;
        this.abilityReference = abilityReference;
    }
    public Effect(EffectType type, Ability abilityReference, GameObject effectedCard)
    {
        this.type = type;
        this.abilityReference = abilityReference;
        if (effectedCard != null)
        {
            Effect effectAlreadyOnCard = effectedCard.GetComponent<CardManagerScript>().GetFirstEffectFromEffectsByType(type);
            if (effectAlreadyOnCard != null)
            {
                Icon = effectAlreadyOnCard.Icon;
                Icon.GetComponent<ToolTipTriggerScript>().multiplier++;
            }
            else
            {
                Icon = EffectIconsDictionary.Instance.GetIconOfEffect(type);
                if (Icon != null)
                {
                    Icon.transform.SetParent(effectedCard.transform.Find("EffectArea"), false);
                }
            }
        }
    }
}