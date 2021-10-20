using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{
    public Canvas craftCanvas;
    private PlayerInventory inventory;
    private PlayerMovement movement;

    public GameObject buttonPrefab;
    private GameObject[] buttons; // All buttons in order
    private Recipe[] recipies; // All Recipe info
    private int RecipeAmount;

    // Read from json
    public TextAsset data;

    [System.Serializable]
    public class Recipe
    {
        public string name;
        public string[] items; // Record all Recipe in string[]
        public int[] amounts; // record corresponding Recipe amount
    }

    void Start()
    {
        craftCanvas.enabled = false;
        inventory = this.gameObject.GetComponent<PlayerInventory>();
        movement = this.gameObject.GetComponent<PlayerMovement>();

        // Load all recipies
        RecipeAmount = 3;
        recipies = new Recipe[RecipeAmount];
        buttons = new GameObject[RecipeAmount];

        //Recipe[] set = JsonHelper.getJsonArray<Recipe>(data.text);
        //Recipe[] set = JsonUtility.FromJson<Recipes>(data.text).recipes;
        //Recipe set = JsonUtility.FromJson<Recipe>(data.text);

        recipies[0] = new Recipe();
        recipies[0].name = "Furnace";
        recipies[0].items = new string[2];
        recipies[0].items[0] = "Coal";
        recipies[0].items[1] = "Wood";
        recipies[0].amounts = new int[2];
        recipies[0].amounts[0] = 1;
        recipies[0].amounts[1] = 1;

        recipies[1] = new Recipe();
        recipies[1].name = "Crafttable";
        recipies[1].items = new string[1];
        recipies[1].items[0] = "Wood";
        recipies[1].amounts = new int[1];
        recipies[1].amounts[0] = 1;

        recipies[2] = new Recipe();
        recipies[2].name = "Spaceship";
        recipies[2].items = new string[2];
        recipies[2].items[0] = "IronIngot";
        recipies[2].items[1] = "AluminumIngot";
        recipies[2].amounts = new int[2];
        recipies[2].amounts[0] = 1;
        recipies[2].amounts[1] = 1;

        CreateButtons();
        string[] list = new string[2];
        list[0] = "Spaceship";
        list[1] = "Furnace";
        SwitchRecipeState(list, false);

    }

    // Generate all buttons based on needs
    private void CreateButtons()
    {
        for (int i = 0; i < RecipeAmount; i++)
        {
            buttons[i] = Instantiate(buttonPrefab);
            buttons[i].transform.SetParent(craftCanvas.transform);
            buttons[i].transform.localPosition = new Vector3(200, -200 + 105 * i, 0);
            buttons[i].transform.Find("ItemName").GetComponent<Text>().text = recipies[i].name;
            buttons[i].transform.Find("Item1").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/" + recipies[i].items[0]);
            buttons[i].transform.Find("Text1").GetComponent<Text>().text = recipies[i].amounts[0].ToString();
            if (recipies[i].items.Length > 1)
            {
                buttons[i].transform.Find("Item2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/" + recipies[i].items[1]);
                buttons[i].transform.Find("Text2").GetComponent<Text>().text = recipies[i].amounts[1].ToString();
            } else
            {
                buttons[i].transform.Find("Item2").gameObject.SetActive(false);
                buttons[i].transform.Find("Text2").gameObject.SetActive(false);
                buttons[i].transform.Find("Add sign").gameObject.SetActive(false);
            }
            Crafting cft = this;
            int j = i;
            buttons[i].GetComponent<Button>().onClick.AddListener(() => cft.CraftItem(recipies[j].name));
        }
    }

    // Switch state of Recipe by name
    public void SwitchRecipeState(string[] name, bool state)
    {
        foreach(string recName in name)
        {
            int pos = RecipeFinder(recName);
            if (pos != -1)
            {
                buttons[pos].GetComponent<Button>().interactable = state;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            // Set on focus
            movement.ChangeCursorFocus(true);
            craftCanvas.enabled = true;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            movement.ChangeCursorFocus(false);
            craftCanvas.enabled = false;
        }
    }

    // Check item amount and use items if possible
    private bool ItemChecker(string[] items, int[] amount)
    {
        // Use checker instead of grabbing things out before check
        bool cond;
        for (int i = 0; i < items.Length; i++)
        {
            cond = inventory.Check(items[i], amount[i]);
            if (!cond)
            {
                return false;
            }
        }
        for (int i = 0; i < items.Length; i++)
        {
            inventory.use(items[i], amount[i]);
        }
        return true;
    }

    // Find position of Recipe in array, -1 if not found
    public int RecipeFinder(string name)
    {
        if (name == null || name == "")
        {
            return -1;
        }
        for (int i = 0; i < RecipeAmount; i++)
        {
            if (recipies[i].name == name)
            {
                return i;
            }
        }
        return -1;
    }

    // Building craft items and put in inventory
    public void CraftItem(string name)
    {
        int pos = RecipeFinder(name);
        if (ItemChecker(recipies[pos].items, recipies[pos].amounts))
            inventory.putIn(name, 1);
        else
            Debug.Log("Crafting failed!");
    }
}
