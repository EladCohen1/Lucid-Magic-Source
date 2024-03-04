// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class DragDropScript : MonoBehaviour
// {
//     public GameObject mainCanvas;
//     private bool isDragging = false;
//     private Vector2 distanceFromMouse = Vector2.zero;
//     void Start()
//     {
//         mainCanvas = GameObject.Find("Main Canvas");
//     }
//     void Update()
//     {
//         if (isDragging)
//         {
//             transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) + distanceFromMouse;
//         }
//     }

//     public void begingDrag()
//     {
//         distanceFromMouse = new Vector2(transform.position.x - Input.mousePosition.x, transform.position.y - Input.mousePosition.y);
//         isDragging = true;
//         gameObject.transform.SetParent(mainCanvas.transform, true);
//     }

//     public void endDrag()
//     {
//         isDragging = false;
//     }
// }
