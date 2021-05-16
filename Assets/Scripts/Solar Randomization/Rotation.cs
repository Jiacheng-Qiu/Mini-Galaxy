using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float rotateSpeed;
    private int rotateDir;
    public bool selfStart = false;

    public void Start()
    {
        // If no parent calls (on menu), init self
        if (selfStart)
            Init();
    }

    public void Init()
    {
        rotateDir = (Random.value > 0.5) ? 1 : -1;
        // Around 1 min 1 round
        rotateSpeed = 6f;
    }

    void FixedUpdate()
    {
        // TODO: understand how to adjust rotation to all angles
        this.gameObject.transform.Rotate(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
    }
}
