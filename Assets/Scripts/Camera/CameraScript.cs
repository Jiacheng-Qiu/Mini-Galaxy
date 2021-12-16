using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    void Start()
    {
        transform.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

}
