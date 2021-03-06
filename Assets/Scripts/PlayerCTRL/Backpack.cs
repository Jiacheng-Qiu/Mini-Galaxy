using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// The backpack system that contains items
public class Backpack : MonoBehaviour
{
    public int itemLimit;
    public GameObject slotContainer;
    private GameObject[] slots;
    private string[] slotItems; // Save the name of all items in slots
    private Dictionary<string, int> inventory;
    private int currentItemCount;
    private int nextEmptySlot;

    private int[] onBar; // Record the items reged on inv bar
    private InteractionAnimation uiInteraction;
    public PlayerInventory invBar;

    void Awake()
    {
        // Create slots before interface enabled
        slots = new GameObject[itemLimit];
        CreateSlots(5);

        uiInteraction = gameObject.GetComponent<InteractionAnimation>();
        onBar = new int[5];
        for (int i = 0; i < 5; i++)
        {
            onBar[i] = -1;
        }
        currentItemCount = 0;
        nextEmptySlot = 0;

        inventory = new Dictionary<string, int>();
        slotItems = new string[itemLimit];
    }

    public void LoadItems(SaveFormat save)
    {
        foreach(string itemName in save.items.Keys){
            PutIn(itemName, save.items[itemName]);
        }
        for (int i = 0; i < save.onBar.Count(); i++)
        {
            if (save.onBar[i] != -1)
                PutOnBar(i, save.onBar[i]);
        }
    }


    public Dictionary<string, int> getInventory()
    {
        return inventory;
    }

    public int[] invBarPref()
    {
        return onBar;
    }

