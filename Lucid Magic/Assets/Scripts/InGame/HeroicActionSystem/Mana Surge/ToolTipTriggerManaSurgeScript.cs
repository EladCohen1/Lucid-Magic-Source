using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTipTriggerManaSurgeScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Fields When Mana Surge Usable")]
    [TextArea]
    public string headerManaSurgeUsable;
    [TextArea]
    public string contentManaSurgeUsable;

    [Header("Fields When Not Enough Mana")]
    [TextArea]
    public string headerManaSurgeNotEnoughMana;
    [TextArea]
    public string contentManaSurgeNotEnoughMana;

    [Header("Fields When Mana Surge Already Used This Turn")]
    [TextArea]
    public string headerManaSurgeUnusable;
    [TextArea]
    public string contentManaSurgeUnusable;

    [Header("Fields When Max Mana Reached")]
    [TextArea]
    public string headerMaxreached;
    [TextArea]
    public string contentMaxReached;

    [Header("Fields When Not On Turn")]
    [TextArea]
    public string headerNotOnTurn;
    [TextArea]
    public string contentNotOnTurn;

    [Header("Components")]
    private Button btn;
    private IncreaseMaxManaButtonScript increaseMaxManaButtonScript;

    void Start()
    {
        btn = gameObject.GetComponent<Button>();
        increaseMaxManaButtonScript = gameObject.GetComponent<IncreaseMaxManaButtonScript>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (btn.interactable)
        {
            ToolTipManagerScript.Instance.ShowToolTip(contentManaSurgeUsable, headerManaSurgeUsable, HeroicActionSystemManagerScript.Instance.manaSurgeCost.ToString());
        }
        else
        {
            if ((increaseMaxManaButtonScript.Owner == PlayerType.player && InGameManagerScript.Instance.State == InGameState.PlayerTurn)
            || increaseMaxManaButtonScript.Owner == PlayerType.enemy && InGameManagerScript.Instance.State == InGameState.EnemyTurn)
            {
                if (HeroicActionSystemManagerScript.Instance.manaSurgeUsedThisTurn)
                {
                    ToolTipManagerScript.Instance.ShowToolTip(contentManaSurgeUnusable, headerManaSurgeUnusable);
                }
                else
                {
                    if (ManaSystemManagerScript.Instance.GetCurrentPlayerMaxMana() >= ManaSystemManagerScript.Instance.MaxPossibleMana)
                    {
                        ToolTipManagerScript.Instance.ShowToolTip(contentMaxReached, headerMaxreached);
                    }
                    else if (ManaSystemManagerScript.Instance.GetCurrentPlayerMana() < HeroicActionSystemManagerScript.Instance.manaSurgeCost)
                    {
                        ToolTipManagerScript.Instance.ShowToolTip(contentManaSurgeNotEnoughMana, headerManaSurgeNotEnoughMana);
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
