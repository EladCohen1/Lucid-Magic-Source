using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ReviveCompanionButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Game Variables")]
    public PlayerType Owner;
    public int shadowSize;
    private Vector2 upPos;
    private Vector2 downPos;

    [Header("Components")]
    private Button btn;
    private Shadow shadow;

    void Awake()
    {
        InGameManagerScript.OnInGameStateChanged += InGameManagerOnInGameStateChanged;
        ManaSystemManagerScript.OnManaChanged += ManaSystemManagerOnManaChanged;
        InGameManagerScript.OnCardDeath += InGameManagerOnCardDeath;
    }
    void OnDestroy()
    {
        InGameManagerScript.OnInGameStateChanged -= InGameManagerOnInGameStateChanged;
        ManaSystemManagerScript.OnManaChanged -= ManaSystemManagerOnManaChanged;
        InGameManagerScript.OnCardDeath -= InGameManagerOnCardDeath;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (btn.interactable)
        {
            shadow.enabled = false;
            transform.position = downPos;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (btn.interactable)
        {
            shadow.enabled = true;
            transform.position = upPos;
        }
    }
    void Start()
    {
        upPos = transform.position;
        downPos = transform.position;
        downPos.x += shadowSize;
        downPos.y -= shadowSize;

        btn = gameObject.GetComponent<Button>();
        shadow = gameObject.GetComponent<Shadow>();
        btn.onClick.AddListener(() =>
        {
            HeroicActionSystemManagerScript.Instance.isSearchingForReviveTarget = true;
            Vector2 hotSpot = new Vector2(GameManager.Instance.cursorReviveTargetIcon.width / 2f, GameManager.Instance.cursorReviveTargetIcon.height / 2f);
            Cursor.SetCursor(GameManager.Instance.cursorReviveTargetIcon, hotSpot, CursorMode.ForceSoftware);
            HeroicActionSystemManagerScript.Instance.SetReviveTarget(null);
            ButtonDown();
        });
    }

    void Update()
    {
        UpdateButtonState();
        if (HeroicActionSystemManagerScript.Instance.isSearchingForReviveTarget)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (InGameManagerScript.Instance.IsPlayerTypeTurn(Owner))
                {
                    HeroicActionSystemManagerScript.Instance.CancelReviveCompanion();
                }
            }
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
        if (Owner == PlayerType.player
        && ManaSystemManagerScript.Instance.PlayerMaxMana >= HeroicActionSystemManagerScript.Instance.reviveCompanionCost
        && InGameManagerScript.Instance.GetCurrentPlayerNumOfDeadCards() > 0)
        {
            ButtonUp();
        }
        else
        {
            ButtonDown();
        }
    }
    private void HandleEnemyTurnState()
    {
        if (Owner == PlayerType.enemy
        && ManaSystemManagerScript.Instance.EnemyMaxMana >= HeroicActionSystemManagerScript.Instance.reviveCompanionCost
        && InGameManagerScript.Instance.GetCurrentPlayerNumOfDeadCards() > 0)
        {
            ButtonUp();
        }
        else
        {
            ButtonDown();
        }
    }

    // ManaChanged
    private void ManaSystemManagerOnManaChanged()
    {
        UpdateButtonState();
    }

    // Card Death
    private void InGameManagerOnCardDeath()
    {
        UpdateButtonState();
    }
    // Utils
    public void ButtonDown()
    {
        btn.interactable = false;
        shadow.enabled = false;
        transform.position = downPos;
    }
    public void ButtonUp()
    {
        btn.interactable = true;
        shadow.enabled = true;
        transform.position = upPos;
    }
    public void UpdateButtonState()
    {
        if ((Owner == PlayerType.player && InGameManagerScript.Instance.State == InGameState.PlayerTurn)
        || (Owner == PlayerType.enemy && InGameManagerScript.Instance.State == InGameState.EnemyTurn))
        {
            if (HeroicActionSystemManagerScript.Instance.reviveCompanionUsedThisTurn
            || ManaSystemManagerScript.Instance.GetCurrentPlayerMana() < HeroicActionSystemManagerScript.Instance.reviveCompanionCost
            || InGameManagerScript.Instance.GetCurrentPlayerNumOfDeadCards() <= 0
            || HeroicActionSystemManagerScript.Instance.isSearchingForReviveTarget == true)
            {
                ButtonDown();
            }
            else
            {
                ButtonUp();
            }
        }
    }
}
