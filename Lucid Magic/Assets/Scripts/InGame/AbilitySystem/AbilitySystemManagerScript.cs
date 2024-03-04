using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilitySystemManagerScript : MonoBehaviour
{
    public static AbilitySystemManagerScript Instance { get; private set; }
    public static Action EndHover;

    [Header("Game Variables")]
    public AbilityType ActiveAbility;
    public GameObject User;
    public GameObject Target;

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
        ActiveAbility = AbilityType.noAbility;
        User = null;
        Target = null;
    }
    void OnDisable()
    {

    }
    void Update()
    {
        if (ActiveAbility != AbilityType.noAbility)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                ResetSelection();
            }
        }
    }

    // public functions
    public void SetAbility(GameObject userCard, AbilityType ability)
    {
        if (ActiveAbility == AbilityType.noAbility)
        {
            if (ability.CheckUserEligibility(userCard))
            {
                Vector2 hotSpot = new Vector2(GameManager.Instance.cursorPurpleTargetIcon.width / 2f, GameManager.Instance.cursorPurpleTargetIcon.height / 2f);
                Cursor.SetCursor(GameManager.Instance.cursorPurpleTargetIcon, hotSpot, CursorMode.ForceSoftware);
                ActiveAbility = ability;
                User = userCard;
            }
        }
    }
    public void SetTarget(GameObject targetCard)
    {
        if (ActiveAbility != AbilityType.noAbility && User != null)
        {
            if (ActiveAbility.CheckTargetEligibility(User, targetCard))
            {
                Target = targetCard;
                ActiveAbility.UseAbility(User, Target);
                ResetSelection();
            }
        }
    }

    // Utils
    public void ResetSelection()
    {
        BattleFieldManagerScript.Instance.UpdateCursorBasedOnBattleField();
        User = null;
        Target = null;
        ActiveAbility = AbilityType.noAbility;
        EndHover?.Invoke();
    }
    public void InvokeEndHover()
    {
        EndHover?.Invoke();
    }
}