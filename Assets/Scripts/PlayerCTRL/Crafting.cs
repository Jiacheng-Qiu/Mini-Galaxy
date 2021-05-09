using UnityEngine;

public class Crafting : MonoBehaviour
{
    public Canvas craftCanvas;
    private PlayerInventory inventory;
    private PlayerMovement ctrl;
    void Start()
    {
        craftCanvas.enabled = false;
        inventory = this.gameObject.GetComponent<PlayerInventory>();
        ctrl = this.gameObject.GetComponent<PlayerMovement>();
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.I))
        {
            // Set on focus
            ctrl.onFocus = true;
            craftCanvas.enabled = true;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ctrl.onFocus = false;
            craftCanvas.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private bool ItemChecker(string[] items, int[] amount)
    {
        // Use checker instead of grabbing things out before check
        bool[] conds = new bool[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            conds[i] = inventory.check(items[i], amount[i]);
        }
        foreach (bool cond in conds)
        {
            // if any checker failed, don't take any item
            if (!cond)
                return false;
        }

        // if all conditions are met, use the items
        for (int i = 0; i < items.Length; i++)
        {
            inventory.use(items[i], amount[i]);
        }
        return true;
    }
    public void CraftItem()
    {
        // TODO
        string[] i = { "Iron", "Copper" };
        int[] a = { 5, 3 };
        if (ItemChecker(i, a))
            inventory.putIn("SteelIngot", 2);
    }

    public void InstantiateItem()
    {
        // TODO: add object to bag, let user place it onto ground
        string[] i = { "Copper", "SteelIngot" };
        int[] a = { 3, 2 };
        if (ItemChecker(i, a))
        {
            inventory.putIn("Machine", 1);
        }
    }
}
