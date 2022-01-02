using System;
using UnityEngine;
using Random = UnityEngine.Random;

// Generate planets with spawn of stars, also record all info
public class PlanetGeneration : MonoBehaviour
{
    // public Gradient ocean;
    // public Gradient land;
    // public GameObject tree;
    public GameObject planetPrefab;
    public Material waterPrefab;
    public Material atmospherePrefab;
    public ColorSettings colorPrefab;
    public ShapeSettings shapePrefab;
    private float starSize;
    public GameObject[] planets;
    public int[] planetSize;
    private Color[,] presetColors;

    private void InitColors()
    {
        presetColors = new Color[8, 3];
        presetColors[0, 0] = new Color32(74, 111, 9, 255);
        presetColors[0, 1] = new Color32(170, 96, 45, 255);
        presetColors[0, 2] = new Color32(255, 255, 255, 255);

        presetColors[1, 0] = new Color32(75, 72, 69, 255);
        presetColors[1, 1] = new Color32(138, 130, 136, 255);
        presetColors[1, 2] = new Color32(255, 255, 255, 255);

        presetColors[2, 0] = new Color32(29, 127, 42, 255);
        presetColors[2, 1] = new Color32(135, 178, 55, 255);
        presetColors[2, 2] = new Color32(176, 107, 39, 255);

        presetColors[3, 0] = new Color32(26, 13, 9, 255);
        presetColors[3, 1] = new Color32(80, 10, 6, 255);
        presetColors[3, 2] = new Color32(147, 30, 29, 255);

        presetColors[4, 0] = new Color32(91, 60, 13, 255);
        presetColors[4, 1] = new Color32(140, 79, 34, 255);
        presetColors[4, 2] = new Color32(195, 135, 32, 255);

        presetColors[5, 0] = new Color32(40, 13, 58, 255);
        presetColors[5, 1] = new Color32(30, 16, 111, 255);
        presetColors[5, 2] = new Color32(49, 32, 130, 255);
    }

    private GradientColorKey[] RandomGradient()
    {
        int selected = Random.Range(0, 6);
        GradientColorKey[] keys = new GradientColorKey[3];
        for (int i = 0; i < 3; i++)
        {
            keys[i] = new GradientColorKey();
            keys[i].color = presetColors[selected, i];
            keys[i].time = i * 0.3f + Random.Range(0, 0.2f);
        }
        return keys;
    }

    public void Init(bool isMoon, int createAmount)
    {
        InitColors();
        planets = new GameObject[createAmount];
        planetSize = new int[createAmount];

        //TODO
        starSize = this.gameObject.transform.localScale.x;
        for (int i = 0; i < createAmount; i++)
        {
            int sizeConst = (isMoon ? Random.Range(100, 150) : Random.Range(400 + 100 * i, 650 + 200 * i));

            //Generate random planet
            GameObject planet = Instantiate(planetPrefab) as GameObject;
            planet.tag = "Planet";

            // Add all components of planets
            /*SphereCollider atmosphere = planet.AddComponent<SphereCollider>();
            atmosphere.radius = 1.2f * sizeConst;
            atmosphere.isTrigger = true;*/

            Planet script = planet.GetComponent<Planet>();
            ShapeSettings shape = Instantiate(shapePrefab);
            ColorSettings color = Instantiate(colorPrefab);
            shape.planetRadius = sizeConst;
            shape.noiseLayers[0].noiseSettings.strength = Random.Range(0.05f, 0.15f);
            shape.noiseLayers[0].noiseSettings.roughness = Random.Range(2, 2.5f);
            shape.noiseLayers[0].noiseSettings.baseRoughness = Random.Range(1f, 2f);
            shape.noiseLayers[0].noiseSettings.center = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));

            color.biomeColorSettings.biomes[0].gradient.colorKeys = RandomGradient();
            color.biomeColorSettings.biomes[1].gradient.colorKeys = RandomGradient();
            color.biomeColorSettings.biomes[1].startHeight = Random.Range(0.1f, 0.2f);
            color.biomeColorSettings.biomes[2].gradient.colorKeys = RandomGradient();
            color.biomeColorSettings.biomes[2].startHeight = Random.Range(0.8f, 9f);

            script.shapeSetting = shape;
            script.colorSetting = color;
            script.Regenerate();

            Material water = Instantiate(waterPrefab);
            int[] abc = { 30, 50, 165, 200, 220, 240, 260 };
            float randomBaseColor = abc[Random.Range(0, 7)];
            Color shallow = Color.HSVToRGB(randomBaseColor / 360, 0.91f, 0.95f);
            shallow.a = 0.73f;
            Color deep = Color.HSVToRGB((randomBaseColor + 12f) / 360, 0.91f, 0.53f);
            deep.a = 0.82f;
            Color deeper = Color.HSVToRGB((randomBaseColor + 12f) / 360, 0.91f, 0.28f);
            deeper.a = 0.94f;
            water.SetColor("ShallowWaterColor", shallow);
            water.SetColor("DeepWaterColor", deep);
            water.SetColor("SuperDeepWaterColor", deeper);
            planet.transform.Find("Water").GetComponent<MeshRenderer>().material = water;

            Material atmosphere = Instantiate(atmospherePrefab);
            Color atmosColor = Color.HSVToRGB(Random.Range(0f, 0.67f), 1, 0.8f);
            atmosColor.a = 0.8f;
            atmosphere.color = atmosColor;
            planet.transform.Find("Atmosphere").GetComponent<MeshRenderer>().material = atmosphere;

            planet.GetComponent<SkyManipulation>().enabled = true;

            // All planets will be set onto the same x,z orbit plane with radius of 50-200 times star radius (TODO: 3d revolution)
            float xPos = (isMoon? Random.Range(0.8f, 1.2f) : Random.Range(1f + i, 2f + i)) * starSize;
            float zPos = (isMoon ? Random.Range(0.8f, 1.2f) : Random.Range(1f + i, 2f + i)) * starSize;
            planet.transform.position = new Vector3(this.gameObject.transform.position.x + ((Random.value > 0.5) ? 1 : -1) * xPos, 0, ((Random.value > 0.5) ? 1 : -1) * zPos + this.gameObject.transform.position.z);
            
            // Add script for planets and enable rotations
            Rotation rotation = planet.GetComponent<Rotation>();
            Revolution revolution = planet.GetComponent<Revolution>();
            //orbit radius of planet to star
            revolution.origin = this.gameObject;
            revolution.originMass = 4 * 3.1416f * (float)Math.Pow(starSize, 3) / 3f * 1400;
            revolution.orbitRadius = (float)Math.Sqrt(Math.Pow(xPos, 2) + Math.Pow(zPos, 2));
            rotation.enabled = true;
            revolution.enabled = true;


            // Add all bio randomization
            /*BioRandomize bio = planet.AddComponent<BioRandomize>();
            bio.tree1 = tree;
            bio.Init();*/

            // Add moon randomize script to random amount of planets
            /*if (isMoon)
            {
                planet.transform.parent = this.transform;
            }
            if (!isMoon && Random.value > 0.3f)
            {
                planet.AddComponent<PlanetGeneration>();
                planet.GetComponent<PlanetGeneration>().Init(true, 1);
            }

            // visible circle orbit for planet
            /*if (!isMoon)
            {
                var orbit = new GameObject { name = "Orbit" + i };
                orbit.DrawCircle((float)Math.Sqrt(Math.Pow(xPos, 2) + Math.Pow(zPos, 2)), 20f, Color.white);
            } else
            {
                var orbit = new GameObject { name = "Orbit" + i };
                orbit.transform.position = this.transform.position;
                orbit.transform.parent = this.transform;
                
                orbit.DrawCircle((float)Math.Sqrt(Math.Pow(xPos, 2) + Math.Pow(zPos, 2)), 20f, Color.white);
                
            }*/
            // Add planets into datalist
            planets[i] = planet;
            planetSize[i] = sizeConst;
        }

        // After planet generation, if this gameobject is star, inform map that planets are ready
        if (this.gameObject.name == "Star")
        {
            this.gameObject.GetComponent<StarRandomize>().planetReady = true;
        }
    }

    public bool saveData()
    {
        return false;
    }
}
