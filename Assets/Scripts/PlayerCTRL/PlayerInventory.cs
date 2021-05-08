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
                // In this case, the object material has already been fetched and dissappear TODO
                return false;
            }
            inventory.Add(obj, amount);

            // Add new obj into inventory UI
            GameObject imgIcon = new GameObject("item");
            imgIcon.AddComponent<Image>().sprite = Resources.Load<Sprite>("Icons/" + obj);
            imgIcon.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            imgIcon.transform.parent = slots[nextEmptySlot].transform;
            imgIcon.transform.localPosition = new Vector3(0, 0, 0);

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

    // return null if there is not such obj in inventory
    public GameObject getOut()
    {
        string obj = slotFull[selectedSlot];
        if (obj == "0")
        {
            return null;
        }
        // Make the slot blank
        slotFull[selectedSlot] = "0";
        int amount = 0;
        foreach (Transform child in slots[selectedSlot].GetComponent<Transform>())
        {
            if (child.name == "Amount")
            {
                amount = int.Parse(child.gameObject.GetComponent<Text>().text);
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

        // Create gameobject with properties from inventory, and return output
        GameObject gen = (GameObject)Instantiate(Resources.Load("Prefabs/" + obj), transform.position + transform.forward * 2, Quaternion.identity);
        gen.SetActive(false);
        if (gen.GetComponent<Rigidbody>() == null)
        {
            gen.AddComponent<Rigidbody>().useGravity = false;
        }
        gen.AddComponent<TinyObjGravity>().Init(transform.parent);
        MaterialProperty prop = gen.AddComponent<MaterialProperty>();
        prop.remainInteract = 1;
        prop.materialName = obj;
        prop.minProduct = amount;
        prop.maxProduct = amount;
        gen.tag = "Material";
        gen.transform.parent = transform.parent.Find("Material");

        return gen;
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

    // Check if selected item is placeable TODO
    public bool Placeable()
    {
        return slotFull[selectedSlot].Equals("Machine");
    }

    // check amount of one obj in inventory
    public int GetAmount(string obj)
    {
        return (int)inventory[obj];
    }
}