using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Default script for all popup information
public class InformationDefault : MonoBehaviour
{

    void Update()
    {
        // All information will get distroyed after pressing esc in anycase, and won't display again
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
    }
}
