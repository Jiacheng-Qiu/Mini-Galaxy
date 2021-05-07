using UnityEngine;

public class StoneBehavior : EnvironmentComponent
{
    
    void Start()
    {
        product = Resources.Load("MineOres/Prefabs/" + productName);
    }
}
