using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionAnimation : MonoBehaviour
{
    // Contains animations while player status change
    public Camera cam;
    public Camera uiCam;
    private float lastHurt;
    public GameObject helmet;
    public PlayerMovement movement;

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
    public InterfaceAnimManager craftUI;

    private float cachedShieldDmg;
    private bool oxygenWarningEnabled;
    private bool onStartup;
    private bool firstFrame;
    private bool bagActive;
    private Task walk;
    private float lastUIAnim;
    private bool craftActive;

    private void Start()
    {
        cachedShieldDmg = 0;
        lastUIAnim = -5;
        helmetOn = true;
        onHelmetAnimation = false;
        oxygenWarningEnabled = false;
        onStartup = true;
        firstFrame = true;
        lastHurt = 0;
        helmetX = 0;
        warning.gameObject.SetActive(false);
        invenUI.gameObject.SetActive(false);
        craftUI.gameObject.SetActive(false);
        bagActive = false;
        craftActive = false;
        walk = null;
    }

    private void FixedUpdate()
    {
        if (onStartup)
            StartupAnimation();

        HelmetAnim();

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

    // Method called only on startup for animation
    private void StartupAnimation()
    {

        if (firstFrame)
        {
            startScreen.gameObject.SetActive(true);
            startScreen.startAppear();
            firstFrame = false;
        }
        else if (startScreen.currentState == CSFHIAnimableState.appeared)
        {
            startScreen.startDisappear();
        }
        else if (startScreen.currentState == CSFHIAnimableState.disappeared)
        {
            // Only happen after startScreen runs one loop
            startScreen.gameObject.SetActive(false);
            mainUI.gameObject.SetActive(true);
            mainUI.startAppear();
            invBar.gameObject.SetActive(true);
            invBar.startAppear();
            onStartup = false;
        }
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

    // state: 1=walk, 2=run. Result in different amount of camera shake
    public void WalkCamEffect(float speed)
    {
        if (walk == null || !walk.Running)
        {
            walk = new Task(CameraShake(uiCam, 0.6f / speed, new Vector2(0, -0.002f), true, 0));
            new Task(CameraShake(cam, 0.6f / speed, new Vector2(0, 0.3f), true, 0));
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
            StartCoroutine(CameraShake(cam, 0.4f, new Vector2(0.2f, 0.2f), false, 0));
            lastHurt = Time.time;
        }
    }

    public void LowHPAnimation()
    {
        // TODO: make game borders red
    }

    public void HelmetSwitch()
    {
        if (!onHelmetAnimation)
        {
            onHelmetAnimation = true;
        }
    }

    private void HelmetAnim()
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
            StartCoroutine(OxygenWarning(2));
        }
        else if (amount > 0.2f && oxygenWarningEnabled)
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

    // Only one of inventory/crafttable can be active
    public void DisplayBag()
    {
        if (!bagActive)
        {
            if (craftActive)
            {
                DisplayCraft();
            }
            invenUI.gameObject.SetActive(true);
            invenUI.startAppear();
            bagActive = true;
        }
        else
        {
            invenUI.startDisappear();
            bagActive = false;
        }
        movement.ChangeCursorFocus(bagActive);
    }

    public void DisplayCraft()
    {
        if (!craftActive)
        {
            if (bagActive)
            {
                DisplayBag();
            }
            craftUI.gameObject.SetActive(true);
            craftUI.startAppear();
            craftActive = true;
        }
        else
        {
            craftUI.startDisappear();
            craftActive = false;
        }
        movement.ChangeCursorFocus(craftActive);
    }

    public void UISlide(bool isLeft)
    {
        // Make sure that slide only happen when a UI is fully appeared
        if (craftUI.currentState == CSFHIAnimableState.appearing || invenUI.currentState == CSFHIAnimableState.appearing)
            return;
        if (Time.time > lastUIAnim + 1.75f)
        {
            if (isLeft)
            {
                StartCoroutine(UISlideAnim(craftUI, bagActive ? 2 : 1, 1.75f));
                StartCoroutine(UISlideAnim(invenUI, bagActive ? 1 : 2, 1.75f));
            }
            else
            {
                StartCoroutine(UISlideAnim(craftUI, bagActive ? 0 : 3, 1.75f));
                StartCoroutine(UISlideAnim(invenUI, bagActive ? 3 : 0, 1.75f));
            }
            bagActive = !bagActive;
            craftActive = !craftActive;
            lastUIAnim = Time.time;
        }
    }

    // For bag submenu to check if disappear
    public bool GetBagUIStat()
    {
        return bagActive;
    }

    public bool GetCraftUIStat()
    {
        return craftActive;
    }

    // Method for switching 

    // Make oxygen display red
    private IEnumerator OxygenWarning(float lim)
    {
        float elapsed = 0f;
        while(elapsed < lim)
        {
            float curTime = elapsed / lim;
            oxygenBar.color = Color.HSVToRGB(0, curTime, 1);
            oxygenAmount.color = Color.HSVToRGB(0, curTime, 1);
            elapsed += Time.deltaTime;
            // UI blink effect
            if (curTime > 0.75f)
            {
                oxygenBar.enabled = true;
                oxygenAmount.enabled = true;
            } 
            else if (curTime > 0.5f)
            {
                oxygenBar.enabled = false;
                oxygenAmount.enabled = false;
            } 
            else if (curTime > 0.25f)
            {
                oxygenBar.enabled = true;
                oxygenAmount.enabled = true;
            }
            else
            {
                oxygenBar.enabled = false;
                oxygenAmount.enabled = false;
            }
            yield return null;
        }
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

    // Provide camera xy axis shake, isRegular decides if a random shake is generated. On regular shakes, camera will move for a up and down (and/or left and right) loop
    private IEnumerator CameraShake(Camera camera, float duration, Vector2 magnitude, bool isRegular, float wait)
    {
        Vector3 origPos = camera.transform.localPosition;
        float elapsed = 0f;

        // Wait for start
        while(elapsed < wait)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        // Shift amount based on magnitude per frame
        float xShiftPerFrame = magnitude.x * Time.deltaTime / (0.25f * duration);
        float yShiftPerFrame = magnitude.y * Time.deltaTime / (0.25f * duration);
        bool xEnabled = magnitude.x != 0;
        bool yEnabled = magnitude.y != 0;
        while (elapsed < duration)
        {
            float x = origPos.x;
            float y = origPos.y;
            if (xEnabled)
            {
                if (isRegular)
                {
                    if (elapsed > 0.75f * duration || elapsed < 0.25f * duration)
                    {
                        x = camera.transform.localPosition.x + xShiftPerFrame;
                    } 
                    else
                    {
                        x = camera.transform.localPosition.x - xShiftPerFrame;
                    }
                }
                else
                    x = Random.Range(-1f, 1f) * magnitude.x;
            }
            if (yEnabled)
            {
                if (isRegular)
                {
                    if (elapsed > 0.75f * duration || elapsed < 0.25f * duration)
                    {
                        y = camera.transform.localPosition.y + yShiftPerFrame;
                    }
                    else
                    {
                        y = camera.transform.localPosition.y - yShiftPerFrame;
                    }
                }
                else
                    y = Random.Range(-1f, 1f) * magnitude.y;
            }
            camera.transform.localPosition = new Vector3(x, y, origPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        camera.transform.localPosition = origPos;

    }

    // Four different states: 0=from left to center, 1=from center to left, 2=from right to center, 3=from center to right
    // Slide ui to quickly switch among different interfaces without pressing buttons
    private IEnumerator UISlideAnim(InterfaceAnimManager ui, int state, float time)
    {
        Vector3 origPos = ui.transform.localPosition;
        Quaternion origRot = ui.transform.localRotation;
        // Adjust starting position based on state
        if (state == 0)
        {
            ui.startAppear();
            ui.transform.RotateAround(uiCam.transform.position, Vector3.up, -90);
        } 
        else if(state == 2)
        {
            ui.startAppear();
            ui.transform.RotateAround(uiCam.transform.position, Vector3.up, 90);
        } else
        {
            ui.startDisappear();
        }

        float degreePerFrame = ((state == 0 || state == 3) ? 1 : -1) * 90 / time;
        float elapsed = 0;
        while (elapsed < time)
        {
            ui.transform.RotateAround(uiCam.transform.position, Vector3.up, degreePerFrame * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        ui.transform.localPosition = origPos;
        ui.transform.localRotation = origRot;
    }
}
