using UnityEngine;

public class EnvironmentComponent : MonoBehaviour
{
    public float durability = 100; // Health
    public new string name = ""; // The name of this object, used to help creatures distinguish food
    public string generalType;

    public string productName;
    public Object product;
    public Vector2 productAmount = new Vector2(2, 4);

    void Start()
    {// TODO: read data from Json
        product = null;
    }

    void FixedUpdate()
    {
        if (durability <= 0)
        {
            Explode();
        }
    }

    // Used only on non kinematic gameobjects (trees, rocks)
    public bool Hit(float amount)
    {
        durability -= amount;
        return durability <= 0;
    }

    // While explode, destroy self, and generate random amount of product with upward force
    public void Explode()
    {
        int max = (int) Random.Range(productAmount.x, productAmount.y);
        for (int i = 0; i < max; i++)
        {
            GameObject gen = (GameObject)Instantiate(product, transform.position + transform.up * 2, Quaternion.identity);
            if (gen.GetComponent<Rigidbody>() == null)
            {
                gen.AddComponent<Rigidbody>().useGravity = false;
            }
            // Long as all env comps are within subfolders of the planet, this will work
            gen.AddComponent<TinyObjGravity>().enabled = false;
            TinyObjGravity grav = gen.GetComponent<TinyObjGravity>();
            grav.planet = transform.parent.parent.parent;
            grav.enabled = true;

            MaterialProperty prop = gen.AddComponent<MaterialProperty>();
            prop.remainInteract = 1;
            prop.materialName = productName;
            prop.minProduct = 2;
            prop.maxProduct = 4;
            gen.tag = "Material";
            gen.transform.parent = transform.parent.parent.Find("Material");

            
            // Remove the material if not picked up after an amount of time for optimization
            Destroy(gen, 60f);
        }
        Destroy(gameObject);
    }
}
