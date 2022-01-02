using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyObjGravity : MonoBehaviour
{
    // Bool to stick material onto ground
    public bool locked = false;
    private Rigidbody rb;
    public Transform planet;
    private void Start()
    {
        this.rb = gameObject.GetComponent<Rigidbody>();
        transform.SetParent(planet.Find("Material").transform);
    }

    void FixedUpdate()
    {
        // updates each frame and change position based on gravity
        if (!locked)
        {
            Vector3 targetDir = (transform.position - planet.position).normalized;
            rb.AddForce(targetDir * -9.8f);
        } else if (!rb.freezeRotation)
        {
            rb.freezeRotation = true;
        }
    }
}