using UnityEngine;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

public class PlayerMovement : UIInformer
{
    // GUI telling player this can interact
    public GameObject informer;
    private GameSettings settings;

    // Mouse sensitivity
    private float sensX;
    private float sensY;
    // Cam view init
    public float viewX;
    private float viewY;
    // POV wont move if on menu focus
    public bool onFocus;
    public GameObject onShip;
    public int interactRange;

    public float speed;
    public float runSpeed;
    public float jump;

    public bool isGround = true;

    public PlayerInventory inventory;
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

    private bool onPreview; // Assign preview when placing items
    private bool onDismantle;
    private float previewRotate;
    private GameObject dismantleObject;
    private GameObject previewObject;

    private float dismantleStartTime; // Record when m1 held

    private Vector3 marker; // Player marker for marking positions on planet

    void Awake()
    {
        uiAnimation = gameObject.GetComponent<InteractionAnimation>();
        informer.SetActive(true);
        uiAnimation.DisplayInformer(false);
        // GameObject.Find("ShipConsole").GetComponent<Canvas>().enabled = false;
        ChangeCursorFocus(false);
        //hit = new RaycastHit();
        transform.parent = planet.transform;
        backpack = gameObject.GetComponent<Backpack>();

        onPreview = false;
        onDismantle = false;
        dismantleStartTime = -1;

        flashlight.SetActive(false);
        flashSwitch = false;
        marker = Vector3.zero;

        sensX = 1;
        sensY = 1;
    }

    /*private void Start()
    {
        settings = GameObject.Find("DataTransfer").GetComponent<GameSettings>();
        ChangeSettings();
    }*/

    public void LoadStatus(SaveFormat data)
    {
        this.speed = data.speed;
        this.runSpeed = data.runSpeed;
        this.jump = data.jump;
        transform.position = new Vector3(data.posX, data.posY, data.posZ);
        transform.eulerAngles = new Vector3(data.rotationX, data.rotationY, data.rotationZ);
        this.viewX = data.camViewX;
    }

    public void ChangeSettings()
    {
        sensX = settings.sensx;
        sensY = settings.sensy;

        cam.GetComponent<Camera>().fieldOfView = settings.fov;
    }

    public Vector3 GetMarker()
    {
        return marker;
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
        // Tell attack script if shooting disabled
        PlayerStatus.attackDisabled = onFocus;
    }

