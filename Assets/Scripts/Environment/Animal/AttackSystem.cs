using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    public int attack = 40;
    public float attackCD = 1;
    private float lastAttack = 0;
    
    void Start()
    {
        // TODO: Read from JSON
    }

    // Can only attack after a certain CD
    public bool Attack(GameObject target)
    {
        if (Time.time - lastAttack < attackCD)
        {
            return false;
        }
        // Animals have health, while environment doesn't
        if (target.tag == "Animal" || target.tag == "Player")
            target.GetComponent<HealthSystem>().Hurt(this.gameObject, attack);
        else if (target.tag == "Environment")
            target.GetComponent<EnvironmentComponent>().Hit(attack);
        lastAttack = Time.time;
        return true;
    }
}
