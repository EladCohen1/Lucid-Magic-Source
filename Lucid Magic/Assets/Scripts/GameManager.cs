using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Static And State")]
    public GameState State;
    public static event Action<GameState> OnGameStateChanged;

    [Header("Popup Warnings")]
    [TextArea]
    public string decksNotChosenWarningHeader;
    [TextArea]
    public string decksNotChosenWarningContent;

    [Header("Game Variables")]
    public GameOutCome gameOutCome;
    public Texture2D cursorDefault;
    public Texture2D cursorDefaultOrange;
    public Texture2D cursorDefaultBlue;
    public Texture2D cursorPurpleTargetIcon;
    public Texture2D cursorRedTargetIcon;
    public Texture2D cursorReviveTargetIcon;

    [Header("Post Game")]
    public Image PostGameBackground;
    public TMP_Text EndScreenHeader;

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
        OnGameStateChanged += GameManagerOnGameStateChanged;
    }
    void OnDestroy()
    {
        OnGameStateChanged -= GameManagerOnGameStateChanged;
    }
    void Start()
    {
        Cursor.SetCursor(GameManager.Instance.cursorDefault, Vector2.zero, CursorMode.ForceSoftware);
        InitiatePreGame();
    }

    // Start game
    public void StartGame()
    {
        if (CheckStartGameEligibility())
        {
            UpdateGameState(GameState.InGame);
        }
        else
        {
            PopupManagerScript.TriggerPopup?.Invoke(decksNotChosenWarningContent, decksNotChosenWarningHeader);
        }
    }
    private bool CheckStartGameEligibility()
    {
        foreach (GameObject card in PreGameManagerScript.Instance.selectedPlayerCards)
        {
            if (card == null)
            {
                return false;
            }
        }
        foreach (GameObject card in PreGameManagerScript.Instance.selectedEnemyCards)
        {
            if (card == null)
            {
                return false;
            }
        }
        return true;
    }

    // Go to pregame
    public void InitiatePreGame()
    {
        UpdateGameState(GameState.PreGame);
    }

    // Quit Game
    public void QuitGame()
    {
        Application.Quit();
    }

    // Game state change
    public void UpdateGameState(GameState newState)
    {
        State = newState;
        OnGameStateChanged?.Invoke(newState);
    }
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
        Cursor.SetCursor(GameManager.Instance.cursorDefault, Vector2.zero, CursorMode.ForceSoftware);
        PostGameBackground.gameObject.SetActive(false);
        PauseSystemManager.Instance.CanPause = true;
    }
    private void HandleInGameState()
    {
        PostGameBackground.gameObject.SetActive(false);
        PauseSystemManager.Instance.CanPause = true;
    }
    private void HandlePostGameState()
    {
        if (gameOutCome == GameOutCome.PlayerWon)
        {
            EndScreenHeader.text = "Player 1 Won!";
        }
        else if (gameOutCome == GameOutCome.EnemyWon)
        {
            EndScreenHeader.text = "Player 2 Won!";
        }
        else if (gameOutCome == GameOutCome.Draw)
        {
            EndScreenHeader.text = "Draw!";
        }
        PostGameBackground.gameObject.SetActive(true);
        PauseSystemManager.Instance.CanPause = false;
        BattleFieldManagerScript.Instance.UpdateCursorBasedOnBattleField();
    }
}