using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Guide players through all possible UI elements intelligently.
public class PlayerGuiding : MonoBehaviour
{
    private List<Instruction> instructions;
    private List<GameObject> instObj;
    public class Instruction
    {
        public string key;
        public string instName;
        public bool called;
        public List<Instruction> specialInst;
        public Instruction(string key, string instName, bool called, bool isNull)
        {
            this.key = key;
            this.instName = instName;
            this.called = called;
            specialInst = (isNull)? null : new List<Instruction>();
        }
    }
    
    private void Start()
    {
        instObj = new List<GameObject>();
        instObj.Add(transform.Find("Guide0").gameObject);
        instObj.Add(transform.Find("Guide1").gameObject);
        instObj.Add(transform.Find("Guide2").gameObject);

        instructions = new List<Instruction>();
        instructions.Add(new Instruction("L", "Flashlight", false, true));
        instructions.Add(new Instruction("J", "Mission", false, false));
        instructions[1].specialInst.Add(new Instruction("M1", "Choose Mission", false, true));
        instructions[1].specialInst.Add(new Instruction("J", "Close mission", false, true));

        instructions.Add(new Instruction("B", "Backpack", false, false));
        instructions[2].specialInst.Add(new Instruction("M1", "Item Submenu", false, true));
        instructions[2].specialInst.Add(new Instruction("B", "Close inventory", false, true));

        instructions.Add(new Instruction("C", "Craft menu", false, false));
        instructions[3].specialInst.Add(new Instruction("M1", "Select item", false, true));
        instructions[3].specialInst.Add(new Instruction("C", "Close craft menu", false, true));

        instructions.Add(new Instruction("I", "Weapon customize", false, false));
        instructions[4].specialInst.Add(new Instruction("M1", "Select part", false, true));
        instructions[4].specialInst.Add(new Instruction("I", "Close menu", false, true));

        instructions.Add(new Instruction("M", "Map", false, false));
        instructions[5].specialInst.Add(new Instruction("M1", "Scroll planet", false, true));
        instructions[5].specialInst.Add(new Instruction("M", "Close map", false, true));

        instructions.Add(new Instruction("E", "Place Item", false, false));
        instructions[6].specialInst.Add(new Instruction("M1", "Place", false, true));
        instructions[6].specialInst.Add(new Instruction("E", "Cancel Place", false, true));

        instructions.Add(new Instruction("T", "Dismantle", false, false));
        instructions[7].specialInst.Add(new Instruction("M1", "Dismantle", false, true));
        instructions[7].specialInst.Add(new Instruction("T", "Cancel", false, true));

        instructions.Add(new Instruction("K", "Scan nearby", false, true));

        instructions.Add(new Instruction("F", "Interact", false, true));
        instructions.Add(new Instruction("G", "Throw", false, true));

        DisplayChange("", false);
    }

    // Display depends on current state and what haven't been displayed
    public void DisplayChange(string key, bool state)
    {
        // If one ui is ongoing, show its submenu, else show 3- functionalities that haven't been used
        if (state)
        {
            int index = FindInstructionIndex(key);
            for (int i = 0; i < instructions[index].specialInst.Count; i++)
            {
                foreach (Transform child in instObj[i].transform)
                {
                    child.gameObject.SetActive(false);
                }
                switch (instructions[index].specialInst[i].key)
                {
                    case "M1":
                        instObj[i].transform.Find("MouseL").gameObject.SetActive(true);
                        break;
                    case "M2":
                        instObj[i].transform.Find("MouseR").gameObject.SetActive(true);
                        break;
                    case "M3":
                        instObj[i].transform.Find("MouseM").gameObject.SetActive(true);
                        break;
                    default:
                        instObj[i].transform.Find("Button").GetComponent<Text>().text = instructions[index].key;
                        instObj[i].transform.Find("Button").gameObject.SetActive(true);
                        break;
                }
                instObj[i].GetComponent<Text>().text = instructions[index].specialInst[i].instName;
                instObj[i].SetActive(true);
            }
            for (int i = instructions[index].specialInst.Count; i < 3; i++)
            {
                instObj[i].SetActive(false);
            }
        } 
        else
        {
            int index = 0;
            for (int i = 0; i < 3; i++)
            {
                if (index == -1)
                {
                    instObj[i].SetActive(false);
                    continue;
                }
                foreach(Transform child in instObj[i].transform)
                {
                    child.gameObject.SetActive(false);
                }
                switch (instructions[index].key)
                {
                    case "M1":
                        instObj[i].transform.Find("MouseL").gameObject.SetActive(true);
                        break;
                    case "M2":
                        instObj[i].transform.Find("MouseR").gameObject.SetActive(true);
                        break;
                    case "M3":
                        instObj[i].transform.Find("MouseM").gameObject.SetActive(true);
                        break;
                    default:
                        instObj[i].transform.Find("Button").GetComponent<Text>().text = instructions[index].key;
                        instObj[i].transform.Find("Button").gameObject.SetActive(true);
                        break;
                }
                instObj[i].GetComponent<Text>().text = instructions[index].instName;
                instObj[i].SetActive(true);

                index = (index >= 0) ? FindInstructionNotUsed(index + 1) : -1;
            }
        }
    }

    public void DisplayUsed(string key)
    {
        instructions[FindInstructionIndex(key)].called = true;
    }

    private int FindInstructionIndex(string key)
    {
        for (int i = 0; i < instructions.Count; i++)
        {
            if (instructions[i].key == key)
            {
                return i;
            }
        }
        return -1;
    }

    private int FindInstructionNotUsed(int startIndex)
    {
        for (int i = startIndex; i < instructions.Count; i++)
        {
            if (!instructions[i].called)
            {
                return i;
            }
        }
        return -1;
    }
}
