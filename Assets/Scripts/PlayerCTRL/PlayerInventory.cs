using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    private Hashtable inventory;
    public int itemLimit = 5;
    private int currentItemCount = 0;

    // UI display part
    public GameObject[] slots;
    public GameObject itemButton;
    public GameObject itemButton2;
    public GameObject itemButton3;
    private string[] slotFull;
    private int nextEmptySlot = 0;
    public int selectedSlot = 0;

    void Start()
    {
        inventory = new Hashtable();
        // slots = new GameObject[itemLimit];
        slotFull = new string[itemLimit];
        for (int i = 0; i < itemLimit; i++)
        {
            slotFull[i] = "0";
            // mark all item counts and selected slots as hidden
            slots[i].transform.Find("Amount").gameObject.SetActive(false);
            if (i != selectedSlot)
                slots[i].transform.Find("Selected").gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        int origSelect = selectedSlot;
        // TODO: Need to optimize the ifs
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSlot = 0;
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSlot = 1;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedSlot = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedSlot = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectedSlot = 4;
        }
        slots[origSelect].transform.Find("Selected").gameObject.SetActive(false);
        slots[selectedSlot].transform.Find("Selected").gameObject.SetActive(true);
    }

    // Return false if inventory up to limit
    public bool putIn(string obj, int amount)
    {
        // Divided into already existed materials and new materials
        if (inventory.ContainsKey(obj))
        {
            int total = (int)inventory[obj] + amount;
            inventory[obj] = total;
            // Renew the amount count
            int pos = -1;
            for (int i = 0; i < itemLimit; i++)
            {
                if (slotFull[i].Equals(obj))
                {
                    pos = i;
                    break;
                }
            }
            
            slots[pos].transform.Find("Amount").gameObject.GetComponent<Text>().text = total.ToString();
        }
        else
        {
            if (currentItemCount >= itemLimit)
            {
                return false;
            }
            inventory.Add(obj, amount);
            // Add new obj into inventory UI

            // TODO: The IF check here is just for testing use
            if (obj == "Iron")
                Instantiate(itemButton, slots[nextEmptySlot].transform);
            else if (obj == "Copper")
                Instantiate(itemButton2, slots[nextEmptySlot].transform);
            else if (obj == "Steel")
                Instantiate(itemButton3, slots[nextEmptySlot].transform);
            else if (obj == "Machine")
                Instantiate(itemButton3, slots[nextEmptySlot].transform);

            slotFull[nextEmptySlot] = obj;
            GameObject amountCount = slots[nextEmptySlot].transform.Find("Amount").gameObject;
            // Make the item counter appear & Init the amount to the slot
            amountCount.SetActive(true);
            amountCount.GetComponent<Text>().text = amount.ToString();

            currentItemCount++;
            emptyChecker();
        }
        return true;
    }

    // Check an amount of some materials in the inventory
    public bool check(string obj, int amount)
    {
        // Return false if the obj isn't in inventory
        for (int i = 0; i < itemLimit; i++)
        {
            if (slotFull[i].Equals(obj) && (int)inventory[obj] >= amount)
            {
                return true;
            }
        }
        return false;
    }

    // Use an amount of some materials in the inventory
    public bool use(string obj, int amount)
    {
        // Return false if the obj isn't in inventory
        for (int i = 0; i < itemLimit; i++)
        {
            if (slotFull[i].Equals(obj))
            {
                // Examine if the player have required amount
                if ((int) inventory[obj] == amount)
                {
                    // if the count = required amount, run getOut and take out material
                    int recordSelect = selectedSlot;
                    selectedSlot = i;
                    getOut();
                    selectedSlot = recordSelect;
                    return true;
                } else if ((int)inventory[obj] > amount)
                {
                    // Update amount in display UI and hashtable
                    inventory[obj] = (int)inventory[obj] - amount;
                    slots[i].transform.Find("Amount").gameObject.GetComponent<Text>().text = inventory[obj].ToString();
                    return true;
                }
                break;
            }
        }
        return false;
    }

    // return false if there is not such obj in inventory
    public bool getOut()
    {
        string obj = slotFull[selectedSlot];
        if (obj == "0")
        {
            return false;
        }
        // Make the slot blank
        slotFull[selectedSlot] = "0";
        foreach (Transform child in slots[selectedSlot].GetComponent<Transform>())
        {
            if (child.name == "Amount")
            {
                // No need to reset amount as it will init to new val each time
                child.gameObject.SetActive(false);
            }
            else if (child.name != "Num" && child.name != "Selected")
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        inventory.Remove(obj);
        currentItemCount--;
        emptyChecker();
        return true;
    }

    private void emptyChecker()
    {
        // Check item limit and set next index in use
        for (int i = 0; i < itemLimit; i++)
        {
            if (slotFull[i] == "0")
            {
                nextEmptySlot = i;
                break;
            }
        }
    }

    // Check if selected item is placeable
    public bool placeable()
    {
        return slotFull[selectedSlot].Equals("Machine");
    }

    // check amount of one obj in inventory
    public int getAmount(string obj)
    {
        return (int)inventory[obj];
    }
}