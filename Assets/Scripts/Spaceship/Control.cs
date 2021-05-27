using UnityEngine;

public class Control : MonoBehaviour
{
    public float speed = 10f;
    public float flySpeed = 15f;
    private bool fly = false;
    private bool inSpace = false;

    public Rigidbody rb;
    public GameObject planet;
    private Vector3 gDirection;

    // float for recorded destination to create smooth rotation
    float targetY;
    float targetX;

    float currentX;
    float currentY;

    private ShipConsole console;

    void Start()
    {
        console = gameObject.GetComponent<ShipConsole>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Planet")
        {
            inSpace = false;
            planet = collision.transform.gameObject;
            this.gameObject.transform.parent = planet.transform.Find("Animal").transform;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.tag == "Planet")
        {
            inSpace = true;
            this.gameObject.transform.parent = null;
        }
    }

    // Whenever the spaceship leave the planet atmosphere, enter space mode
    void FixedUpdate()
    {
        // Apply rotation for whenever its on planet gravity space
        /*Physics.Raycast(transform.position, -transform.up, out hit, 10);
        gDirection = hit.normal;*/
        gDirection = transform.position - planet.transform.position;
        if (!fly)
            Gravitize();
    }

    void Gravitize()
    {
        Quaternion onPlanetRotate = Quaternion.FromToRotation(transform.up, gDirection) * transform.rotation;
        transform.rotation = onPlanetRotate;
        // Apply gravity
        Vector3 grav = (transform.position - planet.transform.position).normalized;
        rb.AddForce(grav * -9.8f);
    }

    public void SpaceshipMove()
    {
        // Space switch fly/land state when on planet attraction
        if (!inSpace && Input.GetKeyDown(KeyCode.LeftShift))
        {
            fly = !fly;
        }
        if (fly)
        {
            // Rotation when fly is active
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0, 0, -0.5f);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0, 0, 0.5f);
            }
        }
        // Flying state has a init flying speed which wont be adjusted
        float z = (Input.GetAxisRaw("Vertical") * speed + (fly ? flySpeed : 0)) * Time.deltaTime;
        // Creates a smooth ship direction control with mouse
        targetY += Input.GetAxis("Mouse X");
        targetX -= Input.GetAxis("Mouse Y");
        float adjustX = Mathf.Clamp((targetX - currentX) * 0.1f, -0.2f, 0.2f);
        float adjustY = Mathf.Clamp((targetY - currentY) * 0.1f, -0.2f, 0.2f);
        currentX += adjustX;
        currentY += adjustY;
        console.MoveAimer(new Vector2((targetY - currentY), -(targetX - currentX)));
        transform.Rotate(adjustX, adjustY, 0);
        transform.Translate(0, 0, z);
    }

    public bool isFly()
    {
        return fly;
    }
}
