using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

// Checks if game has save files, if not disable button
public class StartContinueButton : MonoBehaviour
{
    private void Start()
    {
        BinaryFormatter format = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save0";
        try
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            stream.Position = 0;
            stream.Close();
            gameObject.GetComponent<Button>().interactable = true;
        }
        catch (Exception e)
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
    }
}
