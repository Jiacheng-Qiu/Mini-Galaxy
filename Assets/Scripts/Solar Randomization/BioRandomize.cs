using UnityEngine;

public class BioRandomize : MonoBehaviour
{
    public int grassAmount = 3;
    public int treeAmount = 5;
    public int rockAmount = 2;
    public int stoneAmount = 1;
    public bool enableGrass = true;
    public bool enableRock = true;
    private Object[] animals;
    private Object[] oreStones;
    private Object[] trees;
    private Object[] grass;
    private Object[] rocks;
    private Planet script;
    private TerrainFace[] faces;
    private void Start()
    {
        
        script = gameObject.GetComponent<Planet>();
        Debug.Log(script.shapeSetting.planetRadius);
        faces = script.getTerrainFaces();

        // Initiate all plants on surfaces
        Transform plantFolder = transform.Find("Plant");
        trees = new Object[treeAmount];
        grass = new Object[grassAmount];
        rocks = new Object[rockAmount];

        for (int i = 0; i < treeAmount; i++)
        {
            trees[i] = Resources.Load("Plants/_Prefabs/Tree" + i);
        }
        for (int i = 0; i < rockAmount; i++)
        {
            rocks[i] = Resources.Load("Plants/_Prefabs/Rock" + i);
        }
        for (int i = 0; i < grassAmount; i++)
        {
            grass[i] = Resources.Load("Plants/_Prefabs/Grass" + i);
        }
        GenerateType(plantFolder, trees, 200, new Vector2(1, 3), new Vector2(2, 6), 5);
        if (enableRock)
            GenerateType(plantFolder, rocks, 100, new Vector2(0, 0), new Vector2(1, 1), 0);
        if (enableGrass)
            GenerateType(plantFolder, grass, 500, new Vector2(0, 0), new Vector2(5, 8), 3);

        // Initiate animals
        Transform animalFolder = transform.Find("Animal");
        animals = new Object[2];
        for (int i = 0; i < 2; i++)
        {
            animals[i] = Resources.Load("Animal" + i);
        }
        GenerateType(animalFolder, animals, 200, new Vector2(0, 0), new Vector2(1, 1), 5);

        // Initiate Mine ores
        Transform orbFolder = transform.Find("Orb");
        oreStones = new Object[stoneAmount];
        for (int i = 0; i < stoneAmount; i++)
        {
            oreStones[i] = Resources.Load("MineOres/Prefabs/Mine" + i);
        }
        GenerateType(orbFolder, oreStones, 200, new Vector2(0,0), new Vector2(1, 2), 3);
    }

    // Build any type of creatures based on needs
    public void GenerateType(Transform parentFolder, Object[] prefab, int amount, Vector2 resize, Vector2 groupSize, float randomRange)
    {
        for (int i = 0; i < amount; i++)
        {
            int onFace = (int)(i * 6f / amount);
            int vertAmount = faces[onFace].resolution * faces[onFace].resolution - 1;
            // TODO: avoid duplicated selected vertices
            Vector3 position = faces[onFace].mesh.vertices[Random.Range(0, vertAmount)] + transform.position;
            // Avoid choosing vertices on lakes or sea level
            while (Vector3.Distance(position, transform.position) <= (script.shapeSetting.planetRadius + 1))
            {
                position = faces[0].mesh.vertices[Random.Range(0, vertAmount)] + transform.position;
            }
            for (int j = 0; j < Random.Range(groupSize.x, groupSize.y); j++)
            {
                Transform creature = ((GameObject)Instantiate(prefab[Random.Range(0, prefab.Length)], position, Quaternion.identity)).transform;
                if (resize.y != 0)
                {
                    float size = Random.Range(resize.x, resize.y);
                    creature.localScale = new Vector3(size, size, size);
                }
                Quaternion onPlanetRotate = Quaternion.FromToRotation(creature.up, creature.position - transform.position) * Quaternion.AngleAxis(Random.Range(0, 359), creature.up);
                creature.rotation = onPlanetRotate;
                creature.position += Random.Range(-randomRange, randomRange) * creature.forward + Random.Range(-randomRange, randomRange) * creature.right;
                creature.parent = parentFolder;
            }
        }
    }
}
