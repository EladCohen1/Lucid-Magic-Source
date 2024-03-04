using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTipTriggerReviveCompanionScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Fields When Revive Companion Usable")]
    [TextArea]
    public string headerReviveCompanionUsable;
    [TextArea]
    public string contentReviveCompanionUsable;

    [Header("Fields When Not Enough Mana")]
    [TextArea]
    public string headerReviveCompanionNotEnoughMana;
    [TextArea]
    public string contentReviveCompanionNotEnoughMana;

    [Header("Fields When Revive Companion Already Used This Turn")]
    [TextArea]
    public string headerReviveCompanionUnusable;
    [TextArea]
    public string contentReviveCompanionUnusable;

    [Header("Fields When Player Has No Dead Cards")]
    [TextArea]
    public string headerReviveCompanionNoDeadCards;
    [TextArea]
    public string contentReviveCompanionNoDeadCards;

    [Header("Fields When Not On Turn")]
    [TextArea]
    public string headerNotOnTurn;
    [TextArea]
    public string contentNotOnTurn;

    [Header("Fields When Searching For Revive Target")]
    [TextArea]
    public string headerSearchingForReviveTarget;
    [TextArea]
    public string contentSearchingForReviveTarget;

    [Header("Components")]
    private Button btn;
    private ReviveCompanionButtonScript reviveCompanionButtonScript;

    void Start()
    {
        btn = gameObject.GetComponent<Button>();
        reviveCompanionButtonScript = gameObject.GetComponent<ReviveCompanionButtonScript>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (btn.interactable)
        {
            ToolTipManagerScript.Instance.ShowToolTip(contentReviveCompanionUsable, headerReviveCompanionUsable, HeroicActionSystemManagerScript.Instance.reviveCompanionCost.ToString());
        }
        else
        {
            if ((reviveCompanionButtonScript.Owner == PlayerType.player && InGameManagerScript.Instance.State == InGameState.PlayerTurn)
            || reviveCompanionButtonScript.Owner == PlayerType.enemy && InGameManagerScript.Instance.State == InGameState.EnemyTurn)
            {
                if (HeroicActionSystemManagerScript.Instance.reviveCompanionUsedThisTurn)
                {
                    ToolTipManagerScript.Instance.ShowToolTip(contentReviveCompanionUnusable, headerReviveCompanionUnusable);
                }
                else
                {
                    if (InGameManagerScript.Instance.GetCurrentPlayerNumOfDeadCards() <= 0)
                    {
                        ToolTipManagerScript.Instance.ShowToolTip(contentReviveCompanionNoDeadCards, headerReviveCompanionNoDeadCards);
                    }
                    else if (ManaSystemManagerScript.Instance.GetCurrentPlayerMana() < HeroicActionSystemManagerScript.Instance.reviveCompanionCost)
                    {
                        ToolTipManagerScript.Instance.ShowToolTip(contentReviveCompanionNotEnoughMana, headerReviveCompanionNotEnoughMana);
                    }
                    else if (HeroicActionSystemManagerScript.Instance.isSearchingForReviveTarget)
                    {
                        ToolTipManagerScript.Instance.ShowToolTip(contentSearchingForReviveTarget, headerSearchingForReviveTarget);
                    }
                }
            }
            else
            {
                ToolTipManagerScript.Instance.ShowToolTip(contentNotOnTurn, headerNotOnTurn);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManagerScript.OnMouseExit?.Invoke();
    }
}
