using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    private List<GameObject> objAround;
    private SphereCollider radar;
    private float radarStartTime;
    public Transform mapFolder;
    public GameObject dot;

    private void Start()
    {
        objAround = new List<GameObject>();
        radar = gameObject.GetComponent<SphereCollider>();
        radarStartTime = -10f;
    }

    // Change the range of radar scanner
    public void SetRange(float range)
    {
        radar.radius = range;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && Time.time > radarStartTime + 10f)
        {
            ScanNearby();
            Debug.Log("Scanning");
        }
    }

    private void FixedUpdate()
    {

        // Five seconds after scanning, disable minimap and delete other obj position
        if (Time.time > radarStartTime + 5f)
        {
            mapFolder.gameObject.SetActive(false);
            objAround.Clear();
        }
        // After rader scans for a while, disable it
        else
        {
            if (Time.time > radarStartTime + 0.5f)
            {
                radar.enabled = false;
            }
            MapDisplay();
            MapRotate();
        }
    }

    // Scan all existing objects within range, show position on minimap
    public void ScanNearby()
    {
        radarStartTime = Time.time;
        radar.enabled = true;
        MapDisplay();
        mapFolder.gameObject.SetActive(true);
    }

    private void MapDisplay()
    {
        // Delete all previous dots
        foreach (Transform child in mapFolder)
        {
            Destroy(child.gameObject);
        }

        foreach (GameObject pos in objAround)
        {
            float xDist = pos.transform.localPosition.x - transform.position.x;
            float yDist = pos.transform.localPosition.y - transform.position.y;
            // First check if the object is already out of the range, if so display on edge
            float distFromCenter = Mathf.Sqrt(Mathf.Pow(xDist, 2) + Mathf.Pow(yDist, 2));
            if (distFromCenter > radar.radius)
            {
                xDist *= radar.radius / distFromCenter;
                yDist *= radar.radius / distFromCenter;
            }
            GameObject dotter = Instantiate(dot);
            dotter.transform.SetParent(mapFolder.transform, false);
            dotter.transform.localPosition = new Vector3(xDist / radar.radius * 5.5f, yDist / radar.radius * 5.5f, 0);
        }
    }

    // Rotate map based on player rotation
    private void MapRotate()
    {
        mapFolder.transform.localRotation = Quaternion.identity; 
        mapFolder.transform.localRotation = Quaternion.Euler(0, 0, transform.parent.localRotation.eulerAngles.y);
    }

    private void OnTriggerStay(Collider other)
    {
        // First check tag to determine the type, then add position to array
        if (other.tag == "Environment" && !objAround.Contains(other.gameObject))
        {
            objAround.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Environment")
        {
            objAround.Remove(other.gameObject);
        }
    }


}
