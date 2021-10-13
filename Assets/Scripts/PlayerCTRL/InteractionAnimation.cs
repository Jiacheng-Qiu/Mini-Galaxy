using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionAnimation : MonoBehaviour
{
    // Contains animations while player status change
    public Camera cam;
    private float lastHurt;
    public GameObject helmet;

    private bool helmetOn;
    private bool onHelmetAnimation;
    private float helmetX;

    private void Start()
    {
        helmetOn = true;
        onHelmetAnimation = false;
        lastHurt = 0;
        helmetX = 0;
    }

    // Can only be called once 2s
    public void HurtAnimation()
    {
        if (Time.time > lastHurt + 2f)
        {
            StartCoroutine(CamShake(0.1f, 0.2f));
            lastHurt = Time.time;
        }
    }

    public void LowHPAnimation()
    {
        // TODO: make game borders red
    }

    public void HelmetAnimation()
    {
        if (!onHelmetAnimation)
        {
            onHelmetAnimation = true;
        }
    }

    // Move helmet up or down
    private void FixedUpdate()
    {
        if (onHelmetAnimation)
        {
            if (helmetOn)
            {
                if (helmetX > -90)
                {
                    helmetX -= 1f;
                    helmet.transform.localRotation = Quaternion.Euler(helmetX, 0, 0);
                }
                else
                {
                    helmetOn = false;
                    onHelmetAnimation = false;
                }
            }
            else
            {
                if (helmetX < 0)
                {
                    helmetX += 1f;
                    helmet.transform.localRotation = Quaternion.Euler(helmetX, 0, 0);
                }
                else
                {
                    helmetOn = true;
                    onHelmetAnimation = false;
                }
            }
        }
    }


    private IEnumerator CamShake(float duration, float magnitude)
    {
        Vector3 origPos = cam.transform.localPosition;
        float elapsed = 0f;
        while(elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            cam.transform.localPosition = new Vector3(x, y, origPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.transform.localPosition = origPos;
    }
}
