using UnityEngine;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

public class PlayerMovement : MonoBehaviour
{
    // GUI telling player this can interact
    public GameObject informer;
    // Mouse sensitivity
    public float sensX = 2;
    public float sensY = 2;
    // Cam view init
    private float viewX = 0f;
    // POV wont move if on menu focus
    public bool onFocus = false;
    public GameObject onShip;
    public int interactRange = 5;

    public float speed = 10f;
    public float runSpeed = 20f;
    public float jump = 10f;
    
    public bool isGround = true;

    private PlayerInventory inventory;
    public Camera cam;
    
    public Rigidbody rb;

    //private RaycastHit hit;
    public GameObject planet;
    private Vector3 gDirection;
    private Collider aimObject;

    public GameObject flashlight;
    private bool flashSwitch;

    private bool onPreview = false; // Assign preview when placing items
    private GameObject previewObject;


    void Start()
    {
        /*// Read settings from json file
        Settings set = JsonUtility.FromJson<Settings>(settings.text);
        sensX = set.VerticalSensitivity;
        sensY = set.HorizontalSensitivity;*/

        // Hide informer on init
        informer.SetActive(false);

        // TODO: code dependency
        GameObject.Find("ShipConsole").GetComponent<Canvas>().enabled = false;
        onFocus = true;
        //hit = new RaycastHit();
        transform.parent = planet.transform;
        inventory = gameObject.GetComponent<PlayerInventory>();
    }

    // TODO this method is called after starting game, player need to first read info
    public void InitInfo()
    {
        onFocus = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        // Place object on ground in placing
        // When player tab left mouse button to place, exit preview state
        if (onPreview && Input.GetMouseButton(0))
        {
            Debug.Log("Placing;");
            onPreview = false;
            previewObject.transform.parent = transform.parent;
            previewObject = null;
        }

        // Get input for flashlight
        if (Input.GetKeyUp(KeyCode.L))
        {
            flashlight.SetActive(flashSwitch);
            flashSwitch = !flashSwitch;
        }

        // Tell attack script if shooting disabled
        gameObject.GetComponent<PlayerAttack>().disabled = onFocus;

        // Ask planet to assign mesh collider, only happen when there is one planet
        gDirection = transform.position - planet.transform.position;
        if (planet != null && planet.GetComponent<Planet>() != null)
        {
            planet.GetComponent<Planet>().GenerateCollider((transform.position - planet.transform.position).normalized);
        }
        Interact();
        Move();
        if (!onShip)
        {
            Jump();
            Throw();
            Place();
        }
    }

    private void FixedUpdate()
    {
        Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // TODO: add interactable in the following
        if (Physics.Raycast(rayOrigin, cam.transform.forward, out hit, interactRange, LayerMask.GetMask("Material")))
        {
            if (!hit.collider.isTrigger && hit.collider.tag == "Material" || hit.collider.tag == "Interactable" || hit.collider.tag == "Spaceship")
            {
                aimObject = hit.collider;
                // Also display the name on UI, delete clone text

                informer.transform.Find("Item").Find("ItemName").GetComponent<Text>().text = hit.collider.name.Replace("(Clone)", "");

                informer.SetActive(true);
                return;
            }
        }
        // If above didn't match, then there is no aimObj
        aimObject = null;
        informer.SetActive(false);


        if (onPreview)
            Preview();
    }

