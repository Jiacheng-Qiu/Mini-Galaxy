using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Furnace : MonoBehaviour
{

    private string[] metalOres;
    private List<MetalQueue> queue;
    private float buildTime;
    private float lastBuild;

    private class MetalQueue
    {
        public string metalName;
        public int amount;
        public MetalQueue(string metalName, int amount)
        {
            this.metalName = metalName;
            this.amount = amount;
        }
    }
    void Start()
    {
        queue = new List<MetalQueue>();
        buildTime = 1;
        // TODO these should be read from the JSON files
        metalOres = new string[4];
        metalOres[0] = "Copper";
        metalOres[1] = "Iron";
        metalOres[2] = "Gold";
        metalOres[3] = "Aluminum";
    }

    // Used for constantly building ingots
    void FixedUpdate()
    {
        // Check if there is something to make, and build CD past
        if (queue.Count > 0 && Time.time > lastBuild + buildTime)
        {
            CreateObj();
            lastBuild = Time.time;
            if (queue[0].amount <= 0) 
            {
                queue.RemoveAt(0);
            }
        }
    }

    // Detects if there is material entering, if so, create ingots with material
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Material")
        {
            // Detect if its metal
            MaterialProperty prop = other.GetComponent<MaterialProperty>();
            string name = prop.materialName;
            int amount = prop.minProduct; // Min and max amount should be the same
            if (metalOres.Contains<string>(name))
            {
                queue.Add(new MetalQueue(name, amount));
                Destroy(other.gameObject);
                return;
            }
            // for all other situations, throw the material back out
            other.transform.position = transform.position + transform.up * 2;
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 800);
        }
        
    }

    private void CreateObj()
    {
        // Check the name of product to create
        string name = queue[0].metalName;
        // Reduce the required amount by 1
        queue[0].amount--;

        GameObject gen = (GameObject)Instantiate(Resources.Load("Prefabs/" + name + "Ingot"), transform.position + transform.up * 2, Quaternion.identity);
        gen.SetActive(false);
        if (gen.GetComponent<Rigidbody>() == null)
        {
            gen.AddComponent<Rigidbody>().useGravity = false;
        }
        gen.AddComponent<TinyObjGravity>().Init(transform.parent);
        MaterialProperty prop = gen.AddComponent<MaterialProperty>();
        prop.remainInteract = 1;
        prop.materialName = name + "Ingot";
        prop.minProduct = 1;
        prop.maxProduct = 1;
        gen.tag = "Material";
        gen.transform.parent = transform.parent.Find("Material");
        gen.SetActive(true);
    }
}
