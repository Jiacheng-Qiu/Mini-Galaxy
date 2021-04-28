using System;
using UnityEngine;
using UnityEngine.UI;

public class ShipConsole : MonoBehaviour
{
    public Canvas console;
    private Transform aimer;
    private float border;
    void Start()
    {
        console = GameObject.Find("ShipConsole").GetComponent<Canvas>();
        aimer = console.transform.Find("Aimer");
        border = console.transform.Find("Console").GetComponent<RectTransform>().rect.width / 2;

    }

    void Update()
    {
        if (console.enabled && Input.GetKeyDown(KeyCode.Escape))
        {
            console.enabled = false;
        }
    }

    public void moveAimer(Vector2 position)
    {
        
        console.enabled = true;
        position.x = Mathf.Clamp(position.x, -2 * border, 2 * border);
        position.y = Mathf.Clamp(position.y, -2 * border, 2 * border);
        aimer.localPosition = position;
    }
}
