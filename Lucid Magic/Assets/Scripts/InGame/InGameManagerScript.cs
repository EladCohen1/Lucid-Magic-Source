using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class InGameManagerScript : MonoBehaviour
{
    public static InGameManagerScript Instance { get; private set; }
    [Header("Static And State")]
    public InGameState State;
    public static event Action<InGameState> OnInGameStateChanged;
    public static Action OnCardDeath;

    [Header("Areas")]
    public GameObject PlayerHeroArea;
    public GameObject PlayerCompanionsArea;
    public GameObject PlayerGraveyard;
    public GameObject EnemyHeroArea;
    public GameObject EnemyCompanionsArea;
    public GameObject EnemyGraveyard;

    [Header("Objects")]
    public GameObject EndTurnButton;
    public GameObject PlayerTurn;
    public GameObject EnemyTurn;

    [Header("Data")]
    public int HeroHealth = 20;

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
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
        OnInGameStateChanged += InGameManagerOnInGameStateChanged;
    }
    void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
        OnInGameStateChanged -= InGameManagerOnInGameStateChanged;
    }

    // Private functions
    private void ClearUnusedCards()
    {
        for (int i = PreGameManagerScript.Instance.instantiatedCards.Count - 1; i >= 0; i--)
        {
            if (!(PreGameManagerScript.Instance.selectedPlayerCards.Contains(PreGameManagerScript.Instance.instantiatedCards[i]) ||
            PreGameManagerScript.Instance.selectedEnemyCards.Contains(PreGameManagerScript.Instance.instantiatedCards[i])))
            {
                Destroy(PreGameManagerScript.Instance.instantiatedCards[i]);
                PreGameManagerScript.Instance.instantiatedCards.RemoveAt(i);
            }
        }
    }
    private void OrganizeCards()
    {
        for (int i = 0; i < PreGameManagerScript.Instance.selectedPlayerCards.Length; i++)
        {
            CardManagerScript cardManager = PreGameManagerScript.Instance.selectedPlayerCards[i].GetComponent<CardManagerScript>();
            cardManager.Owner = PlayerType.player;
            if (i == 0)
            {
                PreGameManagerScript.Instance.selectedPlayerCards[i].transform.SetParent(PlayerHeroArea.transform, false);
                cardManager.MakeHero();
            }
            else
            {
                PreGameManagerScript.Instance.selectedPlayerCards[i].transform.SetParent(PlayerCompanionsArea.transform, false);
            }
        }
        for (int i = 0; i < PreGameManagerScript.Instance.selectedEnemyCards.Length; i++)
        {
            CardManagerScript cardManager = PreGameManagerScript.Instance.selectedEnemyCards[i].GetComponent<CardManagerScript>();
            cardManager.Owner = PlayerType.enemy;
            if (i == 0)
            {
                PreGameManagerScript.Instance.selectedEnemyCards[i].transform.SetParent(EnemyHeroArea.transform, false);
                cardManager.MakeHero();
            }
            else
            {
                PreGameManagerScript.Instance.selectedEnemyCards[i].transform.SetParent(EnemyCompanionsArea.transform, false);
            }
        }
    }

    // Game state change
    private void GameManagerOnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.PreGame:
                HandlePreGameState();
                break;
            case GameState.InGame:
                HandleInGameState();
                break;
            case GameState.PostGame:
                HandlePostGameState();
                break;
            default:
                break;
        }
    }
    private void HandlePreGameState()
    {
        UpdateInGameState(InGameState.OutOfGame);
        PlayerTurn.SetActive(false);
        EnemyTurn.SetActive(false);
        EndTurnButton.SetActive(false);
        ManaSystemManagerScript.Instance.enabled = false;
        AttackSystemManagerScript.Instance.enabled = false;
        BattleFieldManagerScript.Instance.enabled = false;
        HeroicActionSystemManagerScript.Instance.enabled = false;
        AbilitySystemManagerScript.Instance.enabled = false;
    }
    private void HandleInGameState()
    {
        ClearUnusedCards();
        OrganizeCards();
        EndTurnButton.SetActive(true);
        ManaSystemManagerScript.Instance.enabled = true;
        AttackSystemManagerScript.Instance.enabled = true;
        BattleFieldManagerScript.Instance.enabled = true;
        HeroicActionSystemManagerScript.Instance.enabled = true;
        AbilitySystemManagerScript.Instance.enabled = true;
        UpdateInGameState(InGameState.PlayerTurn);
    }
    private void HandlePostGameState()
    {
        UpdateInGameState(InGameState.OutOfGame);
    }


    // In game state change
    public void UpdateInGameState(InGameState newState)
    {
        State = newState;
        OnInGameStateChanged?.Invoke(newState);
    }
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
        PlayerTurn.SetActive(true);
        EnemyTurn.SetActive(false);
    }
    private void HandleEnemyTurnState()
    {
        EnemyTurn.SetActive(true);
        PlayerTurn.SetActive(false);
    }

    // public functions
    public void EndTurn()
    {
        if (!IsUsingAnotherAction())
        {
            if (State == InGameState.PlayerTurn)
            {
                UpdateInGameState(InGameState.EnemyTurn);
            }
            else if (State == InGameState.EnemyTurn)
            {
                UpdateInGameState(InGameState.PlayerTurn);
            }
        }
    }
    public int GetCurrentPlayerNumOfDeadCards()
    {
        // will return -1 if not in game state
        int counter = -1;
        if (State == InGameState.PlayerTurn)
        {
            counter++;
            for (int i = 0; i < PreGameManagerScript.Instance.selectedPlayerCards.Length; i++)
            {
                CardManagerScript cardManager = PreGameManagerScript.Instance.selectedPlayerCards[i].GetComponent<CardManagerScript>();
                if (cardManager.IsDead)
                {
                    counter++;
                }
            }
        }
        else if (State == InGameState.EnemyTurn)
        {
            counter++;
            for (int i = 0; i < PreGameManagerScript.Instance.selectedEnemyCards.Length; i++)
            {
                CardManagerScript cardManager = PreGameManagerScript.Instance.selectedEnemyCards[i].GetComponent<CardManagerScript>();
                if (cardManager.IsDead)
                {
                    counter++;
                }
            }
        }
        return counter;
    }
    public bool IsPlayerTypeTurn(PlayerType player)
    {
        if (player == PlayerType.player)
        {
            if (State == InGameState.PlayerTurn)
            {
                return true;
            }
        }
        else if (player == PlayerType.enemy)
        {
            if (State == InGameState.EnemyTurn)
            {
                return true;
            }
        }
        return false;
    }

    // Utils
    public bool IsUsingAnotherAction()
    {
        return (HeroicActionSystemManagerScript.Instance.isSearchingForReviveTarget
        || AbilitySystemManagerScript.Instance.ActiveAbility != AbilityType.noAbility);
    }
}