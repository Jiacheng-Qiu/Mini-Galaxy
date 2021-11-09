using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabRotate : MonoBehaviour
{
    private bool onDrag;
    private InterfaceAnimManager appearCheck;
    private Vector2 mousePos;

    private void Start()
    {
        onDrag = false;
        appearCheck = transform.parent.parent.GetComponent<InterfaceAnimManager>();
    }

    private void Update()
    {
        if (appearCheck.currentState == CSFHIAnimableState.appeared && Input.GetMouseButtonDown(0))
        {
            onDrag = true;
            mousePos = Input.mousePosition;
        } 
        else if (onDrag && Input.GetMouseButtonUp(0))
        {
            onDrag = false;
        }
    }

    [System.Obsolete]
    private void FixedUpdate()
    {
        // Only do animation and check when is active
        if (appearCheck.currentState != CSFHIAnimableState.appeared)
            return;
        // When planet is not dragged, auto rotate with slow motion
        if (onDrag)
        {
            Vector2 newMousePos = Input.mousePosition;
            transform.RotateAround(Vector3.up, (mousePos.x - newMousePos.x) / Mathf.Rad2Deg);
            transform.RotateAround(Vector3.right, -(mousePos.y - newMousePos.y) / Mathf.Rad2Deg);
            mousePos = newMousePos;
        }
    }
}
