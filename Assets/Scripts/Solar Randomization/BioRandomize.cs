using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioRandomize : MonoBehaviour
{
    public GameObject animal;
    public GameObject material;
    private float radius = 30;
    void Start()
    {
        radius = this.gameObject.GetComponent<Planet>().shapeSetting.planetRadius;
        //this.radius = this.gameObject.GetComponent<ShapeSettings>().planetRadius;
        material.GetComponent<TinyObjGravity>().planet = this.gameObject;
        int matAmount = Random.Range(10, 30);
        int aniAmount = Random.Range(5, 10);
        for (int i = 0; i < matAmount; i++)
        {
            Vector3 position = this.gameObject.transform.position + Random.onUnitSphere * radius;
            Instantiate(material, position, Quaternion.identity);
        }
        for (int i = 0; i < aniAmount; i++)
        {
            Vector3 position = this.gameObject.transform.position + Random.onUnitSphere * radius;
            Instantiate(animal, position, Quaternion.identity);
        }
    }
}
