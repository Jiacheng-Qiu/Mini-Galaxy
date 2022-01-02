using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Revolution : MonoBehaviour
{
    public GameObject origin;
    public float originMass;
    public float orbitRadius;
    private float velocity;
    private int rotateDir;

    private void Start()
    {
        // Formular for planet travel speed: V=sqrt(G*M_star/R_orbit)
        velocity = (float)Math.Sqrt(6.67 * Math.Pow(10, -11) * originMass / orbitRadius);
        // Rotate direction decided with random
        rotateDir = (Random.value > 0.5) ? 1 : -1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // All planets rotate around y axis on default
        transform.RotateAround(origin.transform.position, Vector3.up, rotateDir * 0.1f * Time.deltaTime);
    }
}
