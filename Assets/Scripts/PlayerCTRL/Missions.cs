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
    public Transform missionIconFolder;
    public Transform uiCamera;
    private Transform[] missionIcons;
    private int displayLimit;
    private InteractionAnimation uiAnimation;
    private MissionUI missionUI;
    private float planetRadius;

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
        missions.Add(new Mission("Learn to move around with the buttons.", "To survive you need to learn the basics: \n- WASD to move around\n- SHIFT to run\n- B to open inventory\n- [to set marker on planets\n- R to reload\n- E for equipment customization\n-C for crafting", new Vector3(0, 736, 13), 0, 0));
        missions.Add(new Mission("Try to build a craft table.", "Gather materials by shooting, build and place the craft table on the ground afterwards. Remember that you can pick it up!", new Vector3(0, 0, 740), 1, 0));

        uiAnimation = gameObject.GetComponent<InteractionAnimation>();
        missionUI = uiAnimation.missionUI.GetComponent<MissionUI>();
        missionUI.CreateButtons(missions);

        missionIcons = new Transform[3];
        for (int i = 0; i < 3; i++)
        {
            missionIcons[i] = missionIconFolder.Find("Mission"+i);
            missionIcons[i].gameObject.SetActive(false);
        }
        planetRadius = 0;

        missionIconFolder.gameObject.SetActive(true);
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
        List<Mission> onDisplayMissions = new List<Mission>();
        foreach(int i in onDisplay)
        {
            onDisplayMissions.Add(missions[i]);
        }
        return onDisplayMissions;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            uiAnimation.DisplayMission();
        }
    }

    // Update mission tags on player helmet based on direction
    private void ShowOnHelmet()
    {
        if (planetRadius <= 0)
        {
            return;
        }
        for (int i = 0; i < 2; i++)
        {
            if (i >= onDisplay.Count)
            {
                missionIcons[i].gameObject.SetActive(false);
            }
            else
            {
                Vector3 missionPos = missions[onDisplay[i]].position;
                float minAngle = Vector3.Angle(transform.localPosition, missionPos);
                int distance = (int)(2 * Mathf.PI * planetRadius * minAngle / 360);
                missionIcons[i].Find("Distance").GetComponent<Text>().text = distance + "m";
                missionIcons[i].localPosition = (missionPos - transform.localPosition).normalized * 5;
                missionIcons[i].rotation = Quaternion.LookRotation(missionIcons[i].localPosition);
                float scale = (distance < 300) ? 0.05f - 0.0001f * distance : 0.02f;
                missionIcons[i].localScale = new Vector3(scale, scale, scale);
                missionIcons[i].gameObject.SetActive(true);
            }
        }
    }

    public void ModifyPlanetRadius(float radius)
    {
        planetRadius = radius;
    }

    private void FixedUpdate()
    {
        ShowOnHelmet();
    }
}
