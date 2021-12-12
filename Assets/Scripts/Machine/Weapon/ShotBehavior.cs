using UnityEngine;
using System.Collections;
using VolumetricLines;

public class ShotBehavior : MonoBehaviour
{
	public float speed;
	public bool isBullet;
	void FixedUpdate()
	{
		if (isBullet)
			transform.position += transform.forward * Time.deltaTime * speed;
	}

	public void StartExtend(float length)
    {
		if (!isBullet)
			StartCoroutine(LaserStretch(length, gameObject.GetComponent<VolumetricLineBehavior>()));
    }

	public void EndExtend(float length)
    {
		if (!isBullet)
			StartCoroutine(LaserStretch(-length, gameObject.GetComponent<VolumetricLineBehavior>()));
    }

	private IEnumerator LaserStretch(float length, VolumetricLineBehavior laser)
    {
		float elapsed = 0;
		while(elapsed <= 0.2f)
        {
			if (length < 0)
            {
				laser.StartPos = new Vector3(0, 0, -elapsed / 0.2f * length);
            } else
            {
				laser.EndPos = new Vector3(0, 0, elapsed / 0.2f * length);
			}
			elapsed += Time.deltaTime;
			yield return null;
        }
		if (length < 0)
        {
			laser.StartPos = Vector3.zero;
			laser.EndPos = Vector3.zero;
			gameObject.SetActive(false);
        }
    }
}
