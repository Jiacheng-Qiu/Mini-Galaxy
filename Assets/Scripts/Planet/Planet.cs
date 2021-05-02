using UnityEngine;

public class Planet : MonoBehaviour
{
    // Enum for choosing which face to render
    public enum FaceRenderMask {Top, Bottom, Left, Right, Front, Back, All};
    public FaceRenderMask faceRenderMask;
    // Won't add limit onto it
    public int resolution = 16;
    public ShapeSettings shapeSetting;
    public ColorSettings colorSetting;

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColorGenerator colorGenerator = new ColorGenerator();

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    private void OnValidate()
    {
        GeneratePlanet();
    }

    private void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSetting);
        colorGenerator.UpdateSettings(colorSetting);
        terrainFaces = new TerrainFace[6];

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        // Directions of the cube faces
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < meshFilters.Length; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh" + i);
                meshObj.transform.parent = transform;
                // Using standard shader as renderer
                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
            }
            if (meshFilters[i].sharedMesh == null)
            {
                meshFilters[i].sharedMesh = new Mesh();
            }
            // Ensure that material is attached
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSetting.planetMaterial;
            // Only faces choosen will be rendered with high resolution, other faces will be of default low resolution
            bool highRender = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask == i;
            int currentRes = (highRender) ? resolution : 64;
            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, currentRes, directions[i]);
            // Can also disable rendering of other facing when stepping on planet
            // meshFilters[i].gameObject.SetActive(highRender && player.onPlanet)
        }
    }

    // Call to generate everything
    public void GeneratePlanet()
    {
        if (shapeSetting != null && colorSetting != null)
        {
            Initialize();
            GenerateMesh();
            GenerateColor();
        }
    }

    // When shape update, just update the mesh
    public void OnShapeSettingUpdated()
    {
        Initialize();
        GenerateMesh();
    }

    public void OnColorSettingUpdated()
    {
        Initialize();
        GenerateColor();
    }
    void GenerateMesh()
    {
        for (int i = 0; i < 6; i ++)
        {
            // Only construct mesh if it's activated
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
            }
        }
        // Update the elevation of ground after shape is updated
        colorGenerator.UpdateElevation(shapeGenerator.terrainElevation);
    }

    // Generate color for mesh based on settings 
    void GenerateColor()
    {
        colorGenerator.UpdateColors();
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].UpdateUVs(colorGenerator);
            }
        }
    }

    // Method designed to assign the mesh collider of planet from player g direction, the face with the smallest angle is the one
    public void GenerateCollider(Vector3 playerY)
    {
        MeshCollider groundCollider = this.transform.Find("Ground").gameObject.GetComponent<MeshCollider>();
        // Generate one collider if its not inited
        if (groundCollider == null)
        {
            groundCollider = this.transform.Find("Ground").gameObject.AddComponent<MeshCollider>();
        }
        // Find out the mesh that has smallest y angle with the player, and assign it onto collider
        float minAngle = 360;
        Mesh curMesh = null;
        for (int i = 0; i < terrainFaces.Length; i++)
        {
            float angle = Mathf.Abs(Vector3.Angle(terrainFaces[i].axisY, playerY));
            if (angle < minAngle)
            {
                minAngle = angle;
                curMesh = terrainFaces[i].mesh;
            }
        }
        if (groundCollider.sharedMaterial != curMesh)
            groundCollider.sharedMesh = curMesh;
    }
}
