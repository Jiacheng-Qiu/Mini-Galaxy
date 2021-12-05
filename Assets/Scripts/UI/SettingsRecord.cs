using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SettingsRecord : MonoBehaviour
{
    private Color darkGrey;
    public Transform volFolder;
    public GameSettings gameSettings;

    private void Start()
    {
        darkGrey = new Color(0.196f, 0.196f, 0.196f);
        volFolder = GameObject.Find("Settings").transform.Find("Vol ctr");
        if (gameSettings == null)
        {
            gameSettings = GameObject.Find("DataTransfer").GetComponent<GameSettings>();
        }
        LoadSettings();

        Screen.fullScreen = gameSettings.isFullScreen;
    }

    public void SaveSettings()
    {
        BinaryFormatter format = new BinaryFormatter();
        string path = Application.persistentDataPath + "/settings.dat";
        FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
        SettingsFormat data = new SettingsFormat();
        data.volume = gameSettings.volume;
        data.sensX = gameSettings.sensx;
        data.sensY = gameSettings.sensy;
        data.fullScreen = gameSettings.isFullScreen;
        data.fov = gameSettings.fov;

        format.Serialize(stream, data);
        stream.Close();
    }

    // Only used on game startup each time, elsewise data saved in static class
    public void LoadSettings()
    {
        BinaryFormatter format = new BinaryFormatter();
        string path = Application.persistentDataPath + "/settings.dat";
        FileStream stream = null;
        try
        {
            stream = new FileStream(path, FileMode.Open);
            stream.Position = 0;
            SettingsFormat data = format.Deserialize(stream) as SettingsFormat;
            Setup(data.volume, data.fullScreen, data.sensX, data.sensY, data.fov);
            stream.Close();
        }
        catch (Exception e)
        {
            if (stream != null)
                stream.Close();
            // In case of loading failure, could be due to no settings file existing, create a new one
            SaveSettings();
        }
    }

    public void Setup(int vol, bool isFull, float sx, float sy, int fov)
    {
        gameSettings.volume = 0;
        while (vol > 0)
        {
            VolUp();
            vol--;
        }
        gameSettings.sensx = sx;
        volFolder.parent.Find("sensXInput").GetComponent<InputField>().text = sx.ToString();
        gameSettings.sensy = sy;
        volFolder.parent.Find("sensYInput").GetComponent<InputField>().text = sy.ToString();
        gameSettings.isFullScreen = isFull;
        volFolder.parent.Find("FS toggle").GetComponent<Toggle>().isOn = isFull;
        gameSettings.fov = fov;
        volFolder.parent.Find("Fov slider").GetComponent<Slider>().value = fov;
    }

    public void VolDown()
    {
        if (gameSettings.volume > 0)
        {
            gameSettings.volume--;
            // Adjust the color of pictures
            volFolder.Find("vol" + gameSettings.volume).GetComponent<Image>().color = darkGrey;
        }
    }

    public void VolUp()
    {
        if (gameSettings.volume < 7)
        {
            // Adjust the color of pictures
            volFolder.Find("vol" + gameSettings.volume).GetComponent<Image>().color = Color.white;
            gameSettings.volume++;
        }
    }

    public void SetFullScreen()
    {
        gameSettings.isFullScreen = !gameSettings.isFullScreen;
        Screen.fullScreen = gameSettings.isFullScreen;
    }

    public void SetSenX()
    {
        try
        {
            gameSettings.sensx = float.Parse(volFolder.parent.Find("sensXInput").GetComponent<InputField>().text);
        }
        catch(Exception e) {
            Debug.Log("None num received, not accepted" + e);
        };
    }

    public void SetSenY()
    {
        try
        {
            gameSettings.sensy = float.Parse(volFolder.parent.Find("sensYInput").GetComponent<InputField>().text);
        }
        catch (Exception e)
        {
            Debug.Log("None num received, not accepted" + e);
        };
    }

    public void SetFoV()
    {
        gameSettings.fov = (int)volFolder.parent.Find("Fov slider").GetComponent<Slider>().value;
    }
}
