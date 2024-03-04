using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleFieldManagerScript : MonoBehaviour
{
    public static BattleFieldManagerScript Instance;

    [Header("BattleFields")]
    public Image Nightmare;
    public Image Mountains;
    public Image Forest;

    // Game Variables
    public int TurnCount { get; private set; } // count turns when turns end, 2 turn cycles would happen after 4 turn ends (player>enemy>player>enemy>now)
    public BattleField CurrentBattleField { get; private set; }

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
        TurnCount = -1; // we ignore the initial turn pass to the player (otherwise 2 cycles would end after the player's 2nd turn which is wrong)
        CurrentBattleField = BattleField.Mountains;
        UpdateBattleFieldDisplay();
        InGameManagerScript.OnInGameStateChanged += InGameManagerOnInGameStateChanged;
    }
    void OnDisable()
    {
        InGameManagerScript.OnInGameStateChanged -= InGameManagerOnInGameStateChanged;
        CurrentBattleField = BattleField.Mountains;
        if (Nightmare != null && Mountains != null && Forest != null)
        {
            UpdateBattleFieldDisplay();
        }
    }

    // Turn change event
    private void InGameManagerOnInGameStateChanged(InGameState state)
    {
        switch (state)
        {
            case InGameState.PlayerTurn:
                TurnCount++;
                HandleBattleFieldEffect();
                HandleTurnState();
                break;
            case InGameState.EnemyTurn:
                TurnCount++;
                HandleBattleFieldEffect();
                HandleTurnState();
                break;
            default:
                break;
        }
    }
    private void HandleTurnState()
    {
        if (TurnCount >= 4)
        {
            RollNewBattleField();
            UpdateBattleFieldDisplay();
            TurnCount = 0;
        }
    }
    private void HandleBattleFieldEffect()
    {
        if (CurrentBattleField == BattleField.Nightmare)
        {
            if (TurnCount == 2 || TurnCount == 4)
            {
                HandleNightmareEffect();
            }
        }
    }

    // Utils
    private void RollNewBattleField()
    {
        int dieResult = Random.Range(1, 7);
        if (CurrentBattleField == BattleField.Mountains)
        {
            if (dieResult < 5)
            {
                CurrentBattleField = BattleField.Forest;
            }
            else
            {
                CurrentBattleField = BattleField.Nightmare;
            }
        }
        else if (CurrentBattleField == BattleField.Forest)
        {
            if (dieResult < 5)
            {
                CurrentBattleField = BattleField.Mountains;
            }
            else
            {
                CurrentBattleField = BattleField.Nightmare;
            }
        }
        else if (CurrentBattleField == BattleField.Nightmare)
        {
            if (dieResult < 4)
            {
                CurrentBattleField = BattleField.Mountains;
            }
            else
            {
                CurrentBattleField = BattleField.Forest;
            }
        }
    }
    private void UpdateBattleFieldDisplay()
    {
        if (CurrentBattleField == BattleField.Mountains)
        {
            Mountains.gameObject.SetActive(true);
            Forest.gameObject.SetActive(false);
            Nightmare.gameObject.SetActive(false);
        }
        else if (CurrentBattleField == BattleField.Forest)
        {
            Mountains.gameObject.SetActive(false);
            Forest.gameObject.SetActive(true);
            Nightmare.gameObject.SetActive(false);
        }
        else if (CurrentBattleField == BattleField.Nightmare)
        {
            Mountains.gameObject.SetActive(false);
            Forest.gameObject.SetActive(false);
            Nightmare.gameObject.SetActive(true);
        }
        UpdateCursorBasedOnBattleField();
    }
    public void UpdateCursorBasedOnBattleField()
    {
        if (CurrentBattleField == BattleField.Mountains)
        {
            if (GameManager.Instance != null)
            {
                Cursor.SetCursor(GameManager.Instance.cursorDefault, Vector2.zero, CursorMode.ForceSoftware);
            }
        }
        else if (CurrentBattleField == BattleField.Forest)
        {
            Cursor.SetCursor(GameManager.Instance.cursorDefaultOrange, Vector2.zero, CursorMode.ForceSoftware);
        }
        else if (CurrentBattleField == BattleField.Nightmare)
        {
            Cursor.SetCursor(GameManager.Instance.cursorDefaultOrange, Vector2.zero, CursorMode.ForceSoftware);
        }
    }
    private void HandleNightmareEffect()
    {
        foreach (GameObject card in PreGameManagerScript.Instance.instantiatedCards)
        {
            CardManagerScript cardManager = card.GetComponent<CardManagerScript>();
            if (!cardManager.IsDead)
            {
                cardManager.ReduceCurrentHealth(1);
            }
        }
    }
}