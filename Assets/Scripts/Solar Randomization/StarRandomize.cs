using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Class specified for randomization of shape/color/size/pos of star init, and generate dot light
public class StarRandomize : MonoBehaviour
{
    // Recorded data of init, will save in json
    public Vector3 size;
    public float mass;
    private Vector3 effectSize;
    public ParticleSystem corona;
    public ParticleSystem surface;

    public bool planetReady = false;

    void Start()
    {
        Random.seed = SeedSettings.seed;
        //size and collider init
        int sizeConst = Random.Range(3000, 5000);
        size = new Vector3(sizeConst, sizeConst, sizeConst);
        // Mass star = 4/3*pi*r^3 * 1400
        mass = 4 * 3.1416f * (float)Math.Pow(sizeConst, 3) / 3f * 1400;
        effectSize = new Vector3(sizeConst * 0.05f, sizeConst * 0.05f, sizeConst * 0.05f);
        this.gameObject.transform.localScale = size;
        SphereCollider collider = this.gameObject.GetComponent<SphereCollider>();
        collider.radius = 0.7f;
        collider.isTrigger = true;
        // Change particle system scale
        corona.transform.localScale = effectSize;
        surface.transform.localScale = effectSize;

        // Color control
        // Generate a random color base, and three layers of color will be randomly generated based on it
        /*int colorBase = Random.Range(30, 359);
        starBase = (colorBase + Random.Range(-30, 30)) / 360f;
        coronaBase = (colorBase + Random.Range(-30, 30)) / 360f;
        surfaceBase = (colorBase + Random.Range(-30, 30)) / 360f;

        this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.HSVToRGB(starBase, 0.5f, 0.3f));
        ParticleSystem.ColorOverLifetimeModule coronaColor = corona.colorOverLifetime;
        ParticleSystem.ColorOverLifetimeModule surfaceColor = surface.colorOverLifetime;
        coronaColor.color = Color.HSVToRGB(coronaBase, 1, 1);
        surfaceColor.color = Color.HSVToRGB(surfaceBase, 1, 1);*/

        // Run planet generation after generation of star
        this.gameObject.AddComponent<Rotation>();
        this.gameObject.GetComponent<Rotation>().Init();
        this.gameObject.GetComponent<PlanetGeneration>().Init(false, Random.Range(4, 7));
    }

    public bool SaveData()
    {
        return false;
    }
}