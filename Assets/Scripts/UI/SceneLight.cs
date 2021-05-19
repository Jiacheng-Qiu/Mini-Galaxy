using UnityEngine;

public class SceneLight : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.rotation *= Quaternion.AngleAxis(0.2f, new Vector3(1, 0, 0));
    }
}
