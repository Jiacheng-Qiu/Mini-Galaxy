using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// The backpack system that contains items
public class Backpack : MonoBehaviour
{
    public GameObject slotPrefab;
    public int itemLimit;
    private GameObject[] slots;
    private string[] slotItems; // Save the name of all items in slots
    private Hashtable inventory;
    private int currentItemCount;
    private int nextEmptySlot;
    private bool bagActive;

    private int[] onBar; // Record the items reged on inv bar

    private PlayerMovement movement;
    private InteractionAnimation uiInteraction;
    private PlayerInventory invBar;

    void Start()
    {
        uiInteraction = gameObject.GetComponent<InteractionAnimation>();
        bagActive = false;
        onBar = new int[5];
        for (int i = 0; i < 5; i++)
        {
            onBar[i] = -1;
        }
        currentItemCount = 0;
        nextEmptySlot = 0;

        inventory = new Hashtable();
        slotItems = new string[itemLimit];
        slots = new GameObject[itemLimit];
        //CreateSlots();
        movement = gameObject.GetComponent<PlayerMovement>();
        invBar = gameObject.GetComponent<PlayerInventory>();
    }

    // Generate slots based on amount on the UI
    private void CreateSlots()
    {
        int itemPerRow = 4;
        for (int i = 0; i < itemPerRow; i++)
        {
            for (int j = 0; j < itemLimit / itemPerRow; j++)
            {
                int cur = i * 4 + j;
                Backpack cla = this;
                slots[cur] = Instantiate(slotPrefab);
                slots[cur].transform.localPosition = new Vector3(750 + 105 * j, 680 - 105 * i, 0);
                slots[cur].transform.SetParent(transform);
                slots[cur].GetComponent<Button>().onClick.AddListener(() => cla.OnListen(cur));
                slots[cur].transform.Find("Amount").gameObject.SetActive(false);
                slots[cur].transform.Find("Image").gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            if (bagActive)
            {
                bagActive = false;
                movement.ChangeCursorFocus(false);
                uiInteraction.DisplayBag(false);
            }
            else
            {
                bagActive = true;
                movement.ChangeCursorFocus(true);
                uiInteraction.DisplayBag(true);
            }
        }
    }

    // Return false if inventory up to limit
    // TODO update on inv bar
    public bool PutIn(string obj, int amount)
    {
        // Divided into already existed materials and new materials
        if (inventory.ContainsKey(obj))
        {
            int total = (int)inventory[obj] + amount;
            inventory[obj] = total;
            // Renew the amount count
            int pos = 0;
            for (pos = 0; pos < itemLimit; pos++)
            {
                if (slotItems[pos] != null && slotItems[pos].Equals(obj))
                {
                    break;
                }
            }
            slots[pos].transform.Find("Amount").gameObject.GetComponent<Text>().text = total.ToString();
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

            // Add new obj into inventory UI
            GameObject imgIcon = new GameObject("Item");
            imgIcon.AddComponent<Image>().sprite = Resources.Load<Sprite>("Icons/" + obj);
            imgIcon.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            imgIcon.transform.SetParent(slots[nextEmptySlot].transform, false);
            imgIcon.transform.localPosition = new Vector3(0, 0, 0);

            slotItems[nextEmptySlot] = obj;
            GameObject amountCount = slots[nextEmptySlot].transform.Find("Amount").gameObject;
            // Make the item counter appear & Init the amount to the slot
            amountCount.SetActive(true);
            amountCount.GetComponent<Text>().text = amount.ToString();
            currentItemCount++;
            // Check for the next empty slot
            emptyChecker();
        }
        return true;
    }

    // Check an amount of some materials in the inventory
    public bool Check(string obj, int amount)
    {
        // Return false if the obj isn't in inventory
        for (int i = 0; i < itemLimit; i++)
        {
            if (slotItems[i] == obj && (int)inventory[obj] >= amount)
            {
                return true;
            }
        }
        return false;
    }

    private void emptyChecker()
    {
        // Check item limit and set next index in use
        for (int i = 0; i < itemLimit; i++)
        {
            if (slotItems[i] == null)
            {
                nextEmptySlot = i;
                break;
            }
        }
    }

    // Use an amount of some materials in the inventory
    // TODO update on inv bar
    public bool Use(string obj, int amount)
    {
        // Return false if the obj isn't in inventory
        for (int i = 0; i < itemLimit; i++)
        {
            if (slotItems[i] != null && slotItems[i].Equals(obj))
            {
                // Examine if the player have required amount
                if ((int)inventory[obj] == amount)
                {
                    // if the count = required amount, run getOut and take out material entirely from slot
                    GetOut(i);
                    return true;
                }
                else if ((int)inventory[obj] > amount)
                {
                    // Update amount in display UI and hashtable
                    inventory[obj] = (int)inventory[obj] - amount;
                    slots[i].transform.Find("Amount").gameObject.GetComponent<Text>().text = inventory[obj].ToString();
                    UpdateAmount(i);
                    return true;
                }
                break;
            }
        }
        return false;
    }

    // Checker method for determining the tag of the selected item in slot
    public string CheckTag(int barPos)
    {
        int selectedSlot = onBar[barPos];
        if (selectedSlot == -1)
        {
            return "";
        }
        string obj = slotItems[selectedSlot];
        return ((GameObject)Resources.Load("Prefabs/" + obj)).tag;
    }

    // return null if there is not such obj in inventory
    // TODO also delete on inventory bar
    public GameObject GetOut(int barPos)
    {
        int selectedSlot = onBar[barPos];
        if (selectedSlot == -1)
        {
            // No item on select, return
            return null;
        }
        string obj = slotItems[selectedSlot];
        if (obj == null)
        {
            return null;
        }
        // Make the slot blank and hide UI effects
        slots[selectedSlot].transform.Find("Selected").gameObject.SetActive(false);
        slots[selectedSlot].transform.Find("Code").gameObject.SetActive(false);

        slotItems[selectedSlot] = null;
        int amount = 0;
        foreach (Transform child in slots[selectedSlot].GetComponent<Transform>())
        {
            if (child.name == "Amount")
            {
                amount = int.Parse(child.gameObject.GetComponent<Text>().text);
                // No need to reset amount as it will init to new val each time
                child.gameObject.SetActive(false);
            }
            else if (child.name == "Item")
            {
                // Destroy the image icon
                GameObject.Destroy(child.gameObject);
                break;
            }
        }
        inventory.Remove(obj);
        currentItemCount--;
        emptyChecker();
        UpdateAmount(selectedSlot);
        // Create gameobject with properties from inventory, and return output
        GameObject gen;
        try
        {
            gen = (GameObject)Instantiate(Resources.Load("Prefabs/" + obj));
        }
        catch
        {
            Debug.LogError("Output failure");
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
            prop.materialName = obj;
            prop.minProduct = amount;
            prop.maxProduct = amount;
        }
        
        return gen;
    }

    public void UpdateAmount(int slotPos)
    {
        if (onBar.Contains(slotPos))
        {
            int barPos = Array.IndexOf(onBar, slotPos);
            string name = slotItems[slotPos];
            // Check if the item still exists, if not unassign on both sides
            if (slotItems[slotPos] == null)
            {
                onBar[barPos] = -1;
                invBar.UnassignIcon(barPos);
            } else
            {
                // Update the amount on bar
                invBar.UpdateIcon(barPos, (int)inventory[name]);
            }
        }
    }

    // Add to inventory bar when selected, display item pos on inv bar
    public void OnListen(int pos)
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
    }

    public void IconHelper(int index, int pos)
    {
        string name = slotItems[pos];
        if (name == null)
            return;
        int amount = (int)inventory[name];
        int orig = onBar[index];
        // If the pos on bar is previously assigned, unassign it.
        if (orig == pos)
        {
            Debug.Log("what");
            // If same thing already assigned, don't do it again
            return;
        }
        else if (onBar.Contains(pos))
        {
            // Case that the same item is assigned onto different slot
            int oldPos = Array.IndexOf(onBar, pos);
            onBar[oldPos] = -1;
            invBar.UnassignIcon(oldPos);
        }
        if (orig != -1 && orig != pos)
        {
            invBar.UnassignIcon(index);
            slots[orig].transform.Find("Selected").gameObject.SetActive(false);
            slots[orig].transform.Find("Code").gameObject.SetActive(false);

        }
        invBar.AssignIcon(index, name, amount);
        slots[pos].transform.Find("Selected").gameObject.SetActive(true);
        GameObject code = slots[pos].transform.Find("Code").gameObject;
        code.GetComponent<Text>().text = (index + 1).ToString();
        code.SetActive(true);
        onBar[index] = pos;
    }
}
