using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Random = UnityEngine.Random;

public class Revolution : MonoBehaviour
{
    public GameObject origin;
    public float originMass;
    public float orbitRadius;
    private float velocity;
    private int rotateDir;
    public void Init()
    {
        // Formular for planet travel speed: V=sqrt(G*M_star/R_orbit)
        velocity = (float)Math.Sqrt(6.67 * Math.Pow(10, -11) * originMass / orbitRadius);
        // Rotate direction decided with random
        rotateDir = (Random.value > 0.5)? 1 : -1;

        //Debug.Log("Star mass:" + star.GetComponent<StarRandomize>().mass + ", orbit:" + orbitRadius + "velocity" + velocity);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // All planets rotate around y axis on default
        this.gameObject.transform.RotateAround(origin.transform.position, new Vector3(0, 1, 0), rotateDir * 0.01f * velocity);
    }
}
