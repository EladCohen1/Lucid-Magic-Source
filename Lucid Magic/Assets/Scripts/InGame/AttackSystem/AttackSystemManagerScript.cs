using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackSystemManagerScript : MonoBehaviour
{
    public static AttackSystemManagerScript Instance { get; private set; }

    [Header("Game Variables")]
    public GameObject Attacker;
    public GameObject Defender;
    public int attackCost = 1;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void OnEnable()
    {

    }
    void OnDisable()
    {

    }

    // Set up and checks
    public void SetAttacker(GameObject newAttacker)
    {
        if (newAttacker == null)
        {
            Attacker = null;
            BattleFieldManagerScript.Instance.UpdateCursorBasedOnBattleField();
            return;
        }
        if (CheckAttackerEligibility(newAttacker))
        {
            Attacker = newAttacker;
            Vector2 hotSpot = new Vector2(GameManager.Instance.cursorRedTargetIcon.width / 2f, GameManager.Instance.cursorRedTargetIcon.height / 2f);
            Cursor.SetCursor(GameManager.Instance.cursorRedTargetIcon, hotSpot, CursorMode.ForceSoftware);
        }
        else
        {
            // set up sound effect
            Attacker = null;
        }
    }
    public bool CheckAttackerEligibility(GameObject newAttacker)
    {
        CardManagerScript cardManagerScript = newAttacker.GetComponent<CardManagerScript>();
        CardAttackScript cardAttackScript = newAttacker.GetComponent<CardAttackScript>();
        // is the correct player playing
        if (InGameManagerScript.Instance.IsPlayerTypeTurn(cardManagerScript.Owner))
        {
            // if card isn't dead
            if (!cardManagerScript.IsDead)
            {
                // if has enough mana
                if (ManaSystemManagerScript.Instance.GetCurrentPlayerMana() >= AttackSystemManagerScript.Instance.attackCost)
                {
                    // if didnt attack yet this turn
                    if (!cardAttackScript.AlreadyAttacked)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public void SetDefender(GameObject newDefender)
    {
        if (newDefender == null)
        {
            Defender = null;
        }
        else if (CheckDefenderEligibility(newDefender))
        {
            Defender = newDefender;
            ExecuteAttack();
        }
    }
    public bool CheckDefenderEligibility(GameObject newDefender)
    {
        CardManagerScript cardManagerScript = newDefender.GetComponent<CardManagerScript>();
        CardAttackScript cardAttackScript = newDefender.GetComponent<CardAttackScript>();
        if (!(InGameManagerScript.Instance.IsPlayerTypeTurn(cardManagerScript.Owner) || cardManagerScript.IsDead))
        {
            return true;
        }
        return false;
    }

    // Attacking
    private void ExecuteAttack()
    {
        // check if using another action
        if (InGameManagerScript.Instance.IsUsingAnotherAction())
        {
            return;
        }

        if (Attacker != null && Defender != null && CheckAttackerEligibility(Attacker) && CheckDefenderEligibility(Defender))
        {
            if (!ManaSystemManagerScript.Instance.UseMana(attackCost))
            {
                PopupManagerScript.TriggerPopup?.Invoke("Attacking costs " + attackCost + " mana", "Not Enough Mana!");
                ResetSelection();
                return;
            }
            ExecuteAttackNoRestrictions(Attacker, Defender);
            // Update attacker's "alreadyAttacked"
            Attacker.GetComponent<CardAttackScript>().AlreadyAttacked = true;
            // Update hover outline to disable when attack executed
            Defender.GetComponent<CardAttackScript>().HoverOutline.SetActive(false);
        }
        // clear attacker and defender
        ResetSelection();
    }

    // Utils
    public void ExecuteAttackNoRestrictions(GameObject attacker, GameObject defender, int attackerAttackMod = 0, int defenderAttackMod = 0)
    {
        CardManagerScript defenderManager = defender.GetComponent<CardManagerScript>();
        CardManagerScript attackerManager = attacker.GetComponent<CardManagerScript>();

        if (defenderManager.WillKillHero(attackerManager.CurrentAttack)
        && attackerManager.WillKillHero(defenderManager.CurrentAttack))
        {
            defenderManager.ReduceHealthWithoutKillingOrCappingToZero(attackerManager.CurrentAttack);
            attackerManager.ReduceHealthWithoutKillingOrCappingToZero(defenderManager.CurrentAttack);

            // end game in a draw
            GameManager.Instance.gameOutCome = GameOutCome.Draw;
            GameManager.Instance.UpdateGameState(GameState.PostGame);
        }
        else
        {
            if (defenderManager.IsEffectTypeInEffects(EffectType.RoyalBlood))
            {
                if (BattleFieldManagerScript.Instance.CurrentBattleField == BattleField.Forest)
                {
                    if (attackerManager.CurrentAttack + attackerAttackMod - 1 > 0)
                    {
                        EffectType.RoyalBlood.RemoveLast(defender);
                    }
                }
                else
                {
                    if (attackerManager.CurrentAttack + attackerAttackMod > 0)
                    {
                        EffectType.RoyalBlood.RemoveLast(defender);
                        attackerManager.ReduceCurrentHealth(1);
                    }
                }
            }
            else if (defenderManager.IsEffectTypeInEffects(EffectType.NaturesBlessing))
            {
                defenderManager.ReduceCurrentHealth(attackerManager.CurrentAttack + attackerAttackMod);
                attackerManager.ReduceCurrentHealth(defenderManager.CurrentAttack + defenderAttackMod + 1);
            }
            else
            {
                defenderManager.ReduceCurrentHealth(attackerManager.CurrentAttack + attackerAttackMod);
                attackerManager.ReduceCurrentHealth(defenderManager.CurrentAttack + defenderAttackMod);
            }
        }
    }
    public void ResetSelection()
    {
        Attacker = null;
        Defender = null;
        BattleFieldManagerScript.Instance.UpdateCursorBasedOnBattleField();
    }
}
