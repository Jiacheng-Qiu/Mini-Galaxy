using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gravitational attractor applied for only stars and planets
public class Attractor : MonoBehaviour
{
    const float G = 6.674f;
    public static List<Attractor> attractors;
    public Rigidbody rb;

    private void OnEnable()
    {
        // cond that this is the first in list
        if (attractors == null)
        {
            attractors = new List<Attractor>();
        }
        attractors.Add(this);
    }

    private void OnDisable()
    {
        attractors.Remove(this);
    }


    private void FixedUpdate()
    {
        // Do attraction calculation.
        foreach(Attractor attr in attractors)
        {
            Attract(attr);
        }
    }

    void Attract(Attractor obj)
    {
        Rigidbody rbObj = obj.rb;

        Vector3 direction = rb.position - rbObj.position;
        float distance = direction.magnitude;

        if (distance == 0)
        {
            return;
        }
        // Attraction force: F=m1*m2/d^2*G
        float forceM = (rb.mass * rbObj.mass) / Mathf.Pow(distance, 2) * G;
        Vector3 force = direction.normalized * forceM;

        rbObj.AddForce(force);
    }
}
