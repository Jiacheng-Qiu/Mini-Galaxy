using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    public GameObject laser;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject go = GameObject.Instantiate(laser, transform.position, transform.Find("Main Camera").rotation) as GameObject;
            Destroy(go, 3f);
        }
    }
}
