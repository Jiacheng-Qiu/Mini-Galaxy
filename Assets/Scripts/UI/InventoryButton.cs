using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int slotID;
    public bool isSubMenu; // If this object is submenu, it will listen to left clicks and disappear
    public InterfaceAnimManager subSlot;
    public PlayerMovement player; 
    public InterfaceAnimManager slotAssign; // On submenu side
    public InterfaceAnimManager splitMenu; // On submenu side
    public InterfaceAnimManager specItem; // On main button side
    public InteractionAnimation anim;

    // Record time mouse keep hover over obj
    private bool onStart;
    private float startTime;


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
        onStart = false;
        specItem.startDisappear();
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
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSubMenu)
        {
            startTime = Time.time;
            onStart = true;
        }
    }

    private void FixedUpdate()
    {
        if (onStart && Time.time > startTime + 1)
        {
            specItem.gameObject.SetActive(true);
            specItem.startAppear();
            specItem.transform.position = gameObject.transform.position + new Vector3(0.3f, -0.1f, 0);
            specItem.transform.Find("Name").GetComponent<Text>().text = player.GetComponent<Backpack>().GetItemName(slotID); ;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSubMenu)
        {
            onStart = false;
            specItem.startDisappear();
        }
    }
}
