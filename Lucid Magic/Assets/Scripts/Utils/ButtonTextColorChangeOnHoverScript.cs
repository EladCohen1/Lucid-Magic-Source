using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ButtonTextColorChangeOnHoverScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Text ButtonText;
    public Color TextColor;
    public Color TextColorHover;
    void Awake()
    {
        ButtonText = transform.Find("ButtonText").gameObject.GetComponent<TMP_Text>();
        gameObject.GetComponent<Button>().onClick.AddListener(ResetTextColor);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ButtonText.color = TextColorHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ButtonText.color = TextColor;
    }

    public void ResetTextColor()
    {
        ButtonText.color = TextColor;
    }
}
