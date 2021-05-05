using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject beacon;
    public Weapon weapon;
    public bool disabled = false;
    void Update()
    {
        if (disabled)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            weapon.Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            weapon.Reload();
        }
        if (Input.GetMouseButtonDown(2))
        {
            GameObject beac = Instantiate(beacon, transform.position, transform.rotation) as GameObject;
            beac.GetComponent<Beacon>().player = gameObject;
            beac.GetComponent<Beacon>().SetColor(Color.red);
        }
    }
}
