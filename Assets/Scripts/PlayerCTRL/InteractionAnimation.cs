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
    public Image oxygenBar;
    public Text oxygenAmount;
    public InterfaceAnimManager warning;
    public InterfaceAnimManager startScreen;
    public InterfaceAnimManager mainUI;
    public InterfaceAnimManager invBar;
    public InterfaceAnimManager informer;
    public InterfaceAnimManager invenUI;

    private float cachedShieldDmg;
    private bool oxygenWarningEnabled;

    private void Start()
    {
        cachedShieldDmg = 0;
        helmetOn = true;
        onHelmetAnimation = false;
        oxygenWarningEnabled = false;
        lastHurt = 0;
        helmetX = 0;
        warning.gameObject.SetActive(false);
        invenUI.gameObject.SetActive(false);
        // After Setting up, run startup animation, and start main UI
        StartCoroutine(SystemBoot());
    }

    // color and size change based on current health, amount=percentage
    public void HealthChange(float amount)
    {
        if (amount == 0)
            return;
        health.color = Color.HSVToRGB(amount / 3.6f, 0.8f, 1);
        health.rectTransform.sizeDelta = new Vector2(7.5f, 5f * amount);
    }

    // animation for shield damage, amount=+-percentage
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

    // animation when shield turns to full charge
    public void ShieldFull()
    {
        StartCoroutine(ShieldShake());
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

    // Warning animation that wakes hiden ui till canceled
    public void Warning(bool on)
    {
        if (on && !warning.gameObject.activeSelf)
        {
            warning.gameObject.SetActive(true);
            warning.startAppear();
        }
        else if (!on && warning.gameObject.activeSelf)
        {
            warning.startDisappear();
        }
    }

    // amount=cur percentage
    public void OxygenChange(float amount)
    {
        if (amount < 0)
        {
            // Avoid image fill amount exception
            amount = 0;
        }
        else if (amount < 0.1f && !oxygenWarningEnabled)
        {
            oxygenWarningEnabled = true;
            StartCoroutine(OxygenWarning());
        } 
        else if (oxygenWarningEnabled)
        {
            oxygenWarningEnabled = false;
            oxygenBar.color = Color.white;
            oxygenAmount.color = Color.white;
        }
        oxygenBar.fillAmount = amount;
        string txt = (amount * 100).ToString();
        // Ensure only two numbers with correct value will be displayed
        if (amount == 0)
        {
            txt = "0";
        }
        else if (txt.Substring(0, 3).Equals("100"))
        {
            txt = "99";
        } else if (txt.Substring(1, 1).Equals("."))
        {
            txt = txt.Substring(0, 1);
        } else
        {
            txt = txt.Substring(0, 2);
        }
        oxygenAmount.text = txt;
    }

    public void DisplayInformer(bool state)
    {
        if (state && informer.currentState == CSFHIAnimableState.disappeared)
        {
            informer.startAppear();
        } else if (!state)
        {
            informer.startDisappear();
        }
    }

    public void DisplayBag(bool state)
    {
        if (state)
        {
            invenUI.gameObject.SetActive(true);
            invenUI.startAppear();
        }
        else
        {
            invenUI.startDisappear();
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

    private IEnumerator SystemBoot()
    {
        float elapsed = 0f;
        startScreen.gameObject.SetActive(true);
        startScreen.startAppear();
        while (elapsed < 2.5f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        startScreen.startDisappear();
        elapsed = 0;
        while (elapsed < 2f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        startScreen.gameObject.SetActive(false);
        mainUI.gameObject.SetActive(true);
        mainUI.startAppear();
        invBar.gameObject.SetActive(true);
        invBar.startAppear();
    }

    // Make oxygen display red
    private IEnumerator OxygenWarning()
    {
        float elapsed = 0f;
        while(elapsed < 0.5f)
        {
            oxygenBar.color = Color.HSVToRGB(0, elapsed, 1);
            oxygenAmount.color = Color.HSVToRGB(0, elapsed, 1);
            elapsed += Time.time;
            yield return null;
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

    private IEnumerator ShieldShake()
    {
        Vector2 origSize = shield.rectTransform.sizeDelta;
        Color origColor = shield.color;
        shield.color = Color.white;
        float elapsed = 0f;
        float sizeChange = 10f;
        while (elapsed < 0.1f)
        {
            shield.rectTransform.sizeDelta = new Vector2(origSize.x + sizeChange * elapsed, origSize.y + sizeChange * elapsed);
            elapsed += Time.deltaTime;
            yield return null;
        }
        shield.rectTransform.sizeDelta = origSize;
        shield.color = origColor;
    }
}
