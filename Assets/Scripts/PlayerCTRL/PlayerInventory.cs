using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    private Hashtable inventory;
    public int itemLimit = 5;

    // UI display part
    public GameObject[] slots;
    public int selectedSlot = 0;

    public GameObject backpackObj;
    private Backpack backpack;

    void Start()
    {
        backpack = backpackObj.GetComponent<Backpack>();
        inventory = new Hashtable();
        // slots = new GameObject[itemLimit];
        for (int i = 0; i < itemLimit; i++)
        {
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
        return backpack.PutIn(obj, amount);
    }

    // Check an amount of some materials in the inventory
    public bool Check(string obj, int amount)
    {
        return backpack.Check(obj, amount);
    }

    // Use an amount of some materials in the inventory
    public bool use(string obj, int amount)
    {
        return backpack.Use(obj, amount);
    }

    public string CheckTag()
    {
        return backpack.CheckTag(selectedSlot);
    }

    // return null if there is not such obj in inventory
    public GameObject getOut()
    {
        GameObject output = backpack.GetOut(selectedSlot);
        if (output == null)
        {
            return null;
        }
        output.transform.position = transform.Find("Main Camera").position;
        output.transform.parent = transform.parent.Find("Material");
        return output;
    }

    // check amount of one obj in inventory
    public int GetAmount(string obj)
    {
        return (int)inventory[obj];
    }

    public void AssignIcon(int index, string item, int amount)
    {
        GameObject imgIcon = new GameObject("Item");
        imgIcon.AddComponent<Image>().sprite = Resources.Load<Sprite>("Icons/" + item);
        imgIcon.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        imgIcon.transform.SetParent(slots[index].transform, false);
        imgIcon.transform.localPosition = new Vector3(0, 0, 0);
        GameObject amountCount = slots[index].transform.Find("Amount").gameObject;
        amountCount.GetComponent<Text>().text = amount.ToString();
        amountCount.SetActive(true);
    }

    public void UpdateIcon(int index, int amount)
    {
        slots[index].transform.Find("Amount").gameObject.GetComponent<Text>().text = amount.ToString();
    }

    public void UnassignIcon(int index)
    {
        foreach (Transform child in slots[index].GetComponent<Transform>())
        {
            if (child.name == "Amount")
            {
                child.gameObject.SetActive(false);
            }
            else if (child.name == "Item")
            {
                // Destroy the image icon
                GameObject.Destroy(child.gameObject);
                break;
            }
        }
    }
}