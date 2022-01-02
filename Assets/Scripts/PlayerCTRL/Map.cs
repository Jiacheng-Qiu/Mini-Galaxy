using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Transform player;
    public Transform marker;
    public Planet mapPlanet;
    private PlayerMovement movement;
    private ShapeSettings shape;
    private InteractionAnimation uiAnimation;
    private Missions mission;
    private Planet planet;
    private bool mapUsable;

    private void Start()
    {
        movement = gameObject.GetComponent<PlayerMovement>();
        uiAnimation = gameObject.GetComponent<InteractionAnimation>();
        mapPlanet.gameObject.SetActive(false);
        mission = transform.GetComponent<Missions>();
        planet = null;
        shape = ScriptableObject.CreateInstance<ShapeSettings>();
        mapUsable = false;
    }

    private void Update()
    {
        if (mapUsable && Input.GetKeyDown(KeyCode.M))
        {
            uiAnimation.DisplayMap();
            if (uiAnimation.GetMapUIStat())
            {
                UpdatePlayerPos();
                UpdateMarkers();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!mapUsable && planet != null)
        {
            shape.noiseLayers = planet.shapeSetting.noiseLayers;
            shape.planetRadius = planet.shapeSetting.planetRadius / 100;
            mapPlanet.colorSetting = planet.colorSetting;
            mapPlanet.shapeSetting = shape;
            mapPlanet.GenerateMapPlanet();
            mapPlanet.transform.localScale = Vector3.one;
            mapPlanet.gameObject.SetActive(true);
            mapUsable = true;
        }
    }

    public void ChangeSize(float input)
    {
        mapPlanet.transform.localScale = new Vector3(input, input, input);
    }

    // Called when player enter new planet
    // TODO Check why this fails
    public void UpdatePlanetInfo(Planet newPlanet)
    {
        if (newPlanet == null)
        {
            // Stop displaying map if player leave planet while viewing
            if (uiAnimation.GetMapUIStat()) 
            {
                uiAnimation.DisplayMap();
            }
            return;
        }
        planet = newPlanet;
        mapUsable = false;
    }

    public void UpdatePlayerPos()
    {
        // Reset player to preset pos
        player.localPosition = transform.localPosition / 99;
        player.localRotation = movement.transform.localRotation;
        player.Rotate(new Vector3(90, 0, 0));

        // Always display player on the center of screen
        mapPlanet.transform.localRotation = Quaternion.FromToRotation(player.localPosition , -Vector3.forward);
    }

    public void UpdateMarkers()
    {
        for (int i = 0; i < 2; i++)
        {
            Transform t = marker.parent.Find("Mission" + i);
        }
        // Check missions markers
        List<Mission> curMissions = mission.GetMissions();
        for (int i = 0; i < 2; i++)
        {
            Transform t = marker.parent.Find("Mission" + i);
            if (i < curMissions.Count)
            {
                t.localPosition = curMissions[i].position / 99;
                t.LookAt(mapPlanet.transform);
                t.gameObject.SetActive(true);
            } 
            else
            {
                t.gameObject.SetActive(false);
            }
        }

        // Check player made marker (Only one allowed each time)
        Vector3 markerPos = movement.GetMarker();
        if (markerPos != Vector3.zero)
        {
            marker.gameObject.SetActive(true);
            marker.localPosition = movement.GetMarker() / 99;
            marker.transform.LookAt(mapPlanet.transform);
        } else
        {
            marker.gameObject.SetActive(false);
        }
    }
}
