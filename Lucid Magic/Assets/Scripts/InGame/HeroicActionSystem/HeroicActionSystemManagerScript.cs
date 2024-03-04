using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HeroicActionSystemManagerScript : MonoBehaviour
{
    public static HeroicActionSystemManagerScript Instance { get; private set; }
    public static Action DoCancelReviveCompanion;

    [Header("Buttons")]
    public GameObject PlayerIncreaseMaxManaButton;
    public GameObject EnemyIncreaseMaxManaButton;
    public GameObject PlayerReviveCompanionButton;
    public GameObject EnemyReviveCompanionButton;

    [Header("Objects")]
    public GameObject ReviveTarget;

    // Game Variables
    public bool manaSurgeUsedThisTurn;
    public bool reviveCompanionUsedThisTurn;
    public int manaSurgeCost = 1;
    public int reviveCompanionCost = 3;
    public bool isSearchingForReviveTarget;
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
        PlayerIncreaseMaxManaButton.SetActive(true);
        EnemyIncreaseMaxManaButton.SetActive(true);
        PlayerReviveCompanionButton.SetActive(true);
        EnemyReviveCompanionButton.SetActive(true);
        manaSurgeUsedThisTurn = false;
        reviveCompanionUsedThisTurn = false;
        isSearchingForReviveTarget = false;
        InGameManagerScript.OnInGameStateChanged += InGameManagerOnInGameStateChanged;
    }
    void OnDisable()
    {
        InGameManagerScript.OnInGameStateChanged -= InGameManagerOnInGameStateChanged;
        if (PlayerIncreaseMaxManaButton != null && EnemyIncreaseMaxManaButton != null
        && PlayerReviveCompanionButton != null && EnemyReviveCompanionButton != null)
        {
            PlayerIncreaseMaxManaButton.SetActive(false);
            EnemyIncreaseMaxManaButton.SetActive(false);
            PlayerReviveCompanionButton.SetActive(false);
            EnemyReviveCompanionButton.SetActive(false);
        }
    }

    // In game state change
    private void InGameManagerOnInGameStateChanged(InGameState state)
    {
        switch (state)
        {
            case InGameState.PlayerTurn:
                HandlePlayerTurnState();
                break;
            case InGameState.EnemyTurn:
                HandleEnemyTurnState();
                break;
            default:
                break;
        }
    }
    private void HandlePlayerTurnState()
    {
        manaSurgeUsedThisTurn = false;
        reviveCompanionUsedThisTurn = false;
    }
    private void HandleEnemyTurnState()
    {
        manaSurgeUsedThisTurn = false;
        reviveCompanionUsedThisTurn = false;
    }

    // Revive Setup and checks
    public void SetReviveTarget(GameObject newReviveTarget)
    {
        if (newReviveTarget == null)
        {
            ReviveTarget = null;
            return;
        }
        if (CheckReviveTargetEligibility(newReviveTarget))
        {
            ReviveTarget = newReviveTarget;
            if (isSearchingForReviveTarget)
            {
                UseReviveCompanion(ReviveTarget);
            }
        }
        else
        {
            ReviveTarget = null;
        }
    }
    private bool CheckReviveTargetEligibility(GameObject newReviveTarget)
    {
        CardManagerScript cardManagerScript = newReviveTarget.GetComponent<CardManagerScript>();
        if (InGameManagerScript.Instance.IsPlayerTypeTurn(cardManagerScript.Owner))
        {
            // if card is dead
            if (cardManagerScript.IsDead)
            {
                return true;
            }
            else
            {
                CancelReviveCompanion();
                PopupManagerScript.TriggerPopup?.Invoke("This card is alive, Revive Companion may only be used on your dead cards", "Unable To Revive!");
            }
        }
        else
        {
            CancelReviveCompanion();
            PopupManagerScript.TriggerPopup?.Invoke("Revive Companion may only be used on your own cards", "Unable To Revive!");
        }

        return false;
    }


    // Utils
    public bool UseManaSurge(PlayerType player)
    {
        // check if trying to revive companion
        if (HeroicActionSystemManagerScript.Instance.isSearchingForReviveTarget)
        {
            HeroicActionSystemManagerScript.Instance.CancelReviveCompanion();
            return manaSurgeUsedThisTurn;
        }
        // check if trying to use an ability
        if (AbilitySystemManagerScript.Instance.ActiveAbility != AbilityType.noAbility)
        {
            AbilitySystemManagerScript.Instance.ResetSelection();
            return manaSurgeUsedThisTurn;
        }
        if ((InGameManagerScript.Instance.State == InGameState.PlayerTurn && player == PlayerType.player)
        || (InGameManagerScript.Instance.State == InGameState.EnemyTurn && player == PlayerType.enemy))
        {
            if (!manaSurgeUsedThisTurn)
            {
                if (ManaSystemManagerScript.Instance.CanUseMana(1))
                {
                    if (ManaSystemManagerScript.Instance.AddMaxMana(1))
                    {
                        manaSurgeUsedThisTurn = true;
                        ManaSystemManagerScript.Instance.UseMana(1);
                    }
                }
            }
        }
        return manaSurgeUsedThisTurn;
    }
    public void UseReviveCompanion(GameObject cardToRevive)
    {
        CardManagerScript cardToReviveManager = cardToRevive.GetComponent<CardManagerScript>();
        // if card is dead
        if (cardToReviveManager.IsDead)
        {
            // if can use mana
            if (ManaSystemManagerScript.Instance.UseMana(reviveCompanionCost))
            {
                cardToReviveManager.ReviveCard();
                reviveCompanionUsedThisTurn = true;
                CancelReviveCompanion();
            }
        }
    }
    public void CancelReviveCompanion()
    {
        isSearchingForReviveTarget = false;
        SetReviveTarget(null);
        BattleFieldManagerScript.Instance.UpdateCursorBasedOnBattleField();
        DoCancelReviveCompanion?.Invoke();
        if (InGameManagerScript.Instance.State == InGameState.PlayerTurn)
        {
            PlayerReviveCompanionButton.GetComponent<ReviveCompanionButtonScript>().UpdateButtonState();
        }
        else if (InGameManagerScript.Instance.State == InGameState.EnemyTurn)
        {
            EnemyReviveCompanionButton.GetComponent<ReviveCompanionButtonScript>().UpdateButtonState();
        }
    }
}