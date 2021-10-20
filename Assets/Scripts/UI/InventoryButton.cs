using UnityEngine;
using System.Collections;

public class InventoryButton : MonoBehaviour
{
    private int slotID;
    public bool isSubMenu; // If this object is submenu, it will listen to left clicks and disappear
    public InterfaceAnimManager subSlot;
    public Backpack backpack; // Only necessary on submenu side
    public InterfaceAnimManager slotAssign; // On submenu side

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
        Debug.Log("Using");
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
        Debug.Log("Spliting");
    }

    public void Drop()
    {
        backpack.GetOut(slotID, false);
    }

    private void Update()
    {
        // If this is submenu and received close signal on either backpack or itself, then do disappear anyway
        if (subSlot && (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.B)))
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
