using UnityEngine;

public class MineHealth : MonoBehaviour
{
    public float durability = 100;
    public string orbName = "Iron";
    private Object ore;
    void Start()
    {
        ore = Resources.Load("MineOres/Prefabs/" + orbName);
    }

    void FixedUpdate()
    {
        if (durability <= 0)
        {
            explode();
        }
    }

    public void Hit(float amount)
    {
        Debug.Log("Stone hit");
        durability -= amount;
    }

    // While explode, destroy self, and generate random amount of ore with upward force
    // TODO: add more edibility to this
    public void explode()
    {
        int max = Random.Range(2, 4);
        for (int i = 0; i < max; i++)
        {
            GameObject gen = (GameObject)Instantiate(ore, transform.position + transform.up * 5, Quaternion.identity);
            gen.GetComponent<TinyObjGravity>().Init(transform.parent.parent);
            MaterialProperty prop = gen.AddComponent<MaterialProperty>();
            prop.remainInteract = 1;
            prop.materialName = orbName;
            prop.minProduct = 1;
            prop.maxProduct = 2;
        }
        Destroy(gameObject);
    }
}
