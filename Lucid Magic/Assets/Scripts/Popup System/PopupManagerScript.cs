using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PopupManagerScript : MonoBehaviour
{
    public static Action<string, string> TriggerPopup;
    public PopupScript Popup;
    public CanvasGroup PopupGroup;
    void OnEnable()
    {
        TriggerPopup += ActivatePopup;
    }
    void OnDisable()
    {
        TriggerPopup -= ActivatePopup;
    }

    public void ActivatePopup(string content, string header = "")
    {
        if (Popup.CheckIfSamePopup(content, header))
        {
            return;
        }
        LeanTween.cancel(Popup.gameObject);
        PopupGroup.alpha = 0;
        Popup.gameObject.SetActive(true);
        Popup.SetText(content, header);

        // Fade in and out animation

        PopupGroup.LeanAlpha(1, 0.5f).setOnComplete(() =>
        {
            PopupGroup.LeanAlpha(1, 2).setOnComplete(() =>
            {
                PopupGroup.LeanAlpha(0, 0.5f).setOnComplete(() =>
                {
                    Popup.gameObject.SetActive(false);
                    Popup.ResetText();
                });
            });
        });
    }
}
