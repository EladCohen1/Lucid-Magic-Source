using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ManaSystemManagerScript : MonoBehaviour
{
    public static ManaSystemManagerScript Instance { get; private set; }
    public static Action OnManaChanged;
    [Header("Objects")]
    public GameObject PlayerMana;
    public GameObject EnemyMana;

    [Header("Game Variables")]
    public int PlayerMaxMana;
    public int PlayerCurrentMana;
    public int EnemyMaxMana;
    public int EnemyCurrentMana;
    public int MaxPossibleMana = 10;
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
        InGameManagerScript.OnInGameStateChanged += InGameManagerOnInGameStateChanged;
        PlayerMana.SetActive(true);
        EnemyMana.SetActive(true);
        PlayerMaxMana = 1;
        PlayerCurrentMana = 1;
        EnemyMaxMana = 1;
        EnemyCurrentMana = 1;
        UpdateManaDisplay();
    }
    void OnDisable()
    {
        InGameManagerScript.OnInGameStateChanged -= InGameManagerOnInGameStateChanged;
        if (PlayerMana != null)
        {
            PlayerMana.SetActive(false);
        }
        if (EnemyMana != null)
        {
            EnemyMana.SetActive(false);
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
        PlayerCurrentMana = PlayerMaxMana;
        UpdateManaDisplay();
    }
    private void HandleEnemyTurnState()
    {
        EnemyCurrentMana = EnemyMaxMana;
        UpdateManaDisplay();
    }

    // public functions
    public bool UseMana(int manaToUse, PlayerType player)
    {
        if (player == PlayerType.player)
        {
            if (PlayerCurrentMana - manaToUse >= 0)
            {
                PlayerCurrentMana -= manaToUse;
                UpdateManaDisplay();
                OnManaChanged?.Invoke();
                return true;
            }
        }
        else if (player == PlayerType.enemy)
        {
            if (EnemyCurrentMana - manaToUse >= 0)
            {
                EnemyCurrentMana -= manaToUse;
                UpdateManaDisplay();
                OnManaChanged?.Invoke();
                return true;
            }
        }
        return false;
    }
    public bool UseMana(int manaToUse)
    {
        if (InGameManagerScript.Instance.State == InGameState.PlayerTurn)
        {
            if (PlayerCurrentMana - manaToUse >= 0)
            {
                PlayerCurrentMana -= manaToUse;
                UpdateManaDisplay();
                OnManaChanged?.Invoke();
                return true;
            }
        }
        else if (InGameManagerScript.Instance.State == InGameState.EnemyTurn)
        {
            if (EnemyCurrentMana - manaToUse >= 0)
            {
                EnemyCurrentMana -= manaToUse;
                UpdateManaDisplay();
                OnManaChanged?.Invoke();
                return true;
            }
        }
        return false;
    }
    public bool CanUseMana(int manaToUse, PlayerType player)
    {
        if (player == PlayerType.player)
        {
            if (PlayerCurrentMana - manaToUse >= 0)
            {
                return true;
            }
        }
        else if (player == PlayerType.enemy)
        {
            if (EnemyCurrentMana - manaToUse >= 0)
            {
                return true;
            }
        }
        return false;
    }
    public bool CanUseMana(int manaToUse)
    {
        if (InGameManagerScript.Instance.State == InGameState.PlayerTurn)
        {
            if (PlayerCurrentMana - manaToUse >= 0)
            {
                return true;
            }
        }
        else if (InGameManagerScript.Instance.State == InGameState.EnemyTurn)
        {
            if (EnemyCurrentMana - manaToUse >= 0)
            {
                return true;
            }
        }
        return false;
    }
    public bool AddMaxMana(int manaToAdd, PlayerType player)
    {
        bool result = false;
        if (player == PlayerType.player)
        {
            if (PlayerMaxMana + manaToAdd > MaxPossibleMana)
            {
                PlayerMaxMana = MaxPossibleMana;
                result = false;
            }
            else
            {
                PlayerMaxMana += manaToAdd;
                result = true;
            }
        }
        else if (player == PlayerType.enemy)
        {
            if (EnemyMaxMana + manaToAdd > MaxPossibleMana)
            {
                EnemyMaxMana = MaxPossibleMana;
                result = false;
            }
            else
            {
                EnemyMaxMana += manaToAdd;
                result = true;
            }
        }
        UpdateManaDisplay();
        return result;
    }
    public bool AddMaxMana(int manaToAdd)
    {
        bool result = false;
        if (InGameManagerScript.Instance.State == InGameState.PlayerTurn)
        {
            if (PlayerMaxMana + manaToAdd > MaxPossibleMana)
            {
                PlayerMaxMana = MaxPossibleMana;
                result = false;
            }
            else
            {
                PlayerMaxMana += manaToAdd;
                result = true;
            }
        }
        else if (InGameManagerScript.Instance.State == InGameState.EnemyTurn)
        {
            if (EnemyMaxMana + manaToAdd > MaxPossibleMana)
            {
                EnemyMaxMana = MaxPossibleMana;
                result = false;
            }
            else
            {
                EnemyMaxMana += manaToAdd;
                result = true;
            }
        }
        UpdateManaDisplay();
        return result;
    }
    public void AddMana(int manaToAdd, PlayerType player)
    {
        if (player == PlayerType.player)
        {
            if (PlayerCurrentMana + manaToAdd > PlayerMaxMana)
            {
                PlayerCurrentMana = PlayerMaxMana;
            }
            else
            {
                PlayerCurrentMana += manaToAdd;
            }
        }
        else if (player == PlayerType.enemy)
        {
            if (EnemyCurrentMana + manaToAdd > MaxPossibleMana)
            {
                EnemyCurrentMana = EnemyMaxMana;
            }
            else
            {
                EnemyCurrentMana += manaToAdd;
            }
        }
        OnManaChanged?.Invoke();
        UpdateManaDisplay();
    }

    private void UpdateManaDisplay()
    {
        // Update player mana
        TMP_Text playerManaText = PlayerMana.transform.Find("ManaText").gameObject.GetComponent<TMP_Text>();
        playerManaText.text = PlayerCurrentMana + "/" + PlayerMaxMana;

        // Update enemy mana
        TMP_Text enemyManaText = EnemyMana.transform.Find("ManaText").gameObject.GetComponent<TMP_Text>();
        enemyManaText.text = EnemyCurrentMana + "/" + EnemyMaxMana;
    }
    public int GetCurrentPlayerMana()
    {
        if (InGameManagerScript.Instance.State == InGameState.PlayerTurn)
        {
            return PlayerCurrentMana;
        }
        else if (InGameManagerScript.Instance.State == InGameState.EnemyTurn)
        {
            return EnemyCurrentMana;
        }
        return -1;
    }
    public int GetCurrentPlayerMaxMana()
    {
        if (InGameManagerScript.Instance.State == InGameState.PlayerTurn)
        {
            return PlayerMaxMana;
        }
        else if (InGameManagerScript.Instance.State == InGameState.EnemyTurn)
        {
            return EnemyMaxMana;
        }
        return -1;
    }
}