    // Give preview of placing placable items
    private void Preview()
    {
        // TODO: sometimes machine dissappear??
        if (previewObject == null)
        {
            onPreview = false;
            return;
        }
        Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, cam.transform.forward, out hit, 30, LayerMask.GetMask("Terrain")))
        { 
            previewObject.transform.position = hit.point;
        }
    }

    // Change planet if colliding with another planet trigger
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Planet")
        {
            planet = collision.transform.gameObject;
            // Add player as child of planet if not on ship (shouldn't happen in actual gameplay)
            if (onShip == null)
            {
                transform.parent = planet.transform;
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.tag == "Planet")
        {
            Debug.Log("Exiting planet");
        }
    }

    void Interact()
    {
        // Can't jump off ship when in space mode
        if (Input.GetKey(KeyCode.Escape) && onShip && !onShip.GetComponent<Control>().isFly())
        {
            transform.parent = planet.transform;
            onShip = null;
        }
        else if (!onShip && Input.GetKey(KeyCode.F))
        {
            // First check if the obj to interact with is destroyed already or not
            if (aimObject == null)
            {
                return;
            }
            switch (aimObject.tag)
            {
                case "Material":
                    MaterialProperty mat = aimObject.gameObject.GetComponent<MaterialProperty>();
                    var materialName = mat.getName();
                    int amount = mat.Interacted();
                    inventory.putIn(materialName, amount);
                    break;
                // TODO
                case "Spaceship":
                    onShip = aimObject.transform.gameObject;
                    transform.parent = onShip.transform;
                    // Reset camera
                    cam.transform.localRotation = new Quaternion();
                    gameObject.transform.position = onShip.transform.position;
                    // TODO: Temporary setup for posiution
                    transform.localPosition = new Vector3(0, -0.2f, 1.5f);
                    gameObject.transform.rotation = onShip.transform.rotation;
                    break;
                // All machines can be taken back into backpack
                case "Interactable":
                    inventory.putIn(aimObject.GetComponent<Machine>().machineName, 1);
                    Destroy(aimObject.gameObject);
                    break;
            }
        }

    }
    // Move POV and make player rotate to gravity
    void Move()
    {
        // move when not on ship
        if (onShip == null)
        {
            float x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
            float z = Input.GetAxisRaw("Vertical") * Time.deltaTime * speed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (gameObject.GetComponent<PlayerHealthSystem>().run(true))
                {
                    x *= runSpeed / speed;
                    z *= runSpeed / speed;
                }
            }
            else
            {
                gameObject.GetComponent<PlayerHealthSystem>().run(false);
            }
            
            transform.Translate(x, 0, z);
            // Apply gravity
            Vector3 grav = (transform.position - planet.transform.position).normalized;
            rb.AddForce(grav * -9.8f);
            Quaternion onPlanetRotate = Quaternion.FromToRotation(transform.up, gDirection) * transform.rotation;
            // Rotate player based on gravity
            transform.rotation = onPlanetRotate;
            if (!onFocus)
            {
                // rotation
                float playerX = sensY * Input.GetAxis("Mouse X");
                viewX -= sensX * Input.GetAxis("Mouse Y");
                // Set up a limit for viewX, so that player can't look at his back with huge flip
                viewX = Mathf.Clamp(viewX, -55, 55);
                // Player only rotate on Y axis, which won't affect quaternion rotation for planet
                transform.Rotate(0, playerX, 0);
                // Also need to rotate cam based on player input + gravity
                Quaternion camRotate = Quaternion.AngleAxis(viewX, cam.transform.right);
                cam.transform.rotation = camRotate * onPlanetRotate;
            }
        }
        else
        {
            informer.SetActive(false);
            onShip.GetComponent<Control>().SpaceshipMove();
            gameObject.transform.position = onShip.transform.position;
            // TODO: Temporary setup for posiution
            transform.localPosition = new Vector3(0, -0.2f, 1.5f);
        }
    }

    void Jump()
    {
        // Check if is on ground
        isGround = Physics.Raycast(transform.position, -transform.up, 1.3f, LayerMask.GetMask("Terrain"));
        if (isGround && Input.GetKeyDown(KeyCode.Space))
        {
            isGround = false;
            rb.AddForce(transform.up * jump);
        }
    }

    // Throw material out
    void Throw()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            // Check material tag before actual throwing
            string tag = inventory.CheckTag();
            if (tag != "Material")
            {
                return;
            }
            GameObject obj = inventory.getOut();
            obj.AddComponent<TinyObjGravity>().Init(transform.parent);
            obj.SetActive(true);
            // Throw in the front direction after activate
            obj.GetComponent<Rigidbody>().AddForce(transform.Find("Main Camera").forward * 500);
        }
    }

    // Put placeable items onto ground only when no interactives on aiming
    void Place()
    {
        if (aimObject == null && Input.GetKeyDown(KeyCode.F))
        {
            string tag = inventory.CheckTag();
            // First check the tag of the output, if it is Interactable or Spaceship, then direct to preview scene
            if (tag != "Interactable" && tag != "Spaceship")
            {
                return;
            }
            GameObject obj = inventory.getOut();
            if (tag == "Interactable")
            {
                obj.GetComponent<Machine>().player = gameObject;
            }
            onPreview = true;
            previewObject = obj;
            previewObject.SetActive(true);
        }
    }
}
