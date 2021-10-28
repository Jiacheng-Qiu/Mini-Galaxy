using UnityEngine;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

public class PlayerMovement : MonoBehaviour
{
    // GUI telling player this can interact
    public GameObject informer;
    // Mouse sensitivity
    public float sensX;
    public float sensY;
    // Cam view init
    private float viewX;
    // POV wont move if on menu focus
    public bool onFocus;
    public GameObject onShip;
    public int interactRange;

    public float speed;
    public float runSpeed;
    public float jump;
    
    public bool isGround = true;

    private PlayerInventory inventory;
    private Backpack backpack;
    public Camera cam;
    private InteractionAnimation uiAnimation;
    public Rigidbody rb;

    //private RaycastHit hit;
    public GameObject planet;
    private Vector3 gDirection;
    private Collider aimObject;

    public GameObject flashlight;
    private bool flashSwitch;

    private bool onPreview = false; // Assign preview when placing items
    private GameObject previewObject;

    public InterfaceAnimManager mapUI;
    private bool mapOn = false;

    void Start()
    {
        /*// Read settings from json file
        Settings set = JsonUtility.FromJson<Settings>(settings.text);
        sensX = set.VerticalSensitivity;
        sensY = set.HorizontalSensitivity;*/
        uiAnimation = gameObject.GetComponent<InteractionAnimation>();
        informer.SetActive(true);
        uiAnimation.DisplayInformer(false);
        // TODO: code dependency
        // GameObject.Find("ShipConsole").GetComponent<Canvas>().enabled = false;
        ChangeCursorFocus(false);
        //hit = new RaycastHit();
        transform.parent = planet.transform;
        inventory = gameObject.GetComponent<PlayerInventory>();
        backpack = gameObject.GetComponent<Backpack>();
    }

    // If there are multiple interface active, only inactive when all screens are shut down
    public void ChangeCursorFocus(bool state)
    {
        if (state)
        {
            onFocus = true;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        } 
        else
        {
            onFocus = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }


    void Update()
    {
        if (PlayerStatus.moveDisabled)
            return;
        // Place object on ground in placing
        // When player tab left mouse button to place, exit preview state
        if (onPreview && Input.GetMouseButton(0))
        {
            onPreview = false;
            previewObject.GetComponent<MeshRenderer>().material = Resources.Load("Materials/" + previewObject.GetComponent<Machine>().machineName + "Material", typeof(Material)) as Material;
            previewObject.GetComponent<Machine>().enabled = true;
            previewObject.transform.parent = transform.parent;
            previewObject = null;
        }

        // Get input for flashlight
        if (Input.GetKeyUp(KeyCode.L))
        {
            flashlight.SetActive(flashSwitch);
            flashSwitch = !flashSwitch;
        }

        // Open or close helmet
        if (Input.GetKeyUp(KeyCode.N))
        {
            gameObject.GetComponent<InteractionAnimation>().HelmetSwitch();
        }

        // TODO: Map showcased
        if (Input.GetKeyUp(KeyCode.M))
        {
            if (mapOn)
            {
                mapUI.startDisappear();
                mapOn = false;
            }
            else
            {
                mapUI.gameObject.SetActive(true);
                mapUI.startAppear();
                mapOn = true;
            }
        }

        // Tell attack script if shooting disabled
        PlayerStatus.attackDisabled = onFocus;

        // Ask planet to assign mesh collider, only happen when there is one planet
        gDirection = transform.position - planet.transform.position;
        if (planet != null && planet.GetComponent<Planet>() != null)
        {
            planet.GetComponent<Planet>().GenerateCollider((transform.position - planet.transform.position).normalized);
        }
        // Can't do anything if is on Interfaces
        if (!onFocus)
        {
            Interact();
            Move();
            if (!onShip)
            {
                Jump();
                if (Input.GetKeyDown(KeyCode.G))
                {
                    Throw(-1);
                }

                if (aimObject == null && Input.GetKeyDown(KeyCode.E))
                {
                    Place(-1);
                }
            }
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

                informer.transform.Find("Item name").GetComponent<Text>().text = hit.collider.name.Replace("(Clone)", "");
                uiAnimation.DisplayInformer(true);
                return;
            }
        }

        // If above didn't match, then there is no aimObj
        aimObject = null;
        uiAnimation.DisplayInformer(false);
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
            previewObject.transform.LookAt(transform);
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
                    backpack.PutIn(materialName, amount);
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
                    backpack.PutIn(aimObject.GetComponent<Machine>().machineName, 1);
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
                if (gameObject.GetComponent<PlayerHealthSystem>().Run(true))
                {
                    uiAnimation.WalkCamEffect(1.2f);
                    x *= runSpeed / speed;
                    z *= runSpeed / speed;
                }
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                gameObject.GetComponent<PlayerHealthSystem>().Run(false);
            }
            
            if (x != 0 || z != 0)
            {
                uiAnimation.WalkCamEffect(1);
                transform.Translate(x, 0, z);
            }
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
            uiAnimation.DisplayInformer(false);
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

    // Throw material out from inv bar
    // If pos=-1, throw from invbar, if pos>=0, throw from backpack
    public void Throw(int pos)
    {
        // Check material tag before actual throwing
        /*string tag = inventory.CheckTag();
        if (tag != "Material")
        {
            return;
        }*/
        GameObject obj;
        if (pos == -1)
            obj = inventory.GetOut();
        else
            obj = backpack.GetOut(pos, false);
        obj.transform.position = cam.transform.position;
        obj.transform.parent = transform.parent.Find("Material");
        obj.AddComponent<TinyObjGravity>().Init(transform.parent);
        obj.SetActive(true);
        // Throw in the front direction after activate
        obj.GetComponent<Rigidbody>().AddForce(transform.Find("Main Camera").forward * 500);
    }

    // Put placeable items onto ground only when no interactives on aiming
    public void Place(int bagPos)
    {
        string tag = bagPos == -1 ? inventory.CheckTag() : backpack.CheckTag(bagPos, false);
        // First check the tag of the output, if it is Interactable or Spaceship, then direct to preview scene
        if (tag != "Interactable" && tag != "Spaceship")
        {
            return;
        }
        GameObject obj;
        if (bagPos == -1)
        {
            obj = inventory.GetOut();
        } else
        {
            obj = backpack.GetOut(bagPos, false);
        }
        // Assign preview material, and disable scripts
        obj.GetComponent<Machine>().player = gameObject;
        Debug.Log(Resources.Load("Materials/PreviewMaterial.mat"));
        obj.GetComponent<MeshRenderer>().material = Resources.Load("Materials/PreviewMaterial", typeof(Material)) as Material;
        obj.GetComponent<Machine>().enabled = false;
        onPreview = true;
        previewObject = obj;
        previewObject.SetActive(true);
        
    }
}
