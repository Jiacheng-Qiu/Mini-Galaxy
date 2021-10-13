using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public Canvas minimap;
    private Transform panel;
    public GameObject star;
    public Sprite planetSprite;
    public Sprite playerSprite;
    private PlayerMovement ctrl;

    private GameObject player;
    private GameObject[] planets;
    private ArrayList moonImg;
    private GameObject[] planetImg;

    private bool inited = false;
    public void Init()
    {
        planets = star.GetComponent<PlanetGeneration>().planets;
        int[] planetSize = star.GetComponent<PlanetGeneration>().planetSize;
        minimap.enabled = false;
        ctrl = this.gameObject.GetComponent<PlayerMovement>();
        panel = minimap.transform.Find("MapPanel");
        moonImg = new ArrayList();

        // Create sprite for all planets
        planetImg = new GameObject[planets.Length];
        for (int i = 0; i < planets.Length; i++)
        {
            GameObject planet = new GameObject("Planet");
            Image img = planet.AddComponent<Image>();
            img.sprite = planetSprite;
            planet.transform.SetParent(panel);
            planet.transform.localPosition = new Vector3(0, 0, 0);
            planet.transform.localScale = new Vector3(0.00005f * planetSize[i], 0.00005f * planetSize[i], 1);
            planetImg[i] = planet;

            // planet can have moons, take their position
            PlanetGeneration check = planets[i].GetComponent<PlanetGeneration>();
            if (check != null)
            {
                GameObject moon = new GameObject("Moon");
                Image image = moon.AddComponent<Image>();
                image.sprite = planetSprite;
                moon.transform.SetParent(panel);
                moon.transform.localPosition = new Vector3(0, 0, 0);
                moon.transform.localScale = new Vector3(0.00008f * check.planetSize[0], 0.00008f * check.planetSize[0], 1);
                moonImg.Add(moon);
            }
        }

        // Create a sprite for player, create last so it will always display on top
        player = new GameObject("Player");
        Image playerImg = player.AddComponent<Image>();
        playerImg.sprite = playerSprite;
        player.transform.SetParent(panel);
        player.transform.localPosition = new Vector3(0, 0, 0);
        player.transform.localScale = new Vector3(0.02f, 0.02f, 1);
    }

    // Update map information based on player position
    private void FixedUpdate()
    {
        if (inited && minimap.enabled)
        {
            // Grab position and facing of player, update player last so it will always be on top
            player.transform.localPosition = new Vector3(this.transform.position.x / 800f, this.transform.position.z / 800f, 0);
            //player.transform.rotate = this.transform.rotation.x;
            // In reality, player is moving along y axis, while on map it should be along z axis
            player.transform.eulerAngles = new Vector3(0, 0, this.transform.rotation.eulerAngles.y);

            int j = 0;
            // Grab position of each planet by frame, and stick onto map
            for (int i = 0; i < planets.Length; i++)
            {
                planetImg[i].transform.localPosition = new Vector3(planets[i].transform.position.x / 800f, planets[i].transform.position.z / 800f, 0);
                // Planet can also have moons, take their position
                PlanetGeneration check = planets[i].GetComponent<PlanetGeneration>();
                if (check != null)
                {
                    ((GameObject)moonImg[j]).transform.localPosition = new Vector3(check.planets[0].transform.position.x / 750f, check.planets[0].transform.position.z / 750f, 0);
                    j++;
                }
            }
        }
        else if (!inited && star.GetComponent<StarRandomize>().planetReady)
        {
            Init();
            inited = true;
        }
    }

    void Update()
    {
        /*if (Input.GetKeyUp(KeyCode.M))
        {
            // Set on focus
            ctrl.onFocus = true;
            minimap.enabled = true;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ctrl.onFocus = false;
            minimap.enabled = false;
        }*/
    }
}
