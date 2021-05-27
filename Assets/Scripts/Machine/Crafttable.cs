using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafttable : Machine
{
    public float range = 5f;
    private bool isActive = false; // Keep track of current state, if stay in same state, no adjustion needed
    private string[] list;
    private void Start()
    {
        list = new string[2];
        list[0] = "Spaceship";
        list[1] = "Furnace";
    }

    // If player is within range of the crafttable, enable corresponding crafting recipes
    private void FixedUpdate()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < range)
        {
            if (!isActive)
            {
                isActive = true;
                player.GetComponent<Crafting>().SwitchRecipeState(list, isActive);
            }
        } 
        else
        {
            if (isActive)
            {
                isActive = false;
                player.GetComponent<Crafting>().SwitchRecipeState(list, isActive);
            }
        }
    }
}
