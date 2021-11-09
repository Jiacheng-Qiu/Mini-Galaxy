using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour
{
    public string missionName;
    public string missionDescription;
    public int serializedIndex; // Works like primary key in database
    public Vector3 position;
    public int type; // 0=main,1=optional,2=hidden
    public Mission(string n, string d, Vector3 p, int s, int t)
    {
        missionName = n;
        missionDescription = d;
        position = p;
        serializedIndex = s;
        type = t;
    }
}
