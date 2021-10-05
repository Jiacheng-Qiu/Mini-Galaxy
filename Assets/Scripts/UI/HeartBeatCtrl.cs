using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeatCtrl : MonoBehaviour
{

    public void Init(float size)
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(60, size);
        gameObject.GetComponent<RectTransform>().localPosition = new Vector3(130, -0.5f, 0);
    }

    void FixedUpdate()
    {
        if(transform.localPosition.x <= -130)
        {
            Destroy(gameObject);
        } else
        {
            transform.localPosition -= new Vector3(2, 0, 0);
        }
    }
}
