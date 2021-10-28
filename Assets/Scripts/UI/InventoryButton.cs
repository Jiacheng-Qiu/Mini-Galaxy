using UnityEngine;
using System.Collections;

public class InventoryButton : MonoBehaviour
{
    private int slotID;
    public bool isSubMenu; // If this object is submenu, it will listen to left clicks and disappear
    public InterfaceAnimManager subSlot;
    public PlayerMovement player; 
    public InterfaceAnimManager slotAssign; // On submenu side
    public InterfaceAnimManager splitMenu; // On submenu side

    public InteractionAnimation anim;

    // Setup the slot currently submenu is calling on
    public void SetId(int id)
    {
        slotID = id;
    }

    public void CallSubMenu()
    {
        if (subSlot.currentState != CSFHIAnimableState.disappeared)
        {
            return;
        }
        // Assign slotid onto submenu
        subSlot.gameObject.SetActive(true);
        subSlot.transform.position = gameObject.transform.position;
        subSlot.startAppear();
        int slotId = int.Parse(gameObject.name);
        subSlot.transform.GetComponent<InventoryButton>().SetId(slotId);
    }

    public void Use()
    {
        player.Place(slotID);
        anim.DisplayBag();
    }

    public void Assign()
    {
        if (slotAssign.currentState != CSFHIAnimableState.disappeared)
        {
            // Avoid having several slots calling assign, or assign called multiple times
            return;
        }
        slotAssign.gameObject.SetActive(true);
        slotAssign.startAppear();
        slotAssign.gameObject.GetComponent<SlotAssign>().SetId(slotID);
    }

    public void Split()
    {
        splitMenu.gameObject.SetActive(true);
        splitMenu.startAppear();
        splitMenu.gameObject.GetComponent<SplitMenu>().SetId(slotID);
        Debug.Log("Spliting");
    }

    public void Drop()
    {
        player.Throw(slotID);
    }

    private void Update()
    {
        // If this is submenu and received close signal on either backpack or itself, then do disappear anyway
        if (subSlot && (Input.GetMouseButtonUp(0) || !anim.GetBagUIStat()))
        {
            // Is active means that some other slot called the menu, simply make it disappear
            if (subSlot.currentState == CSFHIAnimableState.appeared)
            {
                subSlot.startDisappear();
                return;
            }
        }
    }

}
