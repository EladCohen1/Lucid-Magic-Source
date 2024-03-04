using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSelectionScript : MonoBehaviour, IPointerClickHandler
{
    GameObject outline;
    public Color SelectHeroColor;
    public Color SelectCompanionColor;

    void Awake()
    {
        outline = gameObject.transform.GetChild(0).gameObject;
    }
    public void SelectCard()
    {
        int result = PreGameManagerScript.Instance.SelectCard(gameObject);
        switch (result)
        {
            case 1:
                outline.GetComponent<Image>().color = SelectHeroColor;
                outline.SetActive(true);
                break;
            case 2:
                outline.GetComponent<Image>().color = SelectCompanionColor;
                outline.SetActive(true);
                break;
            default:
                outline.SetActive(false);
                break;
        }
    }

    void OnDisable()
    {
        outline.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SelectCard();
        }
    }
}
