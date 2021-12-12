using UnityEngine;

public class TerrainFace
{
    ShapeGenerator shapeGenerator;
    public Mesh mesh;
    // Modificable planet resolution
    public int resolution;
    // This vector determines the top that terrain is facing
    public Vector3 axisY;
    Vector3 axisX;
    Vector3 axisZ;
    Vector3 generalAxisY;
    Vector3 generalAxisX;
    Vector3 generalAxisZ;
    int faceX;
    int faceY;

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 axisY, Vector3 generalAxis, int faceX, int faceY)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.axisY = axisY;
        this.generalAxisY = generalAxis;
        this.faceX = faceX;
        this.faceY = faceY;

        axisX = new Vector3(axisY.y, axisY.z, axisY.x);
        // Third can be found as perpendicular to both vector
        axisZ = Vector3.Cross(axisY, axisX);

        generalAxisX = new Vector3(generalAxisY.y, generalAxisY.z, generalAxisY.x);
        generalAxisZ = Vector3.Cross(generalAxisY, generalAxisX);
    }

    public void ConstructMesh()
    {
        // for a n*n points matrix, there are (n-1)^2 squares, twice as much triangles, and 6 times as much vertices
        Vector3[] vertices = new Vector3[resolution * resolution];
        // Record all vertices of triangles, 3 as a group
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        Vector2[] uv = (mesh.uv.Length == vertices.Length)? mesh.uv : new Vector2[vertices.Length];
        int index = 0;
        int triIndex = 0;
        // Initiating all squares line by line
        for (int y = 0; y < resolution; y ++)
        {
            for (int x = 0; x < resolution; x ++)
            {
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 posOnCube = generalAxisY + (faceX / 2f - 1 + percent.x / 2) * generalAxisX + (faceY / 2f - 1 + percent.y / 2) * generalAxisZ;
                Vector3 position = posOnCube.normalized;
                float unscaled = shapeGenerator.UnscaledElevation(position);
                // Create points on surface with noise layers applied
                vertices[index] = position * shapeGenerator.ScaledElevation(unscaled);
                uv[index].y = unscaled;
                // Two triangles within same square would have vertices: i, i+resolution+1, i+resolution and i, i+1, i+resolution+1
                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex++] = index;
                    triangles[triIndex++] = index + resolution + 1;
                    triangles[triIndex++] = index + resolution;
                    triangles[triIndex++] = index;
                    triangles[triIndex++] = index + 1;
                    triangles[triIndex++] = index + resolution + 1;
                }
                index++;
            }
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.uv = uv;
        }
    }

    // Generate color seperate from mesh
    public void UpdateUVs(ColorGenerator colorGenerator)
    {
        Vector2[] uv = mesh.uv;
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 posOnCube = generalAxisY + (faceX / 2f - 1 + percent.x / 2) * generalAxisX + (faceY / 2f - 1 + percent.y / 2) * generalAxisZ;
                Vector3 position = posOnCube.normalized;

                uv[x + y * resolution].x = colorGenerator.BiomePercentFromPoint(position);
            }
        }
        mesh.uv = uv;
    }
}
