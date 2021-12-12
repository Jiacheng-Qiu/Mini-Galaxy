using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{

    // UI display part
    public GameObject player;
    // Record the amount of slots 
    public int slotAmount;
    private GameObject[] slots;
    private int selectedSlot;

    private Backpack backpack;

    private Color notSelectedColor;
    private Color selectedColor;

    void Start()
    {
        slots = new GameObject[slotAmount];
        for (int i = 0; i < slotAmount; i++)
        {
            slots[i] = transform.Find("Slot" + i).gameObject;
        }
        notSelectedColor = Color.HSVToRGB(215f / 360, 0.7f, 1);
        selectedColor = Color.HSVToRGB(215f / 360, 1, 1);

        // Originally slot0 is selected
        selectedSlot = 0;
        slots[selectedSlot].GetComponent<Image>().color = selectedColor;
        backpack = player.GetComponent<Backpack>();
    }

    public void AssignFocus(int i)
    {
        // Assign selected slot based on input
        int origSelect = selectedSlot;
        selectedSlot = i;
        if (selectedSlot != origSelect)
        {
            slots[origSelect].GetComponent<Image>().color = notSelectedColor;
            slots[selectedSlot].GetComponent<Image>().color = selectedColor;
        }
    }

    // Icon associated methods to connect backpack with bar
    public void AssignIcon(int index, string item, int amount)
    {
        GameObject imgIcon = slots[index].transform.Find("Image").gameObject;
        imgIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/" + item);
        imgIcon.SetActive(true);
        GameObject amountCount = slots[index].transform.Find("Amount").gameObject;
        amountCount.GetComponent<Text>().text = amount.ToString();
        amountCount.SetActive(true);
    }

    public void UpdateAmount(int index, int amount)
    {
        slots[index].transform.Find("Amount").gameObject.GetComponent<Text>().text = amount.ToString();
    }

    public void UnassignIcon(int index)
    {
        slots[index].transform.Find("Image").gameObject.SetActive(false);
        slots[index].transform.Find("Amount").gameObject.SetActive(false);
    }

    public string CheckTag()
    {
        return backpack.CheckTag(selectedSlot, true);
    }

    // return null if there is not such obj in inventory
    public GameObject GetOut()
    {
        GameObject output = backpack.GetOut(selectedSlot, true);
        return output;
    }
}