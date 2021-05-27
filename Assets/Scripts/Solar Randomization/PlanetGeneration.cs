using System;
using UnityEngine;
using Random = UnityEngine.Random;

// Generate planets with spawn of stars, also record all info
public class PlanetGeneration : MonoBehaviour
{
    public Gradient ocean;
    public Gradient land;
    public GameObject tree;
    public Material planetMaterial;
    private float starSize;
    public GameObject[] planets;
    public int[] planetSize;

    public void Init(bool isMoon, int createAmount)
    {
        planets = new GameObject[createAmount];
        planetSize = new int[createAmount];

        //TODO
        starSize = this.gameObject.transform.localScale.x;
        for (int i = 0; i < createAmount; i++)
        {
            int sizeConst = (isMoon ? Random.Range(100, 150) : Random.Range(400 + 100 * i, 650 + 200 * i));

            //Generate random planet
            GameObject planet = new GameObject("Planet " + i);
            planet.tag = "Planet";

            // Add all components of planets
            SphereCollider atmosphere = planet.AddComponent<SphereCollider>();
            atmosphere.radius = 1.2f * sizeConst;
            atmosphere.isTrigger = true;
            GameObject ground = new GameObject("Ground");
            GameObject orb = new GameObject("Orb");
            GameObject mat = new GameObject("Material");
            mat.tag = "Material";
            GameObject animal = new GameObject("Animal");
            GameObject plant = new GameObject("Plant");
            ground.transform.parent = planet.transform;
            orb.transform.parent = planet.transform;
            mat.transform.parent = planet.transform;
            animal.transform.parent = planet.transform;
            plant.transform.parent = planet.transform;



            // Add terrain generation code
            Planet script = planet.AddComponent<Planet>();
            script.faceRenderMask = Planet.FaceRenderMask.All;
            script.resolution = 64;
            
            ShapeSettings shape = ScriptableObject.CreateInstance<ShapeSettings>();
            ColorSettings color = ScriptableObject.CreateInstance<ColorSettings>();
            shape.planetRadius = sizeConst;
            shape.InitNoiseArr(1);
            shape.noiseLayers[0].easyInput(NoiseSettings.FilterType.Simple, new Vector3(Random.Range(0, 5f), Random.Range(0, 5f), Random.Range(0, 5f)));
            color.oceanColor = ocean;
            color.planetMaterial = planetMaterial;
            color.biomeColorSettings.InitBiomeArr(1, land);
            // TODO Random generate gradients for biome layers, and random noise value for land

            script.shapeSetting = shape;
            script.colorSetting = color;
            script.GeneratePlanet();


            // All planets will be set onto the same x,z orbit plane with radius of 50-200 times star radius (TODO: 3d revolution)
            float xPos = (isMoon? Random.Range(0.8f, 1.2f) : Random.Range(1f + i, 2f + i)) * starSize;
            float zPos = (isMoon ? Random.Range(0.8f, 1.2f) : Random.Range(1f + i, 2f + i)) * starSize;
            planet.transform.position = new Vector3(this.gameObject.transform.position.x + ((Random.value > 0.5) ? 1 : -1) * xPos, 0, ((Random.value > 0.5) ? 1 : -1) * zPos + this.gameObject.transform.position.z);
            // Add script for planets and enable rotations
            Rotation rotation = planet.AddComponent<Rotation>();
            Revolution revolution = planet.AddComponent<Revolution>();
            //orbit radius of planet to star
            revolution.origin = this.gameObject;
            revolution.originMass = 4 * 3.1416f * (float)Math.Pow(starSize, 3) / 3f * 1400;
            revolution.orbitRadius = (float)Math.Sqrt(Math.Pow(xPos, 2) + Math.Pow(zPos, 2));
            rotation.Init();
            revolution.Init();


            // TODO temp file for dealing with collision
            planet.AddComponent<TempReaction>();
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
