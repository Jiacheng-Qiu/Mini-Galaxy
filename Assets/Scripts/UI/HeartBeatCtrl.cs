using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeatCtrl : MonoBehaviour
{

    private void FixedUpdate()
    {
        transform.localPosition -= new Vector3(4 * Time.deltaTime, 0, 0);
        if (transform.localPosition.x <= -4.1)
        {
            Destroy(gameObject);
        }
    }
}
