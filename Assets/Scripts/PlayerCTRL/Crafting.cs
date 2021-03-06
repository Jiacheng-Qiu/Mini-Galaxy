using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{
    private Backpack backpack;

    public GameObject buttonPrefab;
    public GameObject craftMenu;
    private CraftAmount craftAmount;
    private GameObject detailedPage;
    private GameObject[] buttons; // All buttons in order
    private Blueprint[] blueprints; // All blueprint info
    private int blueprintAmount;
    private InteractionAnimation uiInteraction;
    private int currentOnView; // Record the current viewing blueprint
    private Color warningRed;
    private float successCountdown; // Record the time craft success msg was sent

    // Read from json
    public TextAsset data;

    [System.Serializable]
    public class Blueprint
    {
        public string itemName;
        public string[] items; // Record all blueprint in string[]
        public int[] amounts; // record corresponding blueprint amount
    }

    void Start()
    {
        successCountdown = -1;
        backpack = gameObject.GetComponent<Backpack>();
        uiInteraction = gameObject.GetComponent<InteractionAnimation>();
        detailedPage = craftMenu.transform.Find("Detailed Page").gameObject;

        warningRed = new Color(1, 170f / 255, 200f / 255);

        // Load all blueprints
        blueprintAmount = 3;
        blueprints = new Blueprint[blueprintAmount];
        buttons = new GameObject[blueprintAmount];

        //blueprint[] set = JsonHelper.getJsonArray<blueprint>(data.text);
        //blueprint[] set = JsonUtility.FromJson<blueprints>(data.text).blueprints;
        //blueprint set = JsonUtility.FromJson<blueprint>(data.text);

        blueprints[0] = new Blueprint();
        blueprints[0].itemName = "Furnace";
        blueprints[0].items = new string[2];
        blueprints[0].items[0] = "Coal";
        blueprints[0].items[1] = "Wood";
        blueprints[0].amounts = new int[2];
        blueprints[0].amounts[0] = 1;
        blueprints[0].amounts[1] = 3;

        blueprints[1] = new Blueprint();
        blueprints[1].itemName = "Crafttable";
        blueprints[1].items = new string[1];
        blueprints[1].items[0] = "Wood";
        blueprints[1].amounts = new int[1];
        blueprints[1].amounts[0] = 2;

        blueprints[2] = new Blueprint();
        blueprints[2].itemName = "Rifle Scope";
        blueprints[2].items = new string[1];
        blueprints[2].items[0] = "IronIngot";
        blueprints[2].amounts = new int[1];
        blueprints[2].amounts[0] = 4;

        CreateButtons();
        string[] list = new string[2];
        list[0] = "Rifle Scope";
        list[1] = "Furnace";
        SwitchBlueprintState(list, false);

        craftAmount = detailedPage.transform.Find("Amount").Find("Input").GetComponent<CraftAmount>();
        // Make sure initial amount is always 1
        craftAmount.ResetAmount();
        OnDetailPage(1);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            uiInteraction.DisplayCraft();
            detailedPage.transform.Find("Warning Text").gameObject.SetActive(false);
            ChangeMaterialAmount();
        }
    }

    // Generate all buttons based on needs
    private void CreateButtons()
    {
        for (int i = 0; i < blueprintAmount; i++)
        {
            if (i == 0)
            {
                buttons[0] = buttonPrefab;
            } else
            {
                buttons[i] = Instantiate(buttonPrefab);
                buttons[i].transform.SetParent(buttonPrefab.transform.parent);
                buttons[i].GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
                buttons[i].GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
                buttons[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3((i % 2 == 0 ? -30 : 30), -15 - 40 * (i / 2), 0);
                buttons[i].transform.localRotation = Quaternion.identity;
                buttons[i].name = i.ToString();
            }
            buttons[i].transform.Find("Item name").GetComponent<Text>().text = blueprints[i].itemName;
            buttons[i].transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/" + blueprints[i].itemName);
            /*
            Crafting cft = this;
            int j = i;
            buttons[i].GetComponent<Button>().onClick.AddListener(() => cft.CraftItem(blueprints[j].name));*/
        }
    }

    // Shows the detailed information of current viewed blueprint
    public void OnDetailPage(int index)
    {
        if (index == currentOnView)
            return;
        detailedPage.SetActive(false);
        detailedPage.transform.Find("Item Name").GetComponent<Text>().text = blueprints[index].itemName;
        Transform list = detailedPage.transform.Find("Materials");
        for (int i = 0; i < 5; i++)
        {
            if (i < blueprints[index].items.Length)
            {
                list.Find(i.ToString()).GetComponent<Text>().text = blueprints[index].items[i];
                list.Find(i.ToString()).Find("Amount").GetComponent<Text>().text = blueprints[index].amounts[i].ToString();
                if (!backpack.Check(blueprints[index].items[i], blueprints[index].amounts[i] * craftAmount.GetCurAmount()))
                {
                    list.Find(i.ToString()).GetComponent<Text>().color = warningRed;
                    list.Find(i.ToString()).Find("Amount").GetComponent<Text>().color = warningRed;
                } else
                {
                    list.Find(i.ToString()).GetComponent<Text>().color = Color.white;
                    list.Find(i.ToString()).Find("Amount").GetComponent<Text>().color = Color.white;
                }
                list.Find(i.ToString()).gameObject.SetActive(true);
            } else
            {
                list.Find(i.ToString()).gameObject.SetActive(false);
            }
        }
        craftAmount.ResetAmount();
        detailedPage.transform.Find("Warning Text").gameObject.SetActive(false);
        detailedPage.SetActive(true);
        currentOnView = index;
    }

    // Change the material required amount based on current willing crafting amounts
    public void ChangeMaterialAmount()
    {
        Transform list = detailedPage.transform.Find("Materials");
        int curAmount = craftAmount.GetCurAmount();
        for (int i = 0; i < blueprints[currentOnView].items.Length; i++)
        {
            list.Find(i.ToString()).Find("Amount").GetComponent<Text>().text = (blueprints[currentOnView].amounts[i] * curAmount).ToString();
            if (!backpack.Check(blueprints[currentOnView].items[i], blueprints[currentOnView].amounts[i] * craftAmount.GetCurAmount()))
            {
                list.Find(i.ToString()).GetComponent<Text>().color = warningRed;
                list.Find(i.ToString()).Find("Amount").GetComponent<Text>().color = warningRed;
            }
            else
            {
                list.Find(i.ToString()).GetComponent<Text>().color = Color.white;
                list.Find(i.ToString()).Find("Amount").GetComponent<Text>().color = Color.white;
            }
        }
        detailedPage.transform.Find("Warning Text").gameObject.SetActive(false);
    }

    // Switch state of blueprint by name
    // TODO: Efficiency is super low, try to redo
    public void SwitchBlueprintState(string[] name, bool state)
    {
        foreach(string recName in name)
        {
            int pos = BlueprintFinder(recName);
            if (pos != -1)
            {
                buttons[pos].transform.Find("Avail").gameObject.SetActive(!state);
                buttons[pos].GetComponent<Button>().interactable = state;
            }
        }
    }

    // Check item amount and use items if possible
    private bool ItemChecker(string[] items, int[] amount, int multiplier)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (!backpack.Check(items[i], amount[i] * multiplier))
            {
                return false;
            }
        }
        
        for (int i = 0; i < items.Length; i++)
        {
            backpack.Use(items[i], amount[i] * multiplier);
        }
        return true;
    }

    // Find position of blueprint in array, -1 if not found
    public int BlueprintFinder(string name)
    {
        if (name == null || name == "")
        {
            return -1;
        }
        for (int i = 0; i < blueprintAmount; i++)
        {
            if (blueprints[i].itemName == name)
            {
                return i;
            }
        }
        return -1;
    }

    // Building craft items and put in inventory
    public void CraftItem()
    {
        detailedPage.transform.Find("Warning Text").gameObject.SetActive(false);
        if (ItemChecker(blueprints[currentOnView].items, blueprints[currentOnView].amounts, craftAmount.GetCurAmount()))
        {
            detailedPage.transform.Find("Success Text").gameObject.SetActive(true);
            successCountdown = Time.time;
            backpack.PutIn(blueprints[currentOnView].itemName, craftAmount.GetCurAmount());
        }
        else
        {
            // When call warning text, always disable success text
            detailedPage.transform.Find("Success Text").gameObject.SetActive(false);
            successCountdown = -1;

            detailedPage.transform.Find("Warning Text").gameObject.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        if (successCountdown != -1 && Time.time > successCountdown + 3)
        {
            successCountdown = -1;
            detailedPage.transform.Find("Success Text").gameObject.SetActive(false);
        }
    }
}
