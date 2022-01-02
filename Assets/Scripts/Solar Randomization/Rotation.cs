using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float rotateSpeed;
    private int rotateDir;

    public void Start()
    {
        if (rotateSpeed == 0)
            rotateSpeed = 0.3f;
        rotateDir = 1;
    }

    void FixedUpdate()
    {
        transform.Rotate(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
    }
}
