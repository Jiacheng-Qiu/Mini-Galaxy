using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : UIInformer
{
    private Camera cam;
    public Transform weapon;
    public GameObject miningGun;
    public GameObject laserSample; // for weapon
    public GameObject miningLaser;

    private int weaponState; // 0=weapon, 1=mining
    
    // For weapon
    public int maxAmmo;
    public int maxMag;

    public int curMag;
    public int damage;

    private InterfaceAnimManager ammoDisplay;
    private InterfaceAnimManager inspectDisplay;
    private bool onInspect;
    private Text curAmmoText;
    private Text maxAmmoText;

    // For mining
    private float lastShoot;
    public Image heat;
    private float heatAmount;
    private bool onMining;
    public float heatAccumulationSecond;
    public float miningDmg;
    public GameObject sparks;


    void Awake()
    {
        cam = transform.Find("Main Camera").GetComponent<Camera>();

        weaponState = 0;

        // miner
        miningGun.SetActive(false);
        miningLaser.SetActive(false);
        onMining = false;
        heatAmount = 0;

        // weapon
        ammoDisplay = weapon.Find("Display").GetComponent<InterfaceAnimManager>();
        inspectDisplay = weapon.Find("Inspect").GetComponent<InterfaceAnimManager>();
        curAmmoText = ammoDisplay.transform.Find("curAmmo").GetComponent<Text>();
        maxAmmoText = ammoDisplay.transform.Find("maxAmmo").GetComponent<Text>();
        UpdateAmmo();

        ammoDisplay.gameObject.SetActive(true);
        ammoDisplay.startAppear();
        inspectDisplay.gameObject.SetActive(false);
        weapon.gameObject.SetActive(true);
        onInspect = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (weaponState == 0)
            {
                weapon.gameObject.SetActive(false);
                miningGun.SetActive(true);
                weaponState = 1;
            } 
            else if(weaponState == 1)
            {
                if (onMining)
                    StopMining();
                weapon.gameObject.SetActive(true);
                miningGun.SetActive(false);
                weaponState = 0;
            }
        }
        if (weaponState == 0)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Inspect();
                gameObject.GetComponent<PlayerMovement>().ChangeCursorFocus(onInspect);
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
        else if (weaponState == 1)
        {
            if (PlayerStatus.attackDisabled)
                return;
            if (heatAmount < 100 && Time.time > lastShoot + 0.25f && Input.GetMouseButtonDown(0))
            {
                miningLaser.SetActive(true);
                miningLaser.GetComponent<ShotBehavior>().StartExtend(80);
                onMining = true;
            }
            else if (onMining && Input.GetMouseButtonUp(0))
            {
                StopMining();
            }
        }
    }
    private void StopMining()
    {
        miningLaser.GetComponent<ShotBehavior>().EndExtend(80);
        sparks.SetActive(false);
        onMining = false;
        lastShoot = Time.time;
    }

    private void FixedUpdate()
    {
        if (weaponState == 1)
        {
            if (onMining)
            {
                heatAmount += heatAccumulationSecond * Time.deltaTime;
                if (heatAmount >= 100)
                {
                    // Overheat
                    StopMining();
                    return;
                }
                Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;
                if (Physics.Raycast(rayOrigin, cam.transform.forward, out hit, 10))
                {
                    GameObject other = hit.collider.gameObject;
                    if (other.tag == "Environment")
                    {
                        sparks.SetActive(true);
                        sparks.transform.position = hit.point;
                        sparks.transform.LookAt(transform);
                        if (other.GetComponent<EnvironmentComponent>().Hit(miningDmg * Time.deltaTime))
                        {
                            sparks.SetActive(false);
                        }
                    }
                    else
                        sparks.SetActive(false);
                }
                else
                    sparks.SetActive(false);
            }
            heat.color = Color.HSVToRGB(0, heatAmount / 100, 1);
            heat.fillAmount = heatAmount / 100;
        }
        if (!onMining && heatAmount > 0)
        {
            heatAmount -= heatAccumulationSecond * Time.deltaTime;
            if (heatAmount < 0)
                heatAmount = 0;
        }
    }

    public void Inspect()
    {
        onInspect = !onInspect;
        if (onInspect)
        {
            weapon.localPosition = new Vector3(0.55f, -0.24f, 2.85f);
            weapon.localRotation = Quaternion.Euler(0, 120, 0);
            ammoDisplay.startDisappear();
            inspectDisplay.gameObject.SetActive(true);
            inspectDisplay.startAppear();
            for (int i = 0; i < inspectDisplay.transform.childCount; i++)
            {
                inspectDisplay.transform.GetChild(i).localRotation = Quaternion.Euler(0, 220, 0);
            }
        }
        else
        {
            weapon.localPosition = new Vector3(1.75f, -0.875f, 2.4f);
            weapon.localRotation = Quaternion.Euler(0, 180, 0);
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
        GameObject curLaser = Instantiate(laserSample, laserSample.transform.position, laserSample.transform.rotation) as GameObject;
        curLaser.transform.SetParent(transform.GetComponent<PlayerMovement>().planet.transform);
        curLaser.SetActive(true);
        
        // Make raycast hit same time as bullet, destroy bullet after that
        Destroy(curLaser, ExamineHit());

        UpdateAmmo();
    }

    // Return the estimate time bullet hit object
    private float ExamineHit()
    {
        Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        // TODO: add interactable in the following
        if (Physics.Raycast(rayOrigin, cam.transform.forward, out hit, 100))
        {
            GameObject other = hit.collider.gameObject;
            float duration = Vector3.Distance(transform.position, hit.point) / 150;
            StartCoroutine(BulletHit(duration, other));
            return duration;
        }
        return 2;
    }

    private IEnumerator BulletHit(float time, GameObject obj)
    {
        float elapsed = 0;
        while(elapsed < time)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Possible that object gets destroyed while bullet flying
        if (obj != null)
        {
            switch (obj.tag)
            {
                case "Animal":
                    obj.GetComponent<HealthSystem>().Hurt(gameObject, damage);
                    break;

            }
        }
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
                weapon.Find("LongMuzzle").gameObject.SetActive(!weapon.Find("LongMuzzle").gameObject.activeSelf);
                break;
            case 1:
                weapon.Find("LongBarrel").gameObject.SetActive(!weapon.Find("LongBarrel").gameObject.activeSelf);
                break;
            case 2:
                weapon.Find("LongButtStock").gameObject.SetActive(!weapon.Find("LongButtStock").gameObject.activeSelf);
                break;
            case 3:
                weapon.Find("Stand").gameObject.SetActive(!weapon.Find("Stand").gameObject.activeSelf);
                break;
            case 4:
                weapon.Find("8xScope").gameObject.SetActive(!weapon.Find("8xScope").gameObject.activeSelf);
                break;

        }
    }



}
