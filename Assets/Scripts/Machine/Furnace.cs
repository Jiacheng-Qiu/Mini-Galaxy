using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // Detects if there is material entering, if so, create ingots with material
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Material")
        {
            // Detect if its metal
            string name = other.GetComponent<MaterialProperty>().materialName;
            
            switch (name)
            {
                case "Copper":
                    CreateObj(name);Destroy(other.gameObject);
                    break;
                case "Iron":
                    CreateObj(name); Destroy(other.gameObject);
                    break;
                case "Gold":
                    CreateObj(name); Destroy(other.gameObject);
                    break;
                case "Aluminum":
                    CreateObj(name); Destroy(other.gameObject);
                    break;
                default:

                    break;
            }
        }
    }

    private void CreateObj(string name)
    {
        GameObject gen = (GameObject)Instantiate(Resources.Load("Prefabs/" + name + "Ingot"), transform.position, Quaternion.identity);
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
        gen.GetComponent<Rigidbody>().AddForce(-transform.up * 300);
    }
}
