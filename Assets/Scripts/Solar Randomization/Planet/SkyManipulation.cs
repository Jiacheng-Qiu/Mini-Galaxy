using UnityEngine;

// Generation of multiple layers of cloud to make it 3D and create atmosphere around planets
public class SkyManipulation : MonoBehaviour
{
    public int stackAmount = 30;
    public float stackOffset = 0.1f;
    private float planetRadius;
    // public Mesh quadMesh;
    // public Material cloudMaterial;
    public Transform cloudFolder;
    public GameObject cloudObject;
    public GameObject atmosphere;

    private void Start()
    {
        planetRadius = gameObject.GetComponent<Planet>().shapeSetting.planetRadius;
        // Cloud part
        float curHeight = 0;
        for (int i = 0; i < stackAmount; i++)
        {
            GameObject child = Instantiate(cloudObject, cloudFolder);
            child.name = "Cloud" + i;
            child.transform.localScale = Vector3.one * (planetRadius + 120 * planetRadius / 500 - curHeight);
            child.transform.localPosition -= new Vector3(0, curHeight, 0);
            child.GetComponent<Renderer>().material.SetFloat("CloudHeight", (float)i / stackAmount);
            // Graphics.DrawMesh(quadMesh, Matrix4x4.TRS(transform.position - transform.up * curHeight, transform.rotation, transform.localScale - new Vector3(stackOffset, stackOffset, stackOffset)), cloudMaterial, 0);
            curHeight += stackOffset;
        }

        // Atmosphere part
        atmosphere.transform.localScale = Vector3.one * (planetRadius + 150 * planetRadius / 500);
    }

    // Reduce stack amount for far away planets to optimize graphics performance
    public void UpdateCloudVolumn(int newStack)
    {
        // Reducing
        foreach (Transform child in cloudFolder)
        {
            if (stackAmount < newStack)
            {
                break;
            } 
            else if (stackAmount == newStack)
            {
                return;
            }
            Destroy(child.gameObject);
            stackAmount--;
        }
        // Adding
        for (int i = 0; i < stackAmount; i ++)
        {
            cloudFolder.Find("Cloud" + i).GetComponent<Renderer>().material.SetFloat("CloudHeight", (float)i / newStack);
        }
        float curHeight = stackAmount * stackOffset;
        float maxHeight = planetRadius + 120 * planetRadius / 500;
        for (int i = stackAmount; i < newStack; i++)
        {
            GameObject child = Instantiate(cloudObject, cloudFolder);
            child.name = "Cloud" + i;
            child.transform.localScale = Vector3.one * (maxHeight - curHeight);
            child.transform.localPosition -= new Vector3(0, curHeight, 0);
            child.GetComponent<Renderer>().material.SetFloat("CloudHeight", (float)i / newStack);
            // Graphics.DrawMesh(quadMesh, Matrix4x4.TRS(transform.position - transform.up * curHeight, transform.rotation, transform.localScale - new Vector3(stackOffset, stackOffset, stackOffset)), cloudMaterial, 0);
            curHeight += stackOffset;
        }
    }
}
