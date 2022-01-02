using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Random = UnityEngine.Random;

public class SaveControl : MonoBehaviour
{
    private SeedSettings seedSetting;
    public GameObject player;
    private SaveFormat data;

    private void Awake()
    {
        seedSetting = GameObject.Find("DataTransfer").GetComponent<SeedSettings>();
        if (seedSetting.onLoad)
        {
            LoadFromFile(0);
            // Set player stats based on data
        }

        Random.InitState(seedSetting.seed);
    }

    private void Start()
    {
        if (seedSetting.onLoad)
        {
            player.GetComponent<PlayerMovement>().LoadStatus(data);
            player.GetComponent<PlayerHealthSystem>().LoadStatus(data);
            player.GetComponent<Backpack>().LoadItems(data);
        }
    }

    // Called when new game is started, initialize save file
    public void SaveToFile(int file)
    {
        BinaryFormatter format = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save" + file;
        FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
        SaveFormat data = new SaveFormat();
        data.seed = seedSetting.seed;
        PlayerHealthSystem hpStat = player.GetComponent<PlayerHealthSystem>();
        data.hp = hpStat.currentHealth;
        data.maxHp = hpStat.maxHealth;
        data.shield = hpStat.currentShield;
        data.maxShield = hpStat.maxShield;
        data.oxygen = hpStat.currentOxygen;
        data.maxOxygen = hpStat.maxOxygen;
        data.armor = hpStat.armor;
        data.heartrate = hpStat.heartrate;
        PlayerMovement move = player.GetComponent<PlayerMovement>();
        data.speed = move.speed;
        data.runSpeed = move.runSpeed;
        data.jump = move.jump;
        data.posX = player.transform.position.x;
        data.posY = player.transform.position.y;
        data.posZ = player.transform.position.z;
        data.rotationX = player.transform.rotation.eulerAngles.x;
        data.rotationY = player.transform.rotation.eulerAngles.y;
        data.rotationZ = player.transform.rotation.eulerAngles.z;
        data.camViewX = move.viewX;
        Backpack backpack = player.GetComponent<Backpack>();
        data.items = backpack.getInventory();
        data.onBar = backpack.invBarPref();

        data.planetName = move.planet.name;
        data.planetPosX = move.planet.transform.position.x;
        data.planetPosY = move.planet.transform.position.y;
        data.planetPosZ = move.planet.transform.position.z;
        data.planetRotationX = move.planet.transform.rotation.eulerAngles.x;
        data.planetRotationY = move.planet.transform.rotation.eulerAngles.y;
        data.planetRotationZ = move.planet.transform.rotation.eulerAngles.z;


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
            data = format.Deserialize(stream) as SaveFormat;
            stream.Close();

            seedSetting.seed = data.seed;
            Transform planet = GameObject.Find(data.planetName).transform;
            planet.position = new Vector3(data.planetPosX, data.planetPosY, data.planetPosZ);
            planet.eulerAngles = new Vector3(data.planetRotationX, data.planetRotationY, data.planetRotationZ);
        }
        catch (Exception e)
        {
            Debug.Log("File not existing in save slot" + file + "\n" + e.StackTrace);
        }
    }
}
