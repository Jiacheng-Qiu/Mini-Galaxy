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
        string path = Application.persistentDataPath + "/save1";
        FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
        SaveFormat data = new SaveFormat();
        data.seed = SeedSettings.seed;
        data.gameMode = SeedSettings.gameMode;

        format.Serialize(stream, data);
        stream.Close();
    }

    // Called when load/continue
    public void LoadFromFile(int file)
    {
        BinaryFormatter format = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save" + file;
        try
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            stream.Position = 0;
            SaveFormat data = format.Deserialize(stream) as SaveFormat;
            stream.Close();

            SeedSettings.seed = data.seed;
            SeedSettings.gameMode = data.gameMode;
        }
        catch (Exception e){
            Debug.Log("File not existing in save slot" + file);
        }
    }
}
