using UnityEngine;

public class Planet : MonoBehaviour
{
    // Enum for choosing which face to render
    public ShapeSettings shapeSetting;
    public ColorSettings colorSetting;
    public bool onPreview; // SHow all terrain if planet on preview
    public int globalResolution;

    private ShapeGenerator shapeGenerator;
    private ColorGenerator colorGenerator;
    private Vector3[] directions;

    MeshFilter[,] meshFilters;
    TerrainFace[,] terrainFaces;

    private void Awake()
    {
        if (globalResolution == 0)
        {
            globalResolution = 16;
        }
        directions = new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        shapeGenerator = new ShapeGenerator();
        colorGenerator = new ColorGenerator();
        terrainFaces = new TerrainFace[6, 16];
        meshFilters = new MeshFilter[6, 16];
        GeneratePlanet();
    }

    public void GenerateMapPlanet()
    {
        onPreview = true;
        directions = new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        shapeGenerator = new ShapeGenerator();
        colorGenerator = new ColorGenerator();
        terrainFaces = new TerrainFace[6, 16];
        meshFilters = new MeshFilter[6, 16];
        Initialize();
        GenerateMesh();
        GenerateColor();
    }

    // Call to generate everything
    public void GeneratePlanet()
    {
        if (shapeSetting != null && colorSetting != null)
        {
            Initialize();
            GenerateMesh();
            GenerateColor();
            GenerateCollider();
        }
    }

