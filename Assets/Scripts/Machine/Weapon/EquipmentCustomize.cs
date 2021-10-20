using UnityEngine;

public class EquipmentCustomize : MonoBehaviour
{
    public GameObject player;
    private PlayerInventory inventory;
    private PlayerMovement movement;
    private Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        canvas = gameObject.GetComponent<Canvas>();
        canvas.enabled = false;
        inventory = player.GetComponent<PlayerInventory>();
        movement = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            // Set on focus
            movement.ChangeCursorFocus(true);
            canvas.enabled = true;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            movement.ChangeCursorFocus(false);
            canvas.enabled = false;
        }
    }
}
