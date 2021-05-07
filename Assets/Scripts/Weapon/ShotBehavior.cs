using UnityEngine;

public class ShotBehavior : MonoBehaviour
{
	public float damage = 30;
	public GameObject player;
	public float speed = 150;
	void Update()
	{
		transform.position += transform.forward * Time.deltaTime * speed;
	}

	// On collision, damage hit object and delete self.
	private void OnCollisionEnter(Collision collision)
	{
		GameObject other = collision.gameObject;
		if (other.tag == "Animal")
		{
			other.GetComponent<HealthSystem>().Hurt(player, damage);
			Destroy(gameObject);
		}
		else if (other.tag == "Environment")
		{
			other.GetComponent<EnvironmentComponent>().Hit(damage);
			Destroy(gameObject);
		}
	}
}
