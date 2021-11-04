using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public GameObject player;
    public Planet mapPlanet;
    public Transform camera;
    private PlayerMovement movement;
    private ShapeSettings shape;
    private InteractionAnimation uiAnimation;
    private GameObject[] markers; // TODO
    private float updatePeriod;

    private void Start()
    {
        movement = gameObject.GetComponent<PlayerMovement>();
        uiAnimation = gameObject.GetComponent<InteractionAnimation>();
        mapPlanet.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            uiAnimation.DisplayMap();
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
        else
        {
            UpdatePlayerPos();
            // Update position of player & other obj
            if (Time.time > updatePeriod)
            {
                updatePeriod += 2;
                UpdateMarkers();
            }
        }
    }

    public void ChangeSize(float input)
    {
        mapPlanet.transform.localScale = new Vector3(input, input, input);
    }

    // Called when player enter new planet
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
        player.transform.localPosition = transform.localPosition / 100;
        player.transform.LookAt(camera);
    }

    public void UpdateMarkers()
    {

    }
}