    void Update()
    {
        if (PlayerStatus.moveDisabled)
            return;

        // Get input for flashlight
        if (Input.GetKeyUp(KeyCode.L))
        {
            flashSwitch = !flashSwitch;
            flashlight.SetActive(flashSwitch);
            InformGuide("L", false);
        }

        // REDO
        if (Input.GetKeyUp(KeyCode.LeftBracket))
        {
            Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, cam.transform.forward, out hit, 50, LayerMask.GetMask("Terrain")))
            {
                // Make marker local position on planet
                marker = hit.point - planet.transform.position;
            }
        }

        // Open or close helmet
        /*if (Input.GetKeyUp(KeyCode.N))
        {
            gameObject.GetComponent<InteractionAnimation>().HelmetSwitch();
        }*/
        // Can't do anything if is on Interfaces
        if (!onFocus)
        {
            Move();
            if (!onShip)
            {
                Jump();
                if (!onPreview && !onDismantle)
                {
                    Interact();
                    if (Input.GetKeyDown(KeyCode.G))
                    {
                        Throw(-1);
                    }
                }
                // Place Preview interface
                if (!onPreview)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                        Place(-1);
                } else
                {
                    float rotation = Input.GetAxis("Mouse ScrollWheel");
                    // Place object on ground in placing
                    // When player tab left mouse button to place, exit preview state
                    if (Input.GetMouseButtonUp(0))
                    {
                        previewObject.GetComponent<MeshRenderer>().material = Resources.Load("Materials/" + previewObject.GetComponent<Machine>().machineName + "Material", typeof(Material)) as Material;
                        previewObject.GetComponent<Machine>().enabled = true;
                        previewObject.GetComponent<Collider>().enabled = true;
                        previewObject.transform.parent = transform.parent;
                        previewObject = null;
                        previewRotate = 0;
                        onPreview = false;
                        InformGuide("E", false);
                        PlayerStatus.attackDisabled = false;
                    }
                    else if (Input.GetKeyDown(KeyCode.E))
                    {
                        StopPlace();
                    } 
                    else if(rotation != 0)
                    {
                        previewRotate += rotation * 5;
                    }
                }


                // Dismantle preview interface
                if (Input.GetKeyUp(KeyCode.T))
                {
                    if (onDismantle)
                    {
                        StopDismantle();
                    } else
                    {
                        Dismantle();
                    }
                }

                if (Input.GetMouseButtonDown(0) && onDismantle)
                {
                    dismantleStartTime = Time.time;
                    uiAnimation.StartLoad(1);
                } else if (Input.GetMouseButtonUp(0) && onDismantle)
                {
                    dismantleStartTime = -1;
                    uiAnimation.StopLoad();
                }
            }
        }
    }

    public void Dismantle()
    {
        if (onPreview)
        {
            StopPlace();
        }
        uiAnimation.StopAll();
        onDismantle = true;
        PlayerStatus.attackDisabled = true;
        InformGuide("T", true);
    }

    public void StopDismantle()
    {
        onDismantle = false;
        dismantleStartTime = -1;
        uiAnimation.StopLoad();
        if (dismantleObject != null)
        {
            dismantleObject.GetComponent<MeshRenderer>().material = Resources.Load("Materials/" + dismantleObject.GetComponent<Machine>().machineName + "Material", typeof(Material)) as Material;
        }
        dismantleObject = null;
        PlayerStatus.attackDisabled = false;
        InformGuide("T", false);
    }

    public void StopPlace()
    {
        if (previewObject != null)
        {
            backpack.PutIn(previewObject.GetComponent<Machine>().machineName, 1);
            Destroy(previewObject);
            previewObject = null;
            previewRotate = 0;
            onPreview = false;
            InformGuide("E", false);
            PlayerStatus.attackDisabled = false;
        }
    }

    private void FixedUpdate()
    {
        Gravitize();

        if (planet != null && planet.GetComponent<Planet>() != null)
        {
            
        }

        if (!onPreview && !onDismantle)
        {
            Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            // TODO: add interactable in the following
            if (Physics.Raycast(rayOrigin, cam.transform.forward, out hit, interactRange, (1 << LayerMask.NameToLayer("Material") | (1 << LayerMask.NameToLayer("Artificial")))))
            {
                if (!hit.collider.isTrigger)
                {
                    aimObject = hit.collider;
                    informer.transform.Find("Item name").GetComponent<Text>().text = hit.collider.name.Replace("(Clone)", "");
                    uiAnimation.DisplayInformer(true);
                }
            } else
            {
                uiAnimation.DisplayInformer(false);
                aimObject = null;
            }
        } else
        {
            uiAnimation.DisplayInformer(false);
        }

        if (onPreview)
            Preview();
        else if (onDismantle)
        {
            Vector3 dmOrig = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            RaycastHit dmHit;
            if (Physics.Raycast(dmOrig, cam.transform.forward, out dmHit, 50, LayerMask.GetMask("Artificial")))
            {
                if (dismantleObject != null)
                {
                    if (!GameObject.ReferenceEquals(dismantleObject, dmHit.transform.gameObject))
                    {
                        dismantleObject.GetComponent<MeshRenderer>().material = Resources.Load("Materials/" + dismantleObject.GetComponent<Machine>().machineName + "Material", typeof(Material)) as Material;
                        dismantleObject = dmHit.transform.gameObject;
                        dismantleObject.GetComponent<MeshRenderer>().material = Resources.Load("Materials/DismantleMaterial", typeof(Material)) as Material;
                        dismantleStartTime = -1;
                        uiAnimation.StopLoad();
                    }
                    else if (dismantleStartTime != -1 && Time.time > dismantleStartTime + 1)
                    {
                        backpack.PutIn(dismantleObject.GetComponent<Machine>().machineName, 1);
                        Destroy(dismantleObject);
                        dismantleObject = null;
                        dismantleStartTime = -1;
                        uiAnimation.StopLoad();
                    }
                } else
                {
                    dismantleObject = dmHit.transform.gameObject;
                    dismantleObject.GetComponent<MeshRenderer>().material = Resources.Load("Materials/DismantleMaterial", typeof(Material)) as Material;
                    dismantleStartTime = -1;
                    uiAnimation.StopLoad();
                }
            }
            else if (dismantleObject != null)
            {
                dismantleObject.GetComponent<MeshRenderer>().material = Resources.Load("Materials/" + dismantleObject.GetComponent<Machine>().machineName + "Material", typeof(Material)) as Material;
                dismantleObject = null;
                dismantleStartTime = -1;
                uiAnimation.StopLoad();
            }
        }
    }

    // Give preview of placing placable items
    private void Preview()
    {
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
            previewObject.transform.eulerAngles = transform.eulerAngles;
            previewObject.transform.RotateAroundLocal(transform.up, previewRotate);
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
            gameObject.GetComponent<Map>().UpdatePlanetInfo(planet.GetComponent<Planet>());
            gameObject.GetComponent<Missions>().ModifyPlanetRadius(planet.GetComponent<Planet>().shapeSetting.planetRadius);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.tag == "Planet")
        {
            Debug.Log("Exiting planet");
            // Also clear marker
            marker = Vector3.zero;
        }
    }

    void Interact()
    {
        // Can't jump off ship when in space mode
        if (Input.GetKeyDown(KeyCode.Escape) && onShip && !onShip.GetComponent<Control>().isFly())
        {
            transform.parent = planet.transform;
            onShip = null;
        }
        else if (!onShip && Input.GetKeyDown(KeyCode.F))
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
                    // Open corresponding UI, TODO add more if necessary
                    switch (aimObject.GetComponent<Machine>().machineName)
                    {
                        case "Crafttable":
                            uiAnimation.DisplayCraft();
                            break;
                    }
                    break;
            }
        }
    }

    // Make player rotate to gravity
    void Gravitize()
    {
        // Ask planet to assign mesh collider, only happen when there is one planet
        gDirection = transform.position - planet.transform.position;
        // Apply gravity
        Vector3 grav = (transform.position - planet.transform.position).normalized;
        rb.AddForce(grav * -9.8f);
        // Player only rotate on Y axis, which won't affect quaternion rotation for planet
        transform.Rotate(0, viewY, 0);
        viewY = 0;
        Quaternion onPlanetRotate = Quaternion.FromToRotation(transform.up, gDirection) * transform.rotation;
        // Rotate player based on gravity
        transform.rotation = onPlanetRotate;
        // Also need to rotate cam based on player input + gravity
        Quaternion camRotate = Quaternion.AngleAxis(viewX, cam.transform.right);
        cam.transform.rotation = camRotate * onPlanetRotate;
    }

    // Move POV
    void Move()
    {
        // move when not on ship
        if (onShip == null)
        {
            float x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
            float z = Input.GetAxisRaw("Vertical") * Time.deltaTime * speed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                gameObject.GetComponent<PlayerHealthSystem>().Run(true);
                uiAnimation.WalkCamEffect(1.2f);
                x *= runSpeed / speed;
                z *= runSpeed / speed;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                gameObject.GetComponent<PlayerHealthSystem>().Run(false);
            }
            
            if (x != 0 || z != 0)
            {
                uiAnimation.WalkCamEffect(1);
                transform.Translate(x, 0, z);
                gameObject.GetComponent<PlayerHealthSystem>().Walk(true);
            }else
            {
                gameObject.GetComponent<PlayerHealthSystem>().Walk(false);
            }
            if (!onFocus)
            {
                // rotation
                viewY += sensY * Input.GetAxis("Mouse X");
                viewX -= sensX * Input.GetAxis("Mouse Y");
                // Set up a limit for viewX, so that player can't look at his back with huge flip
                viewX = Mathf.Clamp(viewX, -55, 55);
            }
        }
        else
        {
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
        StopDismantle();
        string tag = (bagPos == -1) ? inventory.CheckTag() : backpack.CheckTag(bagPos, false);
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
        obj.GetComponent<MeshRenderer>().material = Resources.Load("Materials/PreviewMaterial", typeof(Material)) as Material;
        obj.GetComponent<Machine>().enabled = false;
        obj.GetComponent<Collider>().enabled = false;
        onPreview = true;
        previewObject = obj;
        previewObject.SetActive(true);
        InformGuide("E", true);
        PlayerStatus.attackDisabled = true;
    }
}