using UnityEngine;
using UnityEngine.UI;

public class Weapon : UIInformer
{
    public GameObject user;
    private GameObject laser;
    public int maxAmmo;
    public int maxMag;
    public int curMag;

    private InterfaceAnimManager ammoDisplay;
    private InterfaceAnimManager inspectDisplay;
    private bool onInspect;
    private Text curAmmoText;
    private Text maxAmmoText;

    void Start()
    {
        curMag = maxMag;
        laser = Resources.Load("Laser/laser") as GameObject;
        ammoDisplay = transform.Find("Display").GetComponent<InterfaceAnimManager>();
        inspectDisplay = transform.Find("Inspect").GetComponent<InterfaceAnimManager>();
        curAmmoText = ammoDisplay.transform.Find("curAmmo").GetComponent<Text>();
        maxAmmoText = ammoDisplay.transform.Find("maxAmmo").GetComponent<Text>();
        UpdateAmmo();

        ammoDisplay.gameObject.SetActive(true);
        ammoDisplay.startAppear();
        inspectDisplay.gameObject.SetActive(false);
        onInspect = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Inspect();
            user.GetComponent<PlayerMovement>().ChangeCursorFocus(onInspect);
        }
        if (PlayerStatus.attackDisabled)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    public void Inspect()
    {
        onInspect = !onInspect;
        if (onInspect)
        {
            transform.localPosition = new Vector3(0.55f, -0.24f, 2.85f);
            transform.localRotation = Quaternion.Euler(0, 120, 0);
            ammoDisplay.startDisappear();
            inspectDisplay.gameObject.SetActive(true);
            inspectDisplay.startAppear();
            for (int i = 0; i < inspectDisplay.transform.childCount; i++)
            {
                inspectDisplay.transform.GetChild(i).localRotation = Quaternion.Euler(0, 220, 0);
                /*inspectDisplay.transform.GetChild(i).LookAt(transform.parent.Find("HelmetViewCamera"));
                inspectDisplay.transform.GetChild(i).localRotation *= Quaternion.Euler(0, 180, 0);*/
            }
        }
        else
        {
            transform.localPosition = new Vector3(1.75f, -0.875f, 2.4f);
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            ammoDisplay.startAppear();
            inspectDisplay.startDisappear();
        }
        InformGuide("I", onInspect);
    }

    // Fire laser and reduce curMag
    public void Shoot()
    {
        if (curMag <= 0)
            return;
        curMag--;
        GameObject shot = Instantiate(laser, user.transform.Find("Body").position, user.transform.Find("Main Camera").rotation) as GameObject;
        shot.GetComponent<ShotBehavior>().player = user;
        Destroy(shot, 3f);
        UpdateAmmo();
    }

    private void UpdateAmmo()
    {
        curAmmoText.text = curMag.ToString();
        maxAmmoText.text = maxAmmo.ToString();
    }

    public void Reload()
    {
        if (maxAmmo < maxMag - curMag)
        {
            curMag += maxAmmo;
            maxAmmo = 0;
        }
        else
        {
            maxAmmo -= maxMag - curMag;
            curMag = maxMag;
        }
        UpdateAmmo();
    }

    public void AddAmmo(int amount)
    {
        maxAmmo += amount;
        UpdateAmmo();
    }

    public void SwitchComp(int pos)
    {
        switch (pos)
        {
            case 0:
                transform.Find("LongMuzzle").gameObject.SetActive(!transform.Find("LongMuzzle").gameObject.activeSelf);
                break;
            case 1:
                transform.Find("LongBarrel").gameObject.SetActive(!transform.Find("LongBarrel").gameObject.activeSelf);
                break;
            case 2:
                transform.Find("LongButtStock").gameObject.SetActive(!transform.Find("LongButtStock").gameObject.activeSelf);
                break;
            case 3:
                transform.Find("Stand").gameObject.SetActive(!transform.Find("Stand").gameObject.activeSelf);
                break;
            case 4:
                transform.Find("8xScope").gameObject.SetActive(!transform.Find("8xScope").gameObject.activeSelf);
                break;

        }
    }
}
