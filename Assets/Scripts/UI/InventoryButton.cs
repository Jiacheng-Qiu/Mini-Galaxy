using UnityEngine;
using System.Collections;

public class InventoryButton : MonoBehaviour
{
    private int slotID;
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
        // Assign slotid onto submenu
        subSlot.gameObject.SetActive(true);
        subSlot.startAppear();
        int slotId = int.Parse(gameObject.name);
        subSlot.transform.GetComponent<InventoryButton>().SetId(slotId);
    }

    public void Use()
    {
        subSlot.startDisappear();
        Debug.Log("Used material in slot" + slotID);
    }

    public void Assign()
    {
        slotAssign.gameObject.SetActive(true);
        slotAssign.startAppear();
        subSlot.startDisappear();

    }

}
