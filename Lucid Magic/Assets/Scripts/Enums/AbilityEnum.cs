using UnityEngine;
using System.Linq;

public enum AbilityType
{
    Harvest,
    RoyalBlood,
    Fireball,
    DesperatePrayer,
    MonstrousTactics,
    Rage,
    KnightlyCommand,
    NaturesBlessing,
    noAbility
}

public static class AbilityTypeMethods
{
    // Use
    public static void UseAbility(this AbilityType ability, GameObject user, GameObject target)
    {
        switch (ability)
        {
            case AbilityType.Harvest:
                Harvest(user, target);
                break;
            case AbilityType.RoyalBlood:
                RoyalBlood(user, target);
                break;
            case AbilityType.Fireball:
                Fireball(user, target);
                break;
            case AbilityType.DesperatePrayer:
                DesperatePrayer(user, target);
                break;
            case AbilityType.MonstrousTactics:
                MonstrousTactics(user, target);
                break;
            case AbilityType.Rage:
                Rage(user, target);
                break;
            case AbilityType.KnightlyCommand:
                KnightlyCommand(user, target);
                break;
            case AbilityType.NaturesBlessing:
                NaturesBlessing(user, target);
                break;
            default:
                break;
        }
    }
    private static void Harvest(GameObject user, GameObject target)
    {
        Ability abilityInCard = null;
        foreach (Ability item in user.GetComponent<CardAbilityScript>().abilities)
        {
            if (item.type == AbilityType.Harvest && !item.didUse)
            {
                abilityInCard = item;
            }
        }
        if (abilityInCard != null)
        {
            if (ManaSystemManagerScript.Instance.UseMana(abilityInCard.type.GetAbilityManaCost()))
            {
                abilityInCard.timesUsedThisTurn++;
                EffectType.Harvest.ActivateEffect(target, abilityInCard);
                if (abilityInCard.timesUsedThisTurn >= 3)
                {
                    abilityInCard.didUse = true;
                }
            }
        }
    }
    private static void RoyalBlood(GameObject user, GameObject target)
    {
        Ability abilityInCard = null;
        foreach (Ability item in user.GetComponent<CardAbilityScript>().abilities)
        {
            if (item.type == AbilityType.RoyalBlood && !item.didUse)
            {
                abilityInCard = item;
            }
        }
        if (abilityInCard != null)
        {
            if (ManaSystemManagerScript.Instance.UseMana(abilityInCard.type.GetAbilityManaCost()))
            {
                if (abilityInCard.ActiveOn != null)
                {
                    EffectType.RoyalBlood.RemoveLast(abilityInCard.ActiveOn);
                }
                abilityInCard.ActiveOn = target;
                EffectType.RoyalBlood.ActivateEffect(target, abilityInCard);
                abilityInCard.didUse = true;
            }
        }
    }
    private static void Fireball(GameObject user, GameObject target)
    {
        Ability abilityInCard = null;
        foreach (Ability item in user.GetComponent<CardAbilityScript>().abilities)
        {
            if (item.type == AbilityType.Fireball && !item.didUse)
            {
                abilityInCard = item;
            }
        }
        if (abilityInCard != null)
        {
            int targetIndex = target.transform.GetSiblingIndex();
            GameObject brother1 = TryToGetChildOfIndex(target.transform.parent.gameObject, targetIndex + 1);
            GameObject brother2 = TryToGetChildOfIndex(target.transform.parent.gameObject, targetIndex - 1);
            if (ManaSystemManagerScript.Instance.UseMana(abilityInCard.type.GetAbilityManaCost()))
            {
                target.GetComponent<CardManagerScript>().ReduceCurrentHealth(3);
                if (brother1 != null)
                {
                    brother1.GetComponent<CardManagerScript>().ReduceCurrentHealth(1);
                }
                if (brother2 != null)
                {
                    brother2.GetComponent<CardManagerScript>().ReduceCurrentHealth(1);
                }
                abilityInCard.didUse = true;
            }
        }
    }
    private static void DesperatePrayer(GameObject user, GameObject target)
    {
        Ability abilityInCard = null;
        foreach (Ability item in user.GetComponent<CardAbilityScript>().abilities)
        {
            if (item.type == AbilityType.DesperatePrayer && !item.didUse)
            {
                abilityInCard = item;
            }
        }
        if (abilityInCard != null)
        {
            if (ManaSystemManagerScript.Instance.UseMana(abilityInCard.type.GetAbilityManaCost()))
            {
                target.GetComponent<CardManagerScript>().IncreaseCurrentHealth(2);
                abilityInCard.didUse = true;
            }
        }
    }
    private static void MonstrousTactics(GameObject user, GameObject target)
    {
        Ability abilityInCard = null;
        foreach (Ability item in user.GetComponent<CardAbilityScript>().abilities)
        {
            if (item.type == AbilityType.MonstrousTactics && !item.didUse)
            {
                abilityInCard = item;
            }
        }
        if (abilityInCard != null)
        {
            if (ManaSystemManagerScript.Instance.UseMana(abilityInCard.type.GetAbilityManaCost()))
            {
                GameObject[] availableTargetsArray = PreGameManagerScript.Instance.GetSelectedCardByNonTurn().Where(card => !card.GetComponent<CardManagerScript>().IsDead).ToArray();
                if (availableTargetsArray.Length > 0)
                {
                    int randomTargetIndex = UnityEngine.Random.Range(0, availableTargetsArray.Length);
                    GameObject randomTarget = availableTargetsArray[randomTargetIndex];
                    AttackSystemManagerScript.Instance.ExecuteAttackNoRestrictions(user, randomTarget, 1);
                    abilityInCard.didUse = true;
                }
            }
        }
    }
    private static void Rage(GameObject user, GameObject target)
    {
        Ability abilityInCard = null;
        foreach (Ability item in user.GetComponent<CardAbilityScript>().abilities)
        {
            if (item.type == AbilityType.Rage && !item.didUse)
            {
                abilityInCard = item;
            }
        }
        if (abilityInCard != null)
        {
            if (ManaSystemManagerScript.Instance.UseMana(abilityInCard.type.GetAbilityManaCost()))
            {
                abilityInCard.ActiveOn = target;
                EffectType.Rage.ActivateEffect(target, abilityInCard);
                abilityInCard.didUse = true;
            }
        }
    }
    private static void KnightlyCommand(GameObject user, GameObject target)
    {
        Ability abilityInCard = null;
        foreach (Ability item in user.GetComponent<CardAbilityScript>().abilities)
        {
            if (item.type == AbilityType.KnightlyCommand && !item.didUse)
            {
                abilityInCard = item;
            }
        }
        if (abilityInCard != null)
        {
            if (ManaSystemManagerScript.Instance.UseMana(abilityInCard.type.GetAbilityManaCost()))
            {
                AttackSystemManagerScript.Instance.ExecuteAttackNoRestrictions(user, target);
                if (!user.GetComponent<CardManagerScript>().IsDead && target.GetComponent<CardManagerScript>().IsDead)
                {
                    GameObject[] availableTargetsArray = PreGameManagerScript.Instance.GetSelectedCardsByOwner(user.GetComponent<CardManagerScript>().Owner).ToArray();
                    abilityInCard.ActiveOn = user;
                    foreach (GameObject item in availableTargetsArray)
                    {
                        EffectType.KnightlyCommand.ActivateEffect(item, abilityInCard);
                    }
                }
                abilityInCard.didUse = true;
            }
        }
    }
    private static void NaturesBlessing(GameObject user, GameObject target)
    {
        Ability abilityInCard = null;
        foreach (Ability item in user.GetComponent<CardAbilityScript>().abilities)
        {
            if (item.type == AbilityType.NaturesBlessing && !item.didUse)
            {
                abilityInCard = item;
            }
        }
        if (abilityInCard != null)
        {
            if (ManaSystemManagerScript.Instance.UseMana(abilityInCard.type.GetAbilityManaCost()))
            {
                EffectType.NaturesBlessing.ActivateEffect(target, abilityInCard);
                abilityInCard.didUse = true;
            }
        }
    }

