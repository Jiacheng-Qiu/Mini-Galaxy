using UnityEngine;

[System.Serializable]
public class SaveFormat
{
    public int seed;
    public int gameMode;

    // player stat & information
    public float hp;
    public float shield;
    public float armor;
    public bool immune;
    public float speed;
    public float runSpeed;
    public float jump;
    public Vector3 pos;
    public Vector3 rotation;
}