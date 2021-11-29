using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent class for all UI related player scripts, inform PlayerGuiding of current states
public class UIInformer : MonoBehaviour
{
    public PlayerGuiding guide;

    public void InformGuide(string key, bool state)
    {
        guide.DisplayUsed(key);
        guide.DisplayChange(key, state);
    }
}
