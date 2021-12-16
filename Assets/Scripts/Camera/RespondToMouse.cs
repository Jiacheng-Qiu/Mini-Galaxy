using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Camera do tiny rotation based on mouse screen position
public class RespondToMouse : MonoBehaviour
{
    public float xRange = 3;
    public float yRange = 5;

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Debug.Log(mousePos);
        transform.eulerAngles = new Vector3(- (mousePos.y - 540) * yRange / 1080, (mousePos.x - 960) * xRange / 1920, 0);
    }
}
