using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public Transform mapFolder;
    public GameObject dot;
    public float radarRadius;
    public float scanCD;
    
    private List<GameObject> objAround;
    private SphereCollider radar;
    private float radarStartTime;
    private Image scanner;
    private Image cdImg;

    private void Start()
    {
        objAround = new List<GameObject>();
        radar = gameObject.GetComponent<SphereCollider>();
        radarStartTime = -10f;
        scanner = mapFolder.parent.Find("Scanner").GetComponent<Image>();
        cdImg = mapFolder.parent.Find("CDBar").GetComponent<Image>();
        cdImg.enabled = false;
    }

    // Change the range of radar scanner
    public void SetRange(float range)
    {
        radar.radius = range;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && Time.time > radarStartTime + scanCD)
        {
            objAround.Clear();
            ScanNearby();
        }
    }

    private void FixedUpdate()
    {
        // While scanning is active, constantly increase radar size till reaching limit
        if (radar.enabled)
        {
            if (radar.radius < radarRadius)
            {
                radar.radius += radarRadius * Time.deltaTime;
                float len = 12f * radar.radius / radarRadius;
                scanner.rectTransform.sizeDelta = new Vector2(len, len);
            }
            else
            {
                scanner.gameObject.SetActive(false);
                radar.enabled = false;
            }
        }
        MapDisplay();

        // Update CD
        if (cdImg.enabled)
        {
            UpdateCDDisplay();
        }
    }

    // Scan all existing objects within range, show position on minimap
    public void ScanNearby()
    {
        radarStartTime = Time.time;
        radar.enabled = true;
        radar.radius = 0;
        scanner.rectTransform.sizeDelta = new Vector2(0.1f, 0.1f);
        scanner.gameObject.SetActive(true);
        mapFolder.gameObject.SetActive(true);
        cdImg.enabled = true;
    }
    // Display known environment obj from list onto minimap
    private void MapDisplay()
    {
        // Delete all previous dots
        foreach (Transform child in mapFolder)
        {
            Destroy(child.gameObject);
        }
        foreach (GameObject obj in objAround)
        {
            // Possible that object is destroyed during display
            if (obj == null)
            {
                continue;
            }

            // Calculate the angle between player and object viewing from player up axis
            Vector3 from = obj.transform.position - transform.position;
            Vector3 to = transform.forward;
            Vector3 right = Vector3.Cross(from, transform.up);
            from = Vector3.Cross(transform.up, right);
            float angle = -Mathf.Rad2Deg * Mathf.Atan2(Vector3.Dot(to, right), Vector3.Dot(to, from));

            float distance = Vector3.Magnitude(obj.transform.position - transform.position);
            // Check if the object is already out of the range, if so display on edge
            if (distance > radarRadius)
            {
                distance = radarRadius;
            }
            GameObject dotter = Instantiate(dot);
            // Adjust indicator color based on resource type
            switch (obj.GetComponent<EnvironmentComponent>().generalType)
            {
                case "Animal":
                    dotter.GetComponent<Image>().color = Color.red;
                    break;
                case "Tree":
                    dotter.GetComponent<Image>().color = Color.green;
                    break;
                case "Mine":
                    dotter.GetComponent<Image>().color = Color.cyan;
                    break;
            }
            dotter.transform.SetParent(mapFolder.transform, false);
            dotter.transform.localPosition = new Vector3(0, distance / radarRadius * 5.5f, 0);
            dotter.transform.RotateAround(mapFolder.position, mapFolder.forward, angle);
        }
    }
    // Check surrounding environment obj
    private void OnTriggerStay(Collider other)
    {
        // First check tag to determine the type, then add position to array
        if ((other.tag == "Environment" || other.tag == "Animal") && !objAround.Contains(other.gameObject))
        {
            objAround.Add(other.gameObject);
        }
    }

    private void UpdateCDDisplay()
    {
        float timeCount = Time.time - radarStartTime;
        cdImg.fillAmount = timeCount / scanCD;
        if (timeCount - scanCD >= 1)
        {
            cdImg.enabled = false;
            cdImg.fillAmount = 0;
        }
    }
    // Rotate map based on player rotation
    /*private void MapRotate()
    {
        mapFolder.transform.localRotation = Quaternion.identity; 
        mapFolder.transform.localRotation = Quaternion.Euler(0, 0, transform.parent.localRotation.eulerAngles.y);
    }*/

}
