using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    private Text description;
    public Missions mission;
    private int onDisplay;
    private Transform missionFolder;

    private void Awake()
    {
        description = transform.Find("Details").GetComponent<Text>();
        missionFolder = transform.Find("scrollview").Find("viewport").Find("content");
    }

    public void CreateButtons(List<Mission> missions)
    {
        GameObject sample = missionFolder.Find("0").gameObject;
        sample.transform.Find("Content").GetComponent<Text>().text = missions[0].missionName;
        for (int i = 1; i < missions.Count; i++)
        {
            GameObject newMission = Instantiate(sample) as GameObject;
            newMission.name = i.ToString();
            newMission.transform.Find("Content").GetComponent<Text>().text = missions[i].missionName;
            newMission.transform.SetParent(missionFolder);
            newMission.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            newMission.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            newMission.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, -25 - 50 * i, 0);
            newMission.transform.localRotation = Quaternion.identity;
        }
    }

    public void SwitchOnDisplay(int pos)
    {
        onDisplay = pos;
        description.text = mission.GetMission(onDisplay);
    }

    public void SwitchMark()
    {
        int result = mission.AddToDisplay(onDisplay);
        if (result != -1)
        {
            Image mark = missionFolder.Find(onDisplay.ToString()).Find("Marked").GetComponent<Image>();
            mark.enabled = !mark.enabled;
        }
    }
}
