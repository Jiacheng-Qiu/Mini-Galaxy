using System.Collections;
using System.Collections.Generic;
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
        Debug.Log(this.gameObject.name + " attacked target!");
        target.GetComponent<HealthSystem>().Hurt(this.gameObject, attack);
        lastAttack = Time.time;
        return true;
    }
}
