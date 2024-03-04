using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PopupScript : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public int characterWrapLimit;
    public RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void ResetText()
    {
        headerField.text = "";
        contentField.text = "";
    }
    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.text = "";
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        if (string.IsNullOrEmpty(content))
        {
            contentField.text = "";
            contentField.gameObject.SetActive(false);
        }
        else
        {
            contentField.gameObject.SetActive(true);
            contentField.text = content;
        }
        Canvas.ForceUpdateCanvases();
        GetComponent<HorizontalLayoutGroup>().enabled = false;
        GetComponent<HorizontalLayoutGroup>().enabled = true;

        int headerLength = GetMaxLengthOfSingleLine(headerField.text);
        int contentLength = GetMaxLengthOfSingleLine(contentField.text);

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit);
    }

    // Utils
    public bool CheckIfSamePopup(string content, string header = "")
    {
        bool result = true;
        if (contentField.enabled)
        {
            if (!String.Equals(contentField.text, content))
            {
                result = false;
            }
        }
        if (headerField.enabled)
        {
            if (!String.Equals(headerField.text, header))
            {
                result = false;
            }
        }
        return result;
    }
    private int GetMaxLengthOfSingleLine(string text)
    {
        int result = 0;
        string[] lines = text.Split(
        new string[] { "\r\n", "\r", "\n" },
        StringSplitOptions.None
        );
        foreach (string l in lines)
        {
            if (l.Length > result)
            {
                result = l.Length;
            }
        }
        return result;
    }
}
