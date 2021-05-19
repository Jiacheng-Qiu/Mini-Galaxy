using UnityEngine;

public class EquipmentCustomize : MonoBehaviour
{
    public GameObject player;
    private PlayerInventory inventory;
    private PlayerMovement ctrl;
    private Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        canvas = gameObject.GetComponent<Canvas>();
        canvas.enabled = false;
        inventory = player.GetComponent<PlayerInventory>();
        ctrl = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            // Set on focus
            ctrl.onFocus = true;
            canvas.enabled = true;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ctrl.onFocus = false;
            canvas.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
