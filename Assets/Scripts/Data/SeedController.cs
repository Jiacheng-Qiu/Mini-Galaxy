using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class SeedController : MonoBehaviour
{
    public SeedSettings seedSetting;

    public void CreateNewSave()
    {
        seedSetting.onLoad = false;
    }

    public void LoadInstead()
    {
        seedSetting.onLoad = true;
    }

    public void RandomSeed()
    {
        int newSeed = Random.Range(0, 2147483647);
        transform.Find("Seednum").GetComponent<InputField>().text = newSeed.ToString();
        seedSetting.seed = newSeed;
        seedSetting.onLoad = false;
    }

    public void InputSeed()
    {
        try
        {
            seedSetting.seed = (int)float.Parse(transform.Find("Seednum").GetComponent<InputField>().text);
        }
        catch (Exception e)
        {
            Debug.Log("None num received, not accepted" + e);
        };
    }
}
