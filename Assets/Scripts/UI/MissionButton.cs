using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionButton : MonoBehaviour
{
    public MissionUI core;

    public void CallDisplay()
    {
        core.SwitchOnDisplay(int.Parse(transform.name));
    }
}
