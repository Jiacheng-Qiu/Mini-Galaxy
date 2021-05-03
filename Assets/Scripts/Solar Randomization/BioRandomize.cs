using UnityEngine;

public class BioRandomize : MonoBehaviour
{
    public GameObject animal;
    public GameObject material;
    public GameObject tree1;
    public GameObject grass1;
    private Planet script;
    private void Start()
    {
        script = gameObject.GetComponent<Planet>();
        TerrainFace[] faces = script.getTerrainFaces();
        Transform plantFolder = transform.Find("Plant");
        Vector3 planetPos = transform.position;
        /*material.GetComponent<TinyObjGravity>().planet = this.gameObject;
        int matAmount = Random.Range(10, 30);
        int aniAmount = Random.Range(5, 10);
        for (int i = 0; i < matAmount; i++)
        {
            Vector3 position = this.gameObject.transform.position + Random.onUnitSphere * radius;
            Instantiate(material, position, Quaternion.identity);
        }
        for (int i = 0; i < aniAmount; i++)
        {
            Vector3 position = this.gameObject.transform.position + Random.onUnitSphere * radius;
            Instantiate(animal, position, Quaternion.identity);
        }*/
        // In order to make all objects generated on the surface, use mesh position for calculation

        // Build trees (individuals)
        for (int i = 0; i < 300; i++)
        {
            int onFace = i / 50;
            // Randomly choose the position from vertices of a mesh
            Vector3 position = faces[onFace].mesh.vertices[Random.Range(0, faces[onFace].resolution * faces[0].resolution)] + planetPos;
            Transform tree = Instantiate(tree1, position, Quaternion.identity).transform;
            float size = Random.Range(0.8f, 3f);
            tree.localScale = new Vector3(size, size, size);
            Quaternion onPlanetRotate = Quaternion.FromToRotation(tree.up, tree.position - transform.position) * Quaternion.AngleAxis(Random.Range(0, 359), tree.up);
            tree.rotation = onPlanetRotate;
            tree.parent = plantFolder;
        }

        // Build grass (groups)
        for (int i = 0; i < 300; i++)
        {
            int onFace = i / 50;
            // Randomly choose the position from vertices of a mesh
            Vector3 position = faces[onFace].mesh.vertices[Random.Range(0, faces[onFace].resolution * faces[0].resolution)] + planetPos;
            for (int j = 0; j < 30; j++)
            {
                Transform grass = Instantiate(grass1, position, Quaternion.identity).transform;
                Quaternion onPlanetRotate = Quaternion.FromToRotation(grass.up, grass.position - transform.position) * Quaternion.AngleAxis(Random.Range(0, 359), grass.up);
                grass.rotation = onPlanetRotate;
                grass.parent = plantFolder;
            }
        }
    }
}
