using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTriggerScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string header;
    [TextArea]
    public string content;
    public string manaCost;
    public int multiplier = 1;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (multiplier > 1)
        {
            ToolTipManagerScript.Instance.ShowToolTip(content, header + " x" + multiplier, manaCost);
        }
        else
        {
            ToolTipManagerScript.Instance.ShowToolTip(content, header, manaCost);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManagerScript.OnMouseExit?.Invoke();
    }
}
