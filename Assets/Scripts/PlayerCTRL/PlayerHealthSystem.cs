using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerHealthSystem : HealthSystem
{
    
    // While dead, destroy player, and create a crate on ground with everything in backpack
    private void Die()
    {
        Debug.Log("player is dead!");
        // TODO: add crate on ground
        Destroy(this.gameObject);
    }
}
