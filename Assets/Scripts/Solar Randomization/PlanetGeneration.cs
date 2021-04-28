using System;
using UnityEngine;
using Random = UnityEngine.Random;

// Generate planets with spawn of stars, also record all info
public class PlanetGeneration : MonoBehaviour
{
    private float starSize;
    public GameObject[] planets;
    public int[] planetSize;
    Vector3[] position;
    public GameObject planetSample;

    public void Init(bool isMoon, int createAmount)
    {
        planets = new GameObject[createAmount];
        planetSize = new int[createAmount];
        position = new Vector3[createAmount];

        //TODO
        starSize = this.gameObject.transform.localScale.x;
        for (int i = 0; i < createAmount; i++)
        {
            //TODO: prefab version
            int sizeConst = (isMoon ? Random.Range(100, 150) : Random.Range(300 + 75 * i, 450 + 150 * i));

            //Generate random planet

            GameObject planet = Instantiate(planetSample);
            planet.GetComponent<Planet>().shapeSetting.planetRadius = sizeConst;
            
            /*planet.tag = "Planet";

            // Add category folders for planets
            GameObject orb = new GameObject("Orb");
            GameObject mat = new GameObject("Material");
            mat.tag = "Material";
            GameObject animal = new GameObject("Animal");
            orb.transform.parent = planet.transform;
            mat.transform.parent = planet.transform;
            animal.transform.parent = planet.transform;

            int sizeConst = (isMoon? Random.Range(100, 150) : Random.Range(300 + 75 * i, 450 + 150 * i));
            Planet script = planet.AddComponent<Planet>();
            script.shapeSetting = new ShapeSettings();
            script.shapeSetting.planetRadius = sizeConst;
            script.colorSetting = new ColorSettings();
            script.resolution = 64;

            SphereCollider atmosphere = planet.AddComponent<SphereCollider>();
            atmosphere.radius = 1.2f * sizeConst;
            atmosphere.isTrigger = true;

            // Create ground child with collider sphere
            GameObject ground = new GameObject("Ground");
            *//*ground.AddComponent<SphereCollider>();
            ground.transform.localScale = size;*//*
            ground.transform.parent = planet.transform;
            ground.transform.position = new Vector3(0, 0, 0);*/

            // All planets will be set onto the same x,z orbit plane with radius of 50-200 times star radius (TODO: 3d revolution)
            float xPos = (isMoon? Random.Range(0.8f, 1.2f) : Random.Range(1f + i, 2f + i)) * starSize;
            float zPos = (isMoon ? Random.Range(0.8f, 1.2f) : Random.Range(1f + i, 2f + i)) * starSize;
            planet.transform.position = new Vector3(this.gameObject.transform.position.x + ((Random.value > 0.5) ? 1 : -1) * xPos, 0, ((Random.value > 0.5) ? 1 : -1) * zPos + this.gameObject.transform.position.z);
            if (isMoon)
            {
                planet.transform.parent = this.transform;
            }

            // Add script for planets and enable rotations
            Rotation rotation = planet.GetComponent<Rotation>();
            Revolution revolution = planet.GetComponent<Revolution>();

            //orbit radius of planet to star
            revolution.origin = this.gameObject;
            revolution.originMass = 4 * 3.1416f * (float)Math.Pow(starSize, 3) / 3f * 1400;
            revolution.orbitRadius = (float)Math.Sqrt(Math.Pow(xPos, 2) + Math.Pow(zPos, 2));

            rotation.Init();
            revolution.Init();

            // TODOAdd moon randomize script to random amount of planets
            /*if (!isMoon && Random.value > 0.3f)
            {
                planet.AddComponent<PlanetGeneration>();
                planet.GetComponent<PlanetGeneration>().Init(true, 1);
            }*/

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
