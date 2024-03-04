using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


[ExecuteInEditMode()]
public class ToolTipScript : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public TextMeshProUGUI manaCostField;
    public GameObject manaCostGroup;
    public LayoutElement layoutElement;
    public int characterWrapLimit;
    public RectTransform rectTransform;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void SetText(string content, string header, string manaCost)
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
        if (string.IsNullOrEmpty(manaCost))
        {
            manaCostField.text = "";
            manaCostGroup.SetActive(false);
        }
        else
        {
            manaCostGroup.SetActive(true);
            manaCostField.text = manaCost;
        }
        Canvas.ForceUpdateCanvases();
        GetComponent<VerticalLayoutGroup>().enabled = false;
        GetComponent<VerticalLayoutGroup>().enabled = true;

        int headerLength = GetMaxLengthOfSingleLine(headerField.text);
        int contentLength = GetMaxLengthOfSingleLine(contentField.text);

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit);
    }

    // Utils
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
