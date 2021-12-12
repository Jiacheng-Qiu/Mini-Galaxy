using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceStation : MonoBehaviour
{
    public Transform planet;

    private void FixedUpdate()
    {
        transform.RotateAround(planet.position, Vector3.right, -0.2f);
        transform.LookAt(planet);
    }
}
