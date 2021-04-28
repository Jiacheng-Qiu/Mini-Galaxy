using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For all material spawn or placed on the ground waiting for user interaction.
public class MaterialProperty : MonoBehaviour
{
    public int remainInteract = 3;
    public string materialName = "Iron";
    public int minProduct = 1;
    public int maxProduct = 3;

    // When the material is interacted, produce "product" for player
    public int Interacted()
    {
        // Interacted
        remainInteract--;
        if (remainInteract < 1)
        {
            Destroy(this.gameObject);
        }
        return (int) Random.Range(minProduct, maxProduct);

        
    }

    public string getName()
    {
        return materialName;
    }
}
