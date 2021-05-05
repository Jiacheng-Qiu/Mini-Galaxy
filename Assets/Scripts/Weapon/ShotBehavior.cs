using UnityEngine;

public class ShotBehavior : MonoBehaviour {
	public float damage = 30;

	void Update()
	{
		transform.position += transform.forward * Time.deltaTime * 1000f;
	}

	// On collision, damage hit object and delete self.
	private void OnCollisionEnter(Collision collision)
	{
		GameObject other = collision.gameObject;
		if (other.tag == "Animal")
		{
			other.GetComponent<HealthSystem>().Hurt(null, damage);
			Destroy(gameObject);
		} else if (other.tag == "Stone") {
			other.GetComponent<MineHealth>().Hit(damage);
			Destroy(gameObject);
		}
	}
}
