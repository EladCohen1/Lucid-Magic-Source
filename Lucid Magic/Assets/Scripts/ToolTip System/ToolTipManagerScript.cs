using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ToolTipManagerScript : MonoBehaviour
{
    public static ToolTipManagerScript Instance { get; private set; }
    public static Action OnMouseExit;
    public ToolTipScript toolTip;
    public RectTransform toolTipRectTransform;
    public float pivotYBottom = -0.02f;
    public float pivotYTop = 1.2f;
    public float toolTipDelay = 0.2f;

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
        OnMouseExit += HideToolTip;
    }

    void OnDisable()
    {
        OnMouseExit -= HideToolTip;
        StopAllCoroutines();
    }

    void Start()
    {
        toolTipRectTransform = toolTip.gameObject.GetComponent<RectTransform>();
    }

    public void ShowToolTip(string content, string header = "", string manaCost = "")
    {
        toolTip.SetText(content, header, manaCost);
        StartCoroutine(StartToolTipDelay());
    }

    private void HideToolTip()
    {
        toolTip.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    private IEnumerator StartToolTipDelay()
    {
        StartCoroutine(ToolTipFollowMouse());
        yield return new WaitForSeconds(toolTipDelay);
        toolTip.gameObject.SetActive(true);
    }

    private IEnumerator ToolTipFollowMouse()
    {
        while (true)
        {
            Vector2 position = Input.mousePosition;
            float pivotX = position.x / Screen.width;
            float pivotY = position.y / Screen.height;
            if (pivotY < 0.5)
            {
                pivotY = pivotYBottom;
            }
            else
            {
                pivotY = pivotYTop;
            }
            toolTipRectTransform.pivot = new Vector2(pivotX, pivotY);
            toolTipRectTransform.position = position;
            yield return new WaitForEndOfFrame();
        }
    }
}