    // Generate slots based on amount on the UI
    private void CreateSlots(int itemPerRow)
    {
        GameObject slot = slotContainer.transform.Find("0").gameObject;
        for (int i = 0; i < itemPerRow; i++)
        {
            for (int j = 0; j < itemLimit / itemPerRow; j++)
            {
                int cur = i * itemPerRow + j;
                // On first slot, do nothing except assigning it into array
                if (cur == 0)
                {
                    slots[0] = slot;
                    continue;
                }
                slots[cur] = Instantiate(slot);
                slots[cur].name = cur.ToString();
                slots[cur].GetComponent<InventoryButton>().SetId(cur);
                slots[cur].transform.SetParent(slotContainer.transform);
                slots[cur].transform.localPosition = new Vector3(-75 + 37 * j, 75 - 37 * i, 0);
                slots[cur].transform.localRotation = Quaternion.identity;
                // Backpack cla = this;
                // slots[cur].GetComponent<Button>().onClick.AddListener(() => cla.OnListen(cur));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            uiInteraction.DisplayBag();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            invBar.AssignFocus(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            invBar.AssignFocus(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            invBar.AssignFocus(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            invBar.AssignFocus(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            invBar.AssignFocus(4);
        }
        
    }

    public String GetItemName(int pos)
    {
        return slotItems[pos];
    }

    // Return false if inventory up to limit
    // TODO update on inv bar
    public bool PutIn(string obj, int amount)
    {
        // Divided into already existed materials and new materials
        if (inventory.ContainsKey(obj))
        {
            int total = inventory[obj] + amount;
            inventory[obj] = total;
            // Renew the amount count
            int pos;
            for (pos = 0; pos < itemLimit; pos++)
            {
                if (slotItems[pos] != null && slotItems[pos].Equals(obj))
                {
                    break;
                }
            }
            slots[pos].transform.Find("Amount").GetComponent<Text>().text = total.ToString();
            UpdateAmount(pos);
        }
        else
        {
            if (currentItemCount >= itemLimit)
            {
                //In this case, the object material has already been fetched and dissappear
                // TODO throw item out
                return false;
            }
            
            inventory.Add(obj, amount);

            Transform img = slots[nextEmptySlot].transform.Find("Image");
            img.GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/" + obj);
            img.gameObject.SetActive(true);
            slotItems[nextEmptySlot] = obj;

            // Make the item counter appear & Init the amount to the slot
            GameObject amountCount = slots[nextEmptySlot].transform.Find("Amount").gameObject;
            amountCount.GetComponent<Text>().text = amount.ToString();
            amountCount.SetActive(true);
            
            currentItemCount++;

            // Check if some spot on inventory bar is empty, if so also add to bar
            for (int i = 0; i < onBar.Length; i++)
            {
                if(onBar[i] == -1)
                {
                    PutOnBar(nextEmptySlot, i);
                }
            }

            // Check for the next empty slot
            emptyChecker();
        }
        return true;
    }

    // Check an amount of some materials in the inventory for crafting
    public bool Check(string obj, int amount)
    {
        // Return false if the obj isn't in inventory
        for (int i = 0; i < itemLimit; i++)
        {
            if (slotItems[i] == obj && inventory[obj] >= amount)
            {
                return true;
            }
        }
        return false;
    }

    // Check if there exists empty slot for new items
    private void emptyChecker()
    {
        for (int i = 0; i < itemLimit; i++)
        {
            if (slotItems[i] == null)
            {
                nextEmptySlot = i;
                break;
            }
        }
    }

    // Use an amount of some materials in the inventory, mainly used by crafting
    public bool Use(string obj, int amount)
    {
        // Return false if the obj isn't in inventory
        for (int i = 0; i < itemLimit; i++)
        {
            if (slotItems[i] != null && slotItems[i].Equals(obj))
            {
                // Examine if the player have required amount
                if (inventory[obj] == amount)
                {
                    // if the count = required amount, run getOut and take out material entirely from slot
                    GetOut(i, false);
                    return true;
                }
                else if (inventory[obj] > amount)
                {
                    // Update amount in display UI and hashtable
                    inventory[obj] -= amount;
                    slots[i].transform.Find("Amount").GetComponent<Text>().text = inventory[obj].ToString();
                    UpdateAmount(i);
                    return true;
                }
                break;
            }
        }
        return false;
    }

    // Checker method for determining the tag of the selected item in slot
    public string CheckTag(int barPos, bool isOnBar)
    {
        int selectedSlot = isOnBar ? onBar[barPos] : barPos;
        if (selectedSlot == -1)
        {
            return "";
        }
        string obj = slotItems[selectedSlot];
        return ((GameObject)Resources.Load("Prefabs/" + obj)).tag;
    }

    // return null if there is not such obj in inventory
    public GameObject GetOut(int pos, bool barPos)
    {
        // The pos assigned could be from inv bar or from backpack directly
        int selectedSlot = barPos? onBar[pos] : pos;
        if (selectedSlot < 0 || selectedSlot >= itemLimit)
        {
            return null;
        }
        string item = slotItems[selectedSlot];
        if (item == null)
        {
            return null;
        }
        // Make the slot blank and hide UI effects
        slots[selectedSlot].transform.Find("Image").gameObject.SetActive(false);
        slots[selectedSlot].transform.Find("Amount").gameObject.SetActive(false);

        slotItems[selectedSlot] = null;
        int amount = inventory[item];
        inventory.Remove(item);
        currentItemCount--;
        UpdateAmount(selectedSlot);
        emptyChecker();

        return GenerateObject(item, amount);
    }

    private GameObject GenerateObject(string item, int amount)
    {
        // Create gameobject with properties from inventory, and return output
        GameObject gen;
        try
        {
             gen = (GameObject)Instantiate(Resources.Load("Prefabs/" + item));
        }
        catch
        {
            // If generation failed, this means the stuff generated is Interactable instead of material
            return new GameObject();
        }
        gen.SetActive(false);
        if (gen.GetComponent<Rigidbody>() == null)
        {
            gen.AddComponent<Rigidbody>().useGravity = false;
        }
        // Only assign material property when its material
        if (gen.tag == "Material")
        {
            MaterialProperty prop = gen.AddComponent<MaterialProperty>();
            prop.remainInteract = 1;
            prop.materialName = item;
            prop.minProduct = amount;
            prop.maxProduct = amount;
        }

        return gen;
    }

    // Update amount of item on inventory bar if assigned
    public void UpdateAmount(int slotPos)
    {
        if (onBar.Contains(slotPos))
        {
            int barPos = Array.IndexOf(onBar, slotPos);
            string item = slotItems[slotPos];
            // Check if the item still exists, if not unassign on both sides
            if (slotItems[slotPos] == null)
            {
                onBar[barPos] = -1;
                invBar.UnassignIcon(barPos);
            } else
            {
                // Update the amount on bar
                invBar.UpdateAmount(barPos, inventory[item]);
            }
        }
    }

    // Add to inventory bar when selected, display item pos on inv bar
/*    public void OnListen(int pos)
    {
        
        if (Input.GetKey(KeyCode.Alpha1))
        {
            IconHelper(0, pos);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            IconHelper(1, pos);
        } 
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            IconHelper(2, pos);
        } 
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            IconHelper(3, pos);
        } 
        else if (Input.GetKey(KeyCode.Alpha5))
        {
            IconHelper(4, pos);
        }
    }*/

    // Assign icon to inventory bar
    public void PutOnBar(int index, int slotId)
    {
        string item = slotItems[slotId];
        // Return when no object on that slot
        if (item == null)
        {
            return;
        }
        int amount = inventory[item];

        int orig = onBar[index];
        // If the pos on bar is previously assigned, unassign it.
        if (orig == slotId)
        {
            // If same thing already assigned, don't do it again
            return;
        }
        else if (onBar.Contains(slotId))
        {
            // Case that the same item is assigned onto different slot
            int oldPos = Array.IndexOf(onBar, slotId);
            onBar[oldPos] = -1;
            invBar.UnassignIcon(oldPos);
        }

        if (orig != -1 && orig != slotId)
        {
            invBar.UnassignIcon(index);
        }
        invBar.AssignIcon(index, item, amount);
        onBar[index] = slotId;
    }
}
