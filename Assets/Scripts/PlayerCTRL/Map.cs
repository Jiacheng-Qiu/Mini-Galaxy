using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public Transform player;
    public Transform marker;
    public Planet mapPlanet;
    private PlayerMovement movement;
    private ShapeSettings shape;
    private InteractionAnimation uiAnimation;
    private Missions mission;
    //private float updatePeriod;

    private void Start()
    {
        movement = gameObject.GetComponent<PlayerMovement>();
        uiAnimation = gameObject.GetComponent<InteractionAnimation>();
        mapPlanet.gameObject.SetActive(false);
        mission = transform.GetComponent<Missions>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            uiAnimation.DisplayMap();
            UpdatePlayerPos();
            UpdateMarkers();
        }
    }

    private void FixedUpdate()
    {
        // On first frame call start on map
        if (!mapPlanet.gameObject.activeSelf)
        {
            // First check if player is on planet, if not do nothing
            if (movement.planet == null)
            {
                return;
            }
            shape = ScriptableObject.CreateInstance<ShapeSettings>();
            mapPlanet.faceRenderMask = Planet.FaceRenderMask.All;
            mapPlanet.resolution = 16;
            mapPlanet.shapeSetting = shape;
            UpdatePlanetInfo();
            mapPlanet.gameObject.SetActive(true);
        }
        /*else
        {
            // Update position of player & other obj
            if (Time.time > updatePeriod)
            {
                UpdatePlayerPos();
                updatePeriod += 2;
            }
        }*/
    }

    public void ChangeSize(float input)
    {
        mapPlanet.transform.localScale = new Vector3(input, input, input);
    }

    // Called when player enter new planet
    // TODO: fix
    public void UpdatePlanetInfo()
    {
        Planet newPlanet = movement.planet.GetComponent<Planet>();
        shape.noiseLayers = newPlanet.shapeSetting.noiseLayers;
        shape.planetRadius = newPlanet.shapeSetting.planetRadius / 100;
        mapPlanet.colorSetting = newPlanet.colorSetting;
        mapPlanet.GeneratePlanet();
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
        // Check missions markers

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
