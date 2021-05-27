using UnityEngine;

// TODO: This class is a temporary replacement for the original reaction on planets, it is only used to deal with case that player can fall into ground
public class TempReaction : MonoBehaviour
{
    private void Start()
    {
        gameObject.AddComponent<SphereCollider>().radius = gameObject.GetComponent<Planet>().shapeSetting.planetRadius - 2;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // push player upwards
            collision.gameObject.transform.position += collision.gameObject.transform.up * 30;
        }
        else if(collision.gameObject.tag == "Interactable" || collision.gameObject.tag == "Spaceship")
        {
            collision.gameObject.transform.position += collision.gameObject.transform.up * 20;
        }
        else
        {
            // Destroy all objects falling into the ground
            Destroy(collision.gameObject);
        }
    }
}
