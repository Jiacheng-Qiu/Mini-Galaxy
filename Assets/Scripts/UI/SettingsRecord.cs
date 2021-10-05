using UnityEngine;
using UnityEngine.UI;
using System;

public class SettingsRecord : MonoBehaviour
{
    public PlayerMovement player;
    private Color darkGrey;
    public Transform volFolder;

    public int volume;
    public bool isFullScreen;
    public float sensx;
    public float sensy;

    private void Start()
    {
        darkGrey = new Color(0.196f, 0.196f, 0.196f);
        volFolder = GameObject.Find("Settings").transform.Find("Vol ctr");
        isFullScreen = true;
        volume = 7;
        sensx = 1;
        sensy = 1;
}

    public void VolDown()
    {
        if (volume > 0)
        {
            volume--;
            // Adjust the color of pictures
            volFolder.Find("vol" + volume).GetComponent<Image>().color = darkGrey;
        }
    }

    public void VolUp()
    {
        if (volume < 7)
        {
            // Adjust the color of pictures
            volFolder.Find("vol" + volume).GetComponent<Image>().color = Color.white;
            volume++;
        }
    }

    public void SetFullScreen()
    {
        isFullScreen = !isFullScreen;
    }

    public void SetSenX()
    {
        try
        {
            sensx = float.Parse(GameObject.Find("Settings").transform.Find("SensitivityX").Find("InputField").GetComponent<InputField>().text);
        }
        catch(Exception e) {
            Debug.Log("None num received, not accepted");
        };
    }
    public void SetSenY()
    {
        try
        {
            sensy = float.Parse(GameObject.Find("Settings").transform.Find("SensitivityY").Find("InputField").GetComponent<InputField>().text);
        }
        catch (Exception e)
        {
            Debug.Log("None num received, not accepted");
        };
    }
}
