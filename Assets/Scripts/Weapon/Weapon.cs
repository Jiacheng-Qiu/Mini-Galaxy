using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public GameObject user;
    public GameObject laser;
    public int maxAmmo = 300;
    public int maxMag = 30;
    public int curMag = 30;

    public Text ammoUI;

    void Start()
    {
        curMag = maxMag;
        UpdateText();
    }

    // Fire laser and reduce curMag
    public void Shoot()
    {
        if (curMag <= 0)
            return;
        curMag--;
        GameObject shot = Instantiate(laser, user.transform.Find("Main Camera").position, user.transform.Find("Main Camera").rotation) as GameObject;
        shot.GetComponent<ShotBehavior>().player = user;
        Destroy(shot, 3f);
        UpdateText();
    }

    private void UpdateText()
    {
        ammoUI.text = curMag + "/" + maxAmmo;
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
        UpdateText();
    }

}
