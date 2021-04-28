using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyObjGravity : MonoBehaviour
{
    // Bool to stick material onto ground
    public bool locked = false;
    private Rigidbody rb;
    public GameObject planet;
    void Start()
    {
        this.rb = this.gameObject.GetComponent<Rigidbody>();
        this.gameObject.transform.parent = planet.transform.Find("Orb").transform;
    }

    void FixedUpdate()
    {
        // updates each frame and change position based on gravity
        if (!locked)
        {
            Vector3 targetDir = (transform.position - planet.transform.position).normalized;
            rb.AddForce(targetDir * -9.8f);
        } else if (!rb.freezeRotation)
        {
            rb.freezeRotation = true;
        }
    }
}