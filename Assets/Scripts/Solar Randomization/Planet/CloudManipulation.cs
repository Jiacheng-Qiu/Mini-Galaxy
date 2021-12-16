using UnityEngine;

// Generation of multiple layers of cloud to make it 3D
public class CloudManipulation : MonoBehaviour
{
    public int stackAmount = 30;
    public float stackOffset = 0.1f;
    // public Mesh quadMesh;
    // public Material cloudMaterial;
    public Transform cloudFolder;
    public GameObject cloudObject;

    private void Start()
    {
        float curHeight = 0;
        for (int i = 0; i < stackAmount; i++)
        {
            GameObject child = Instantiate(cloudObject, cloudFolder);
            child.transform.localScale = Vector3.one * (820 - curHeight);
            child.transform.localPosition -= new Vector3(0, curHeight, 0);
            child.GetComponent<Renderer>().material.SetFloat("CloudHeight", (float)i / stackAmount);
            // Graphics.DrawMesh(quadMesh, Matrix4x4.TRS(transform.position - transform.up * curHeight, transform.rotation, transform.localScale - new Vector3(stackOffset, stackOffset, stackOffset)), cloudMaterial, 0);
            curHeight += stackOffset;
        }
    }
}
