using UnityEngine;

// Gravity applied for animals
public class AnimalGravity : MonoBehaviour
{
    private Rigidbody rb;
    private RaycastHit hit;
    public GameObject planet;

    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Planet")
        {
            planet = collision.transform.gameObject;
            this.gameObject.transform.parent = planet.transform.Find("Animal").transform;
            /*Physics.Raycast(transform.position, -transform.up, out hit, 10);
            gDirection = hit.normal;*/
        }
    }

    // Update is called once per frame
    void Update()
    {
        Gravitize();
    }

    void Gravitize()
    {
        Vector3 grav = (transform.position - planet.transform.position).normalized;
        rb.AddForce(grav * -9.8f);
        Quaternion onPlanetRotate = Quaternion.FromToRotation(transform.up, grav) * transform.rotation;
        transform.rotation = onPlanetRotate;
    }
}
