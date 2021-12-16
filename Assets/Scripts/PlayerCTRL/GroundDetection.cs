using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            other.transform.GetComponent<MeshRenderer>().enabled = true;
            int i = int.Parse(other.transform.parent.name.Substring(10));
            int j = int.Parse(other.transform.name.Substring(4));
            other.transform.parent.parent.GetComponent<Planet>().UpgradeResolution(i, j, 64);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            other.transform.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
