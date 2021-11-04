using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Missions : MonoBehaviour
{
    private List<string> missions;
    private List<int> onDisplay;
    private List<Text> displayMissions;
    public Transform missionFolder;
    private int displayLimit;
    private InteractionAnimation uiAnimation;

    private void Start()
    {
        displayLimit = 3;
        onDisplay = new List<int>();
        missions = new List<string>();
        displayMissions = new List<Text>();
        for (int i = 0; i < displayLimit; i++)
        {
            displayMissions.Add(missionFolder.Find(i.ToString()).GetComponent<Text>());
        }
        missions.Add("Learn to move around with the buttons.");
        missions.Add("Try to gather materials to build.");
        missions.Add("Try out everything you are interested!");

        uiAnimation = gameObject.GetComponent<InteractionAnimation>();
    }
    public void AssignNewTask(string text)
    {
        missions.Add(text);
    }

    public void AddToDisplay(int missionPos)
    {
        if (onDisplay.Contains(missionPos))
        {
            RemoveFromDisplay(missionPos);
            return;
        }
        if (onDisplay.Count < displayLimit)
        {
            onDisplay.Add(missionPos);
            Display(onDisplay.Count - 1);
        } else
        {
            onDisplay.Insert(0, missionPos);
            RemoveFromDisplay(onDisplay[3]);
        }
    }

    public void RemoveFromDisplay(int missionPos)
    {
        int index = onDisplay.IndexOf(missionPos);
        onDisplay.RemoveAt(index);
        Display(2);
        Display(1);
        Display(0);
    }

    public void Display(int index)
    {
        if (index >= onDisplay.Count)
        {
            StopDisplay(index);
            return;
        }
        displayMissions[index].text = index.ToString() + "." + missions[onDisplay[index]];
        displayMissions[index].gameObject.SetActive(true);
    }

    public void StopDisplay(int index)
    {
        displayMissions[index].gameObject.SetActive(false);
    }

    public void TaskCompleted(string text)
    {
        int pos = missions.IndexOf(text);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            uiAnimation.DisplayMission();
        }
    }
}
