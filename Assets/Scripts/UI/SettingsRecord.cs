using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SettingsRecord : MonoBehaviour
{
    public PlayerMovement player;
    private Color darkGrey;
    public Transform volFolder;

    private void Start()
    {
        darkGrey = new Color(0.196f, 0.196f, 0.196f);
        volFolder = GameObject.Find("Settings").transform.Find("Vol ctr");
        LoadSettings();
    }

    public void SaveSettings()
    {
        BinaryFormatter format = new BinaryFormatter();
        string path = Application.persistentDataPath + "/settings";
        FileStream stream = new FileStream(path, FileMode.Create);
        SettingsFormat data = new SettingsFormat();
        data.volume = GameSettings.volume;
        data.sensX = GameSettings.sensx;
        data.sensY = GameSettings.sensy;
        data.fullScreen = GameSettings.isFullScreen;
        data.resolution = 1920;

        format.Serialize(stream, data);
        stream.Close();
    }

    // Only used on game startup each time, elsewise data saved in static class
    public void LoadSettings()
    {
        BinaryFormatter format = new BinaryFormatter();
        string path = Application.persistentDataPath + "/settings";
        try
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            stream.Position = 0;
            SettingsFormat data = format.Deserialize(stream) as SettingsFormat;
            Setup(data.volume, data.fullScreen, data.sensX, data.sensY);
            stream.Close();
        }
        catch (Exception e)
        {
            // In case of loading failure, could be due to no settings file existing, create a new one
            SaveSettings();
        }
    }

    public void Setup(int vol, bool isFull, float sx, float sy)
    {
        GameSettings.volume = 0;
        while (vol > 0)
        {
            VolUp();
            vol--;
        }
        GameSettings.sensx = sx;
        GameObject.Find("Settings").transform.Find("SensitivityX").Find("InputField").GetComponent<InputField>().text = sx.ToString();
        GameSettings.sensy = sy;
        GameObject.Find("Settings").transform.Find("SensitivityY").Find("InputField").GetComponent<InputField>().text = sy.ToString();
        GameSettings.isFullScreen = isFull;
        GameObject.Find("Settings").transform.Find("FS toggle").GetComponent<Toggle>().isOn = isFull;
    }

    public void VolDown()
    {
        if (GameSettings.volume > 0)
        {
            GameSettings.volume--;
            // Adjust the color of pictures
            volFolder.Find("vol" + GameSettings.volume).GetComponent<Image>().color = darkGrey;
        }
    }

    public void VolUp()
    {
        if (GameSettings.volume < 7)
        {
            // Adjust the color of pictures
            volFolder.Find("vol" + GameSettings.volume).GetComponent<Image>().color = Color.white;
            GameSettings.volume++;
        }
    }

    public void SetFullScreen()
    {
        GameSettings.isFullScreen = !GameSettings.isFullScreen;
    }

    public void SetSenX()
    {
        try
        {
            GameSettings.sensx = float.Parse(GameObject.Find("Settings").transform.Find("SensitivityX").Find("InputField").GetComponent<InputField>().text);
        }
        catch(Exception e) {
            Debug.Log("None num received, not accepted" + e);
        };
    }

    public void SetSenY()
    {
        try
        {
            GameSettings.sensy = float.Parse(GameObject.Find("Settings").transform.Find("SensitivityY").Find("InputField").GetComponent<InputField>().text);
        }
        catch (Exception e)
        {
            Debug.Log("None num received, not accepted" + e);
        };
    }
}
