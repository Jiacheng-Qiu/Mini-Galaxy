using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Missions : MonoBehaviour
{
    private List<Mission> missions;
    private List<int> onDisplay;
    private List<Text> displayMissions;
    public Transform missionFolder;
    private int displayLimit;
    private InteractionAnimation uiAnimation;
    private MissionUI missionUI;

    private void Start()
    {
        displayLimit = 3;
        onDisplay = new List<int>();
        missions = new List<Mission>();
        displayMissions = new List<Text>();
        missionFolder.gameObject.SetActive(false);
        for (int i = 0; i < displayLimit; i++)
        {
            displayMissions.Add(missionFolder.Find(i.ToString()).GetComponent<Text>());
        }
        missions.Add(new Mission("Learn to move around with the buttons.", "To survive you need to learn the basics: \n- WASD to move around\n- SHIFT to run\n- B to open inventory\n- [to set marker on planets\n- R to reload\n- E for equipment customization\n-C for crafting", new Vector3(0, 0, 0), 0, 0));
        missions.Add(new Mission("Try to build a craft table.", "Gather materials by shooting, build and place the craft table on the ground afterwards. Remember that you can pick it up!", new Vector3(0, 0, 0), 1, 0));

        uiAnimation = gameObject.GetComponent<InteractionAnimation>();
        missionUI = uiAnimation.missionUI.GetComponent<MissionUI>();
        missionUI.CreateButtons(missions);
    }


    public int AddToDisplay(int missionPos)
    {
        if (onDisplay.Contains(missionPos))
        {
            return RemoveFromDisplay(missionPos);
        }
        if (onDisplay.Count < displayLimit)
        {
            onDisplay.Add(missionPos);
            Display(onDisplay.Count - 1);
            return onDisplay.Count;
        }
        return -1;
        /*else
        {
            onDisplay.Insert(0, missionPos);
            RemoveFromDisplay(onDisplay[3]);
            return 1;
        }*/
    }

    public int RemoveFromDisplay(int missionPos)
    {
        int index = onDisplay.IndexOf(missionPos);
        onDisplay.RemoveAt(index);
        Display(2);
        Display(1);
        Display(0);
        if (onDisplay.Count == 0)
        {
            // When there are no missions on display, disable mission folder
            missionFolder.gameObject.SetActive(false);
        }
        return index;
    }

    public void Display(int index)
    {
        if (index >= onDisplay.Count)
        {
            StopDisplay(index);
            return;
        }
        missionFolder.gameObject.SetActive(true);
        displayMissions[index].text = (index + 1).ToString() + "." + missions[onDisplay[index]].missionName;
        displayMissions[index].gameObject.SetActive(true);
    }

    public void StopDisplay(int index)
    {
        displayMissions[index].gameObject.SetActive(false);
    }

    // TODO
    public void TaskCompleted(int index)
    {
        int pos = missions.FindIndex(a => a.serializedIndex.Equals(index));
        missions.RemoveAt(pos);
    }

    public string GetMission(int pos)
    {
        return missions[pos].missionDescription;
    }

    public List<Mission> GetMissions()
    {
        return missions;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            uiAnimation.DisplayMission();
        }
    }
}