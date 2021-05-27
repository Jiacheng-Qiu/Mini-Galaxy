using UnityEngine;
using UnityEngine.UI;

public class SettingsRecord : MonoBehaviour
{
    public GameObject hInput;
    public GameObject vInput;

    public PlayerMovement player;
    // JSON file of settings
/*    public TextAsset settings;
    private class Settings
    {
        public float HorizontalSensitivity;
        public float VerticalSensitivity;
    }*/

    public void Clicked()
    {
        // Set placeholder text to inputs, and save inputs in player
        float x = float.Parse(hInput.transform.Find("Text").GetComponent<Text>().text);
        float y = float.Parse(vInput.transform.Find("Text").GetComponent<Text>().text);

        hInput.transform.Find("Placeholder").GetComponent<Text>().text = x.ToString();
        vInput.transform.Find("Placeholder").GetComponent<Text>().text = y.ToString();

        player.Set(x, y);

        /*Settings set = new Settings();
        set.HorizontalSensitivity = x;
        set.VerticalSensitivity = y;*/

        /*Debug.Log(x + " " + y);
        JsonUtility.FromJsonOverwrite(settings.text, set);*/
    }
}
