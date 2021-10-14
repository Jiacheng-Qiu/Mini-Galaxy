using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionAnimation : MonoBehaviour
{
    // Contains animations while player status change
    public Camera cam;
    private float lastHurt;
    public GameObject helmet;

    private bool helmetOn;
    private bool onHelmetAnimation;
    private float helmetX;

    // Animation on core UI stats
    public Image shield;
    public Image baseShield;
    public Image health;
    public Image oxygen;
    public InterfaceAnimManager warning;

    private float cachedShieldDmg;

    private void Start()
    {
        cachedShieldDmg = 0;
        helmetOn = true;
        onHelmetAnimation = false;
        lastHurt = 0;
        helmetX = 0;
        warning.gameObject.SetActive(false);
    }

    // Warning animation that wakes hiden ui till canceled
    public void Warning(bool on)
    {
        if (on && !warning.gameObject.active)
        {
            warning.gameObject.SetActive(true);
            warning.startAppear();
        } else if (!on && warning.gameObject.active)
        {
            warning.startDisappear();
        }
    }

    // color and size change based on current health
    public void NewHealth(float amount)
    {
        if (amount == 0)
            return;
        health.color = Color.HSVToRGB(amount / 3.6f, 0.8f, 1);
        health.rectTransform.sizeDelta = new Vector2(7.5f, 5f * amount);
    }

    // same as above, animation for shield damage
    public void ShieldChange(float amount)
    {
        if (amount == 0)
            return;
        // Instant damage on shield with continuous animation for 1s
        else if (amount < 0)
        {
            shield.fillAmount += amount;
            cachedShieldDmg -= amount;
        } else
        {
            cachedShieldDmg = 0;
            shield.fillAmount += amount;
            baseShield.fillAmount = shield.fillAmount;
        }
    }

    // Can only be called once 1.5s
    public void HurtAnimation()
    {
        if (Time.time > lastHurt + 1.5f)
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
        // Helmet open/close animation
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

        // Reduce the cached base shield if it is larger than 0
        if (cachedShieldDmg > 0)
        {
            float shiftAmount = (cachedShieldDmg > 0.2f) ? cachedShieldDmg * 3 : 0.5f; 
            baseShield.fillAmount -= shiftAmount * Time.deltaTime;
            cachedShieldDmg -= shiftAmount * Time.deltaTime;
            if (cachedShieldDmg < 0)
            {
                cachedShieldDmg = 0;
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
