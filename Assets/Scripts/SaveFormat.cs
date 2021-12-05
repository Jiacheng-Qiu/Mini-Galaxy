using UnityEngine;

[System.Serializable]
public class SaveFormat
{
    public int seed;

    // player stat & information
    public float hp;
    public float maxHp;
    public float shield;
    public float maxShield;
    public int armor;
    public float oxygen;
    public float maxOxygen;
    public int heartrate;
    public float speed;
    public float runSpeed;
    public float jump;
    public float posX;
    public float posY;
    public float posZ;
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    public float camViewX;

    public string planetName;
    public float planetPosX;
    public float planetPosY;
    public float planetPosZ;
    public float planetRotationX;
    public float planetRotationY;
    public float planetRotationZ;

}