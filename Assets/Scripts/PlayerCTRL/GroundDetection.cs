using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    private Transform curPlanet;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            other.transform.GetComponent<MeshRenderer>().enabled = true;
            int i = int.Parse(other.transform.parent.name.Substring(10));
            int j = int.Parse(other.transform.name.Substring(4));
            curPlanet = other.transform.parent.parent;
            other.transform.parent.parent.GetComponent<Planet>().UpgradeResolution(i, j, 64);
            curPlanet.GetComponent<BioRandomize>().EnableSector(int.Parse(other.transform.parent.name.Substring(10)), int.Parse(other.name.Substring(4)));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            other.transform.GetComponent<MeshRenderer>().enabled = false;
            curPlanet.GetComponent<BioRandomize>().SetSectorState(false, int.Parse(other.transform.parent.name.Substring(10)), int.Parse(other.name.Substring(4)));
        }
    }
}
