using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotAssign : MonoBehaviour
{
    private InterfaceAnimManager slotAssign;
    private int slotID;
    public Backpack backpack;
    public InteractionAnimation anim;

    private void Start()
    {
        slotAssign = gameObject.GetComponent<InterfaceAnimManager>();
    }

    public void SetId(int id)
    {
        slotID = id;
    }

    public void AssignOnto(int pos)
    {
        backpack.PutOnBar(pos, slotID);
    }

    private void Update()
    {
        // If received close signal on either backpack or itself, then do disappear anyway
        if ((Input.GetMouseButtonUp(0) || !anim.GetBagUIStat()))
        {
            // Is active means that some other slot called the menu already, simply make it disappear
            if (slotAssign.currentState == CSFHIAnimableState.appeared)
            {
                slotAssign.startDisappear();
                return;
            }
        }
    }
}
