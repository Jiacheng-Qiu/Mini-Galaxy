using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitMenu : MonoBehaviour
{
    private InterfaceAnimManager split;
    private int slotID;
    public Backpack backpack;
    private int origAmount;
    private int newAmount;

    private void Start()
    {
        split = gameObject.GetComponent<InterfaceAnimManager>();
    }

    public void SetId(int id)
    {
        slotID = id;
    }

    public void ConfirmSplit()
    {
        // Get the amount from split bar
        split.startDisappear();
    }

    // Cancel button
    public void Disappear()
    {
        split.startDisappear();
    }

    private void Update()
    {
        // If received close signal on either backpack or itself, then do disappear anyway
        if (Input.GetKeyUp(KeyCode.B))
        {
            // Is active means that some other slot called the menu already, simply make it disappear
            if (split.currentState != CSFHIAnimableState.disappeared)
            {
                split.startDisappear();
                return;
            }
        }
    }
}
