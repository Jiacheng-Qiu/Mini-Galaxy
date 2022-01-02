using UnityEngine;

public class BioRandomize : MonoBehaviour
{
    public int treeAmount;
    public int rockAmount;
    public int oreAmount;
    public bool enableGrass;
    public bool enableRock;
    private Object[] animals;
    private Object[] oreStones;
    private Object[] trees;
    private Object[] grass;
    private Object[] rocks;
    private Planet script;
    private TerrainFace[,] faces;
    private Transform plantFolder;
    private Transform orbFolder;
    private Transform animalFolder;

    private void Start()
    {
        script = gameObject.GetComponent<Planet>();
        faces = script.getTerrainFaces();

        // Initiate all plants on surfaces
        plantFolder = transform.Find("Plant");
        orbFolder = transform.Find("Orb");
        animalFolder = transform.Find("Animal");
        trees = new Object[5];
        grass = new Object[1];
        animals = new Object[2];
        rocks = new Object[2];
        oreStones = new Object[5];

        for (int i = 0; i < trees.Length; i++)
        {
            trees[i] = Resources.Load("Plants/_Prefabs/Tree" + i);
        }
        for (int i = 0; i < rocks.Length; i++)
        {
            rocks[i] = Resources.Load("Plants/_Prefabs/Rock" + i);
        }

        grass[0] = Resources.Load("Plants/_Prefabs/Grass");

        for (int i = 0; i < animals.Length; i++)
        {
            animals[i] = Resources.Load("Animal" + i);
        }
        for (int i = 0; i < oreStones.Length; i++)
        {
            oreStones[i] = Resources.Load("MineOres/Prefabs/MetalOre" + i);
        }
    }

    public void EnableSector(int face, int sector)
    {
        if (plantFolder.Find(face + " " + sector) == null)
        {
            GameObject faceFolder = new GameObject(face + " " + sector);
            faceFolder.transform.SetParent(plantFolder);
            faceFolder.transform.localPosition = Vector3.zero;
            faceFolder.transform.localRotation = Quaternion.identity;
            faceFolder.transform.localScale = Vector3.one;
            GenerateType(faceFolder.transform, face, sector, trees, treeAmount, new Vector2(1, 3), new Vector2(1, 4), 4);
            if (enableGrass)
                GenerateType(faceFolder.transform, face, sector, grass, 1000, new Vector2(0, 0), new Vector2(1, 1), 0.5f);

            faceFolder = new GameObject(face + " " + sector);
            faceFolder.transform.SetParent(animalFolder);
            faceFolder.transform.localPosition = Vector3.zero;
            faceFolder.transform.localRotation = Quaternion.identity;
            faceFolder.transform.localScale = Vector3.one;
            GenerateType(faceFolder.transform, face, sector, animals, 5, new Vector2(0, 0), new Vector2(1, 1), 0);

            faceFolder = new GameObject(face + " " + sector);
            faceFolder.transform.SetParent(orbFolder);
            faceFolder.transform.localPosition = Vector3.zero;
            faceFolder.transform.localRotation = Quaternion.identity;
            faceFolder.transform.localScale = Vector3.one;
            GenerateType(faceFolder.transform, face, sector, oreStones, oreAmount, new Vector2(0, 0), new Vector2(1, 3), 1.5f);
            if (enableRock)
                GenerateType(faceFolder.transform, face, sector, rocks, rockAmount, new Vector2(0, 0), new Vector2(1, 1), 0);
        } else
        {
            SetSectorState(true, face, sector);
        }
    }

    public void SetSectorState(bool state, int face, int sector)
    {
        Transform folder = plantFolder.Find(face + " " + sector);
        if (folder != null)
        {
            folder.gameObject.SetActive(state);
        }
        folder = animalFolder.Find(face + " " + sector);
        if (folder != null)
        {
            folder.gameObject.SetActive(state);
        }
        folder = orbFolder.Find(face + " " + sector);
        if (folder != null)
        {
            folder.gameObject.SetActive(state);
        }
    }

    // Build any type of creatures based on needs
    public void GenerateType(Transform parentFolder, int face, int sector, Object[] prefab, int amount, Vector2 resize, Vector2 groupSize, float randomRange)
    {
        // First check if current sector is already generated, if so just enable
        for (int i = 0; i < amount; i++)
        {
            int vertAmount = faces[face, sector].resolution * faces[face, sector].resolution - 1;
            // TODO: avoid duplicated selected vertices
            Vector3 position = faces[face, sector].mesh.vertices[Random.Range(0, vertAmount)];
            // Avoid choosing vertices below lakes or sea level
            while (Vector3.Distance(position, Vector3.zero) <= (script.shapeSetting.planetRadius + 1))
            {
                position = faces[face, sector].mesh.vertices[Random.Range(0, vertAmount)];
            }
            float maximum = Random.Range(groupSize.x, groupSize.y);
            for (int j = 0; j < maximum; j++)
            {
                Transform creature = (Instantiate(prefab[Random.Range(0, prefab.Length)]) as GameObject).transform;
                creature.SetParent(parentFolder);
                creature.localPosition = position;
                if (resize.x > 0)
                {
                    creature.localScale = Vector3.one * Random.Range(resize.x, resize.y);
                }
                creature.localRotation = Quaternion.FromToRotation(creature.up, creature.position - transform.position) * Quaternion.AngleAxis(Random.Range(0, 360), creature.up);
                creature.localPosition += Random.Range(-randomRange, randomRange) * creature.forward + Random.Range(-randomRange, randomRange) * creature.right;
            }
        }
    }
}
