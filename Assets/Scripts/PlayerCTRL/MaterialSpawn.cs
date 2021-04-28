using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THIS CLASS IS ONLY FOR TESTING USE
public class MaterialSpawn : MonoBehaviour
{
    public GameObject planet;
    public Vector3 radius;
    public GameObject[] materials;
    void Start()
    {
        // Set the planet for all thrown out material to be the same as current planet;
        foreach(GameObject material in materials)
            material.GetComponent<TinyObjGravity>().planet = planet;

        //materials[1].GetComponent<MaterialProperty>().materialName = "Copper";
        //materials[0].GetComponent<Transform>().GetComponent<Renderer>().sharedMaterial.color = Color.white;
    }

    public void changePlanet(GameObject planet)
    {
        this.planet = planet;
        foreach (GameObject material in materials)
            material.GetComponent<TinyObjGravity>().planet = planet;
    }
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.N))
        {
            Spawn();
        }
    }

    void Spawn()
    {
        Vector3 position = this.gameObject.transform.position + new Vector3(Random.Range(-radius.x / 2, radius.x / 2), Random.Range(0, radius.y / 2), Random.Range(-radius.z / 2, radius.z / 2));
        // Randomly init any type of materials
        Instantiate(materials[Random.Range(0, 2)], position, Quaternion.identity);
    }
}
