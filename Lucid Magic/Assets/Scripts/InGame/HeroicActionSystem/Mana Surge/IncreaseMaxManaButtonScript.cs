using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IncreaseMaxManaButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
    }
    void OnDestroy()
    {
        InGameManagerScript.OnInGameStateChanged -= InGameManagerOnInGameStateChanged;
        ManaSystemManagerScript.OnManaChanged -= ManaSystemManagerOnManaChanged;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // check for other actions taking place
        if (!InGameManagerScript.Instance.IsUsingAnotherAction())
        {
            if (btn.interactable)
            {
                shadow.enabled = false;
                transform.position = downPos;
            }
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
            bool manaSurgeUsedThisTurn = HeroicActionSystemManagerScript.Instance.UseManaSurge(Owner);
            UpdateButtonState();
        });
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
        if (Owner == PlayerType.player)
        {
            if (ManaSystemManagerScript.Instance.PlayerMaxMana < ManaSystemManagerScript.Instance.MaxPossibleMana)
            {
                ButtonUp();
            }
        }
        else
        {
            ButtonDown();
        }
    }
    private void HandleEnemyTurnState()
    {
        if (Owner == PlayerType.enemy)
        {
            if (ManaSystemManagerScript.Instance.EnemyMaxMana < ManaSystemManagerScript.Instance.MaxPossibleMana)
            {
                ButtonUp();
            }
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
    private void UpdateButtonState()
    {
        if ((Owner == PlayerType.player && InGameManagerScript.Instance.State == InGameState.PlayerTurn)
        || (Owner == PlayerType.enemy && InGameManagerScript.Instance.State == InGameState.EnemyTurn))
        {
            if (HeroicActionSystemManagerScript.Instance.manaSurgeUsedThisTurn
            || ManaSystemManagerScript.Instance.GetCurrentPlayerMana() < HeroicActionSystemManagerScript.Instance.manaSurgeCost
            || ManaSystemManagerScript.Instance.GetCurrentPlayerMaxMana() >= ManaSystemManagerScript.Instance.MaxPossibleMana)
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
