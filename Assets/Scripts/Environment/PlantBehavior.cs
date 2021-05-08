using UnityEngine;

public class PlantBehavior : EnvironmentComponent
{
    void Start()
    {
        product = Resources.Load("Prefabs/" + productName);
    }
}
