// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;

// public class ToolTipManagerScript : MonoBehaviour
// {
//     public static Action<string> OnMouseHover;
//     public static Action OnMouseExit;
//     public TextMeshProUGUI tipText;
//     public RectTransform tipWindow;
//     private void OnEnable()
//     {
//         ToolTipManagerScript.OnMouseHover += ShowTip;
//         ToolTipManagerScript.OnMouseExit += HideTip;
//     }
//     private void OnDisable()
//     {
//         ToolTipManagerScript.OnMouseHover -= ShowTip;
//         ToolTipManagerScript.OnMouseExit -= HideTip;
//     }
//     void Start()
//     {
//         HideTip();
//     }

//     private void ShowTip(string tip)
//     {
//         tipText.text = tip;
//         tipWindow.sizeDelta = new Vector2(tipText.preferredWidth > 350 ? 350 : tipText.preferredWidth, tipText.preferredHeight);
//         tipWindow.gameObject.SetActive(true);
//         StartCoroutine(FollowMouse());
//     }
//     private void HideTip()
//     {
//         tipText.text = default;
//         tipWindow.gameObject.SetActive(false);
//         StopAllCoroutines();
//     }

//     private IEnumerator FollowMouse()
//     {
//         while (true)
//         {
//             //tipWindow.transform.position = new Vector2(Input.mousePosition.x + tipWindow.sizeDelta.x / 2, Input.mousePosition.y + tipWindow.sizeDelta.y / 2);
//             float pivotX = Input.mousePosition.x / Screen.width;
//             float pivotY = Input.mousePosition.y / Screen.height;
//             tipWindow.pivot = new Vector2(pivotX, pivotY);
//             tipWindow.transform.position = Input.mousePosition;
//             yield return new WaitForEndOfFrame();
//         }
//     }
// }
