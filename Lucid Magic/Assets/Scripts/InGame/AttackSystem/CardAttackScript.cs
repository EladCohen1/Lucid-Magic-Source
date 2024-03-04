using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardAttackScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Event triggers")]
    private EventTrigger eventTrigger;
    private EventTrigger.Entry entryBeginDrag;
    private EventTrigger.Entry entryEndDrag;
    private EventTrigger.Entry entryDrop;

    [Header("Objects")]
    public GameObject Outline;
    public GameObject HoverOutline;
    public Color canAttackColor;

    [Header("Scripts")]
    CardManagerScript cardManagerScript;

    // Game Variables
    private bool alreadyAttacked = false;
    public bool AlreadyAttacked
    {
        get { return alreadyAttacked; }
        set
        {
            alreadyAttacked = value;
            UpdateCanAttackDisplay();
        }
    }

    void Awake()
    {
        cardManagerScript = gameObject.GetComponent<CardManagerScript>();
    }

    void OnEnable()
    {
        AddDragDropEvents();
        InGameManagerScript.OnInGameStateChanged += InGameManagerOnInGameStateChanged;
        ManaSystemManagerScript.OnManaChanged += ManaSystemManagerScriptOnManaChanged;
        UpdateCanAttackDisplay();
    }
    void OnDisable()
    {
        RemoveDragDropEvents();
        InGameManagerScript.OnInGameStateChanged -= InGameManagerOnInGameStateChanged;
        ManaSystemManagerScript.OnManaChanged -= ManaSystemManagerScriptOnManaChanged;
    }

    private void AddDragDropEvents()
    {
        // begin drag
        eventTrigger = GetComponent<EventTrigger>();
        entryBeginDrag = new EventTrigger.Entry();
        entryBeginDrag.eventID = EventTriggerType.BeginDrag;
        entryBeginDrag.callback.AddListener((data) => { OnBeginDragDelegate((PointerEventData)data); });
        eventTrigger.triggers.Add(entryBeginDrag);

        // end drag
        entryEndDrag = new EventTrigger.Entry();
        entryEndDrag.eventID = EventTriggerType.EndDrag;
        entryEndDrag.callback.AddListener((data) => { OnEndDragDelegate((PointerEventData)data); });
        eventTrigger.triggers.Add(entryEndDrag);

        // drop
        entryDrop = new EventTrigger.Entry();
        entryDrop.eventID = EventTriggerType.Drop;
        entryDrop.callback.AddListener((data) => { OnDropDelegate((PointerEventData)data); });
        eventTrigger.triggers.Add(entryDrop);
    }

    private void RemoveDragDropEvents()
    {
        if (entryBeginDrag != null)
        {
            eventTrigger.triggers.Remove(entryBeginDrag);
        }
        if (entryDrop != null)
        {
            eventTrigger.triggers.Remove(entryDrop);
        }
        if (entryEndDrag != null)
        {
            eventTrigger.triggers.Remove(entryEndDrag);
        }
    }

    private void OnBeginDragDelegate(PointerEventData data)
    {
        AttackSystemManagerScript.Instance.SetAttacker(gameObject);
    }

    private void OnDropDelegate(PointerEventData data)
    {
        AttackSystemManagerScript.Instance.SetDefender(gameObject);
    }
    private void OnEndDragDelegate(PointerEventData data)
    {
        AttackSystemManagerScript.Instance.SetAttacker(null);
    }
    private void InGameManagerOnInGameStateChanged(InGameState state)
    {
        switch (state)
        {
            case InGameState.PlayerTurn:
                AlreadyAttacked = false;
                break;
            case InGameState.EnemyTurn:
                AlreadyAttacked = false;
                break;
            default:
                break;
        }
    }

    // ManaSystem OnManaChanged
    private void ManaSystemManagerScriptOnManaChanged()
    {
        UpdateCanAttackDisplay();
    }

    // Utils
    public void UpdateCanAttackDisplay()
    {
        if (AttackSystemManagerScript.Instance.CheckAttackerEligibility(gameObject))
        {
            Outline.GetComponent<Image>().color = canAttackColor;
            Outline.SetActive(true);
        }
        else
        {
            Outline.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AttackSystemManagerScript.Instance.Attacker != null)
        {
            if (AttackSystemManagerScript.Instance.CheckDefenderEligibility(gameObject))
            {
                HoverOutline.GetComponent<Image>().color = gameObject.GetComponent<CardAbilityScript>().targetColor;
                HoverOutline.SetActive(true);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverOutline.SetActive(false);
    }
}