    private void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSetting);
        colorGenerator.UpdateSettings(colorSetting);

        // Create water sphere
        transform.Find("Water").localScale = new Vector3(shapeSetting.planetRadius + 2, shapeSetting.planetRadius + 2, shapeSetting.planetRadius + 2);

        // Directions of the cube faces
        for (int i = 0; i < meshFilters.GetLength(0); i++)
        {
            Transform meshFolder = transform.Find("meshFolder" + i);
            if (meshFolder == null)
            {
                meshFolder = (new GameObject("meshFolder" + i)).transform;
                meshFolder.SetParent(transform);
                meshFolder.localPosition = Vector3.zero;
                meshFolder.localRotation = Quaternion.identity;
            }
            for (int j = 0; j < meshFilters.GetLength(1); j++)
            {
                if (meshFolder.Find("mesh" + j) == null)
                {
                    GameObject meshObj = new GameObject("mesh" + j);
                    meshObj.transform.parent = meshFolder;
                    meshObj.transform.localPosition = Vector3.zero;
                    meshObj.transform.localRotation = Quaternion.identity;
                    meshObj.tag = "Ground";
                    meshObj.layer = LayerMask.NameToLayer("Terrain");
                    // Using standard shader as renderer
                    meshObj.AddComponent<MeshRenderer>().enabled = onPreview;
                    meshObj.AddComponent<MeshCollider>().enabled = false;
                    meshFilters[i, j] = meshObj.AddComponent<MeshFilter>();
                    meshFilters[i, j].sharedMesh = new Mesh();
                }
                // Ensure that material is attached
                meshFilters[i, j].GetComponent<MeshRenderer>().sharedMaterial = colorSetting.planetMaterial;
                Vector3 direct = Vector3.zero;
                // The angle between direction[] and faces is given in 2d (alpha, beta). Use that to calculate the vector of all faces
                float alpha = (float)(Mathf.PI * (2 * (j / 4) - 3) * 12.5 / 180f);
                float beta = (float)(Mathf.PI * (2 * (j % 4) - 3) * 12.5 / 180f);
                float prev = Mathf.Sin(alpha);
                float cur = Mathf.Cos(alpha);
                float next = -Mathf.Cos(alpha) * Mathf.Tan(beta);

                if (directions[i].x != 0)
                {
                    direct = directions[i].x * new Vector3(cur, next, prev).normalized;
                } 
                else if (directions[i].y != 0)
                {
                    direct = directions[i].y * new Vector3(prev, cur, next).normalized;
                } 
                else
                {
                    direct = directions[i].z * new Vector3(next, prev, cur).normalized;
                }
                terrainFaces[i, j] = new TerrainFace(shapeGenerator, meshFilters[i, j].sharedMesh, globalResolution, direct, directions[i], j/4, j%4);
                // Can also disable rendering of other facing when stepping on planet
                // meshFilters[i].gameObject.SetActive(highRender && player.onPlanet)
            }
        }
    }

    public void UpgradeResolution(int i, int j, int newRes)
    {
        if (terrainFaces[i, j].resolution < newRes)
        {
            terrainFaces[i, j].resolution = newRes;
            terrainFaces[i, j].ConstructMesh();
            terrainFaces[i, j].UpdateUVs(colorGenerator);
            meshFilters[i, j].GetComponent<MeshCollider>().sharedMesh = meshFilters[i, j].sharedMesh;
        }
    }

    void GenerateMesh()
    {
        for (int i = 0; i < 6; i ++)
        {
            for (int j = 0; j < 16; j++) 
            {
                // Only construct mesh if it's activated
                if (meshFilters[i, j].gameObject.activeSelf)
                {
                    terrainFaces[i, j].ConstructMesh();
                }
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
            for (int j = 0; j < 16; j++)
            {
                if (meshFilters[i, j].gameObject.activeSelf)
                {
                    terrainFaces[i, j].UpdateUVs(colorGenerator);
                }
            }
        }
    }

    // TODO: Method designed to assign the mesh collider of planet from player g direction, the face with the smallest angle is the one
    public void GenerateCollider()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                meshFilters[i, j].GetComponent<MeshCollider>().sharedMesh = meshFilters[i, j].sharedMesh;
                meshFilters[i, j].GetComponent<MeshCollider>().enabled = true;
            }
        }
        /*// Find out the mesh that has smallest y angle with the player, and assign it onto collider
        float minAngle = 360;
        int minI = -1;
        int minJ1 = -1;
        int minJ2 = -1;
        for (int i = 0; i < 6; i++)
        {
            float angle = Mathf.Abs(Vector3.Angle(directions[i], playerY));
            if (angle <= minAngle)
            {
                minAngle = angle;
                minI = i;
            }
        }
        minAngle = 360;
        // Based on the face currently standing on, find the subface from 4 cols
        for (int j = 0; j < 4; j++)
        {
            float angle = Mathf.Abs(Vector3.Angle(terrainFaces[minI, j * 4].axisY, playerY));
            if (angle <= minAngle)
            {
                minAngle = angle;
                minJ1 = j * 4;
            }
        }
        // find the subface from 4 rows
        minAngle = 360;
        for (int k = 0; k < 4; k++)
        {
            float angle = Mathf.Abs(Vector3.Angle(terrainFaces[minI, minJ1 + k].axisY, playerY));
            if (angle <= minAngle)
            {
                minAngle = angle;
                minJ2 = k;
            }
        }
        if (curI == minI && curJ == minJ1 + minJ2)
        {
            return;
        }
        curI = minI;
        curJ = minJ1 + minJ2;
        transform.Find("Ground").GetComponent<MeshCollider>().sharedMesh = meshFilters[minI, minJ1 + minJ2].sharedMesh;
        // A easy way to get through nearest mesh: Increase collider size by frame until 9 closest mesh found
        */
    }

    public TerrainFace[,] getTerrainFaces()
    {
        return terrainFaces;
    }

    public void ChangeToPreview(bool preview)
    {
        if (onPreview == preview)
            return;
        onPreview = preview;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(!onPreview);
        }
        for (int i = 0; i < meshFilters.GetLength(0); i++)
        {
            for (int j = 0; j < meshFilters.GetLength(1); j++)
            {
                meshFilters[i, j].GetComponent<MeshRenderer>().enabled = onPreview;
                meshFilters[i, j].GetComponent<MeshCollider>().enabled = !onPreview;
            }
        }
    }

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
}