    // Check Target
    public static bool CheckTargetEligibility(this AbilityType ability, GameObject user, GameObject target)
    {
        switch (ability)
        {
            case AbilityType.Harvest:
                return CanTargetHarvest(user, target);
            case AbilityType.RoyalBlood:
                return CanTargetRoyalBlood(user, target);
            case AbilityType.Fireball:
                return CanTargetFireball(user, target);
            case AbilityType.DesperatePrayer:
                return CanTargetDesperatePrayer(user, target);
            case AbilityType.MonstrousTactics:
                return CanTargetMonstrousTactics(user, target);
            case AbilityType.Rage:
                return CanTargetRage(user, target);
            case AbilityType.KnightlyCommand:
                return CanTargetKnightlyCommand(user, target);
            case AbilityType.NaturesBlessing:
                return CanTargetNaturesBlessing(user, target);
            default:
                return false;
        }
    }
    private static bool CanTargetHarvest(GameObject user, GameObject target)
    {
        if (GameObject.ReferenceEquals(user, target) && !user.GetComponent<CardManagerScript>().IsDead)
        {
            return true;
        }
        return false;
    }
    private static bool CanTargetRoyalBlood(GameObject user, GameObject target)
    {
        if (user.GetComponent<CardManagerScript>().Owner == target.GetComponent<CardManagerScript>().Owner
        && !target.GetComponent<CardManagerScript>().IsDead)
        {
            foreach (Effect effect in target.GetComponent<CardManagerScript>().Effects)
            {
                if (effect.type == EffectType.RoyalBlood)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }
    private static bool CanTargetFireball(GameObject user, GameObject target)
    {
        if (user.GetComponent<CardManagerScript>().Owner != target.GetComponent<CardManagerScript>().Owner
        && !target.GetComponent<CardManagerScript>().IsDead)
        {
            return true;
        }
        return false;
    }
    private static bool CanTargetDesperatePrayer(GameObject user, GameObject target)
    {
        if (user.GetComponent<CardManagerScript>().Owner == target.GetComponent<CardManagerScript>().Owner
        && !target.GetComponent<CardManagerScript>().IsDead)
        {
            return true;
        }
        return false;
    }
    private static bool CanTargetMonstrousTactics(GameObject user, GameObject target)
    {
        if (GameObject.ReferenceEquals(user, target) && !user.GetComponent<CardManagerScript>().IsDead)
        {
            return true;
        }
        return false;
    }
    private static bool CanTargetRage(GameObject user, GameObject target)
    {
        if (GameObject.ReferenceEquals(user, target) && !user.GetComponent<CardManagerScript>().IsDead)
        {
            foreach (Effect effect in target.GetComponent<CardManagerScript>().Effects)
            {
                if (effect.type == EffectType.Rage)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }
    private static bool CanTargetKnightlyCommand(GameObject user, GameObject target)
    {
        if (user.GetComponent<CardManagerScript>().Owner != target.GetComponent<CardManagerScript>().Owner
        && !target.GetComponent<CardManagerScript>().IsDead)
        {
            return true;
        }
        return false;
    }
    private static bool CanTargetNaturesBlessing(GameObject user, GameObject target)
    {
        if (user.GetComponent<CardManagerScript>().Owner == target.GetComponent<CardManagerScript>().Owner
        && !target.GetComponent<CardManagerScript>().IsDead)
        {
            foreach (Effect effect in target.GetComponent<CardManagerScript>().Effects)
            {
                if (effect.type == EffectType.NaturesBlessing)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }


    // Set Target Hover
    public static void SetTargetHover(this AbilityType ability, GameObject user, GameObject target)
    {
        switch (ability)
        {
            case AbilityType.Harvest:
                SetTargetHovertSingleBenefit(user, target);
                break;
            case AbilityType.RoyalBlood:
                SetTargetHovertSingleBenefit(user, target);
                break;
            case AbilityType.Fireball:
                SetTargetHovertSplashHarm(user, target);
                break;
            case AbilityType.DesperatePrayer:
                SetTargetHovertSingleBenefit(user, target);
                break;
            case AbilityType.MonstrousTactics:
                SetTargetHovertSingleBenefit(user, target);
                break;
            case AbilityType.Rage:
                SetTargetHovertSingleBenefit(user, target);
                break;
            case AbilityType.KnightlyCommand:
                SetTargetHovertSingleHarm(user, target);
                break;
            case AbilityType.NaturesBlessing:
                SetTargetHovertSingleBenefit(user, target);
                break;
            default:
                break;
        }
    }
    private static void SetTargetHovertSingleBenefit(GameObject user, GameObject target)
    {
        target.GetComponent<CardAbilityScript>().SetHoveringState(ColorType.BeneficialTargetColor);
    }
    private static void SetTargetHovertSingleHarm(GameObject user, GameObject target)
    {
        target.GetComponent<CardAbilityScript>().SetHoveringState(ColorType.TargetColor);
    }
    private static void SetTargetHovertSplashBenefit(GameObject user, GameObject target)
    {
        int targetIndex = target.transform.GetSiblingIndex();
        target.GetComponent<CardAbilityScript>().SetHoveringState(ColorType.BeneficialTargetColor);
        GameObject brother1 = TryToGetChildOfIndex(target.transform.parent.gameObject, targetIndex + 1);
        GameObject brother2 = TryToGetChildOfIndex(target.transform.parent.gameObject, targetIndex - 1);
        if (brother1 != null)
        {
            brother1.GetComponent<CardAbilityScript>().SetHoveringState(ColorType.SubBeneficialTargetColor);
        }
        if (brother2 != null)
        {
            brother2.GetComponent<CardAbilityScript>().SetHoveringState(ColorType.SubBeneficialTargetColor);
        }
    }
    private static void SetTargetHovertSplashHarm(GameObject user, GameObject target)
    {
        int targetIndex = target.transform.GetSiblingIndex();
        target.GetComponent<CardAbilityScript>().SetHoveringState(ColorType.TargetColor);
        GameObject brother1 = TryToGetChildOfIndex(target.transform.parent.gameObject, targetIndex + 1);
        GameObject brother2 = TryToGetChildOfIndex(target.transform.parent.gameObject, targetIndex - 1);
        if (brother1 != null)
        {
            brother1.GetComponent<CardAbilityScript>().SetHoveringState(ColorType.SubTargetColor);
        }
        if (brother2 != null)
        {
            brother2.GetComponent<CardAbilityScript>().SetHoveringState(ColorType.SubTargetColor);
        }
    }


    // Check User
    public static bool CheckUserEligibility(this AbilityType ability, GameObject user)
    {
        if (user.GetComponent<CardManagerScript>().IsDead)
        {
            return false;
        }
        if (!InGameManagerScript.Instance.IsPlayerTypeTurn(user.GetComponent<CardManagerScript>().Owner))
        {
            return false;
        }
        if (ability.GetAbilityManaCost() > ManaSystemManagerScript.Instance.GetCurrentPlayerMana())
        {
            return false;
        }
        foreach (Ability item in user.GetComponent<CardAbilityScript>().abilities)
        {
            if (item.type == ability && !item.didUse)
            {
                return true;
            }
        }
        return false;
    }

    // Get Cost
    public static int GetAbilityManaCost(this AbilityType ability)
    {
        switch (ability)
        {
            case AbilityType.Harvest:
                return 1;
            case AbilityType.RoyalBlood:
                return 2;
            case AbilityType.Fireball:
                return 3;
            case AbilityType.DesperatePrayer:
                return 1;
            case AbilityType.MonstrousTactics:
                return 1;
            case AbilityType.Rage:
                return 1;
            case AbilityType.KnightlyCommand:
                return 2;
            case AbilityType.NaturesBlessing:
                return 2;
            default:
                return -1;
        }
    }

    // Utils
    private static GameObject TryToGetChildOfIndex(GameObject parent, int indexOfChild)
    {
        try
        {
            return parent.transform.GetChild(indexOfChild).gameObject;
        }
        catch
        {
            return null;
        }
    }
}

public class Ability
{
    public AbilityType type;
    public bool didUse;
    public int timesUsedThisTurn;
    public GameObject ActiveOn;
    public Ability(AbilityType type)
    {
        this.type = type;
        this.didUse = false;
    }
}