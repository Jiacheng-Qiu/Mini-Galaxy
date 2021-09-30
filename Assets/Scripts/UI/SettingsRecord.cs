using UnityEngine;
using UnityEngine.UI;

public class SettingsRecord : MonoBehaviour
{
    public PlayerMovement player;
    private int volumn = 0;
    private Color darkGrey;
    public Transform volFolder;
    // JSON file of settings
    /*    public TextAsset settings;
        private class Settings
        {
            public float HorizontalSensitivity;
            public float VerticalSensitivity;
        }*/
    /*Settings set = new Settings();
        set.HorizontalSensitivity = x;
        set.VerticalSensitivity = y;*/

    /*Debug.Log(x + " " + y);
    JsonUtility.FromJsonOverwrite(settings.text, set);*/
    private void Start()
    {
        darkGrey = new Color(0.196f, 0.196f, 0.196f);
    }

    public void VolDown()
    {
        if (volumn > 0)
        {
            volumn--;
            // Adjust the color of pictures
            volFolder.Find("vol" + volumn).GetComponent<Image>().color = darkGrey;
        }
    }

    public void VolUp()
    {
        if (volumn < 7)
        {
            // Adjust the color of pictures
            volFolder.Find("vol" + volumn).GetComponent<Image>().color = Color.white;
            volumn++;
        }
    }
}
