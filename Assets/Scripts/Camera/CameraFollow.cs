using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Transform playerCam;

    private void FixedUpdate()
    {
        Vector3 euler = player.transform.localRotation.eulerAngles;
        transform.eulerAngles = new Vector3(euler.x + playerCam.localEulerAngles.x, euler.y, euler.z);
    }
}
