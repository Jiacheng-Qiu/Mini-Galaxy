using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using Random = UnityEngine.Random;

public class SaveController : MonoBehaviour
{
    public SettingsRecord settingsData;
    public void RandomSeed()
    {
        int newSeed = Random.Range(0, 2147483647);
        GameObject.Find("New Game").transform.Find("Seed Num").GetComponent<Text>().text = newSeed.ToString();
        SeedSettings.seed = newSeed;
    }

    public void ModeSelection(int mode)
    {
        SeedSettings.gameMode = mode;
    }

    // Called when new game is started, initialize save file
    public void SaveToFile()
    {
        BinaryFormatter format = new BinaryFormatter();
        string path = "C:/Users/Emperdust/Desktop" + "/save1";
        FileStream stream = new FileStream(path, FileMode.Create);
        SaveFormat data = new SaveFormat();
        data.seed = SeedSettings.seed;
        data.gameMode = SeedSettings.gameMode;
        data.volume = settingsData.volume;
        data.fullScreen = settingsData.isFullScreen;
        data.resolution = 1920;  // TODO
        data.sensX = settingsData.sensx;
        data.sensX = settingsData.sensy;

        format.Serialize(stream, data);
        stream.Close();
    }

    // Called when load/continue
    public void LoadFromFile(int file)
    {
        BinaryFormatter format = new BinaryFormatter();
        string path = "C:/Users/Emperdust/Desktop" + "/save1";
        try
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveFormat data = format.Deserialize(stream) as SaveFormat;
            stream.Close();
            Debug.Log(data.volume);
        }
        catch (Exception e){

        }
    }
}
