using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[System.Serializable]
public class HealthSystem : MonoBehaviour
{

    Rigidbody rb;
    // TODO: later implementation will read health from json
    // Shield takes full damage, HP takes reduced damage based on defense
    public int maxShield = 0;
    private int currentShield;
    public int maxHealth = 100;
    private int currentHealth;
    public int shieldHeal = 5;
    public int hpHeal = 1;
    public int armor = 10;
    public bool immunity = false;

    public GameObject lastAttacker = null;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        //TODO: Read properties from JSON
        currentHealth = maxHealth;
        currentShield = maxShield;
        InvokeRepeating("Recover", 1f, 1f);
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Heal if damaged, called every second
    private void Recover()
    {
        if (currentShield < maxShield)
            currentShield = (maxShield - currentShield > shieldHeal) ? currentShield + shieldHeal : maxShield;
        if (currentHealth < maxHealth)
            currentHealth = (maxHealth - currentHealth > hpHeal) ? currentHealth + hpHeal : maxHealth;
    }

    public bool Hurt(GameObject attacker, int damage)
    {
        if (immunity)
            return false;
        lastAttacker = attacker;
        // Damage taken is calculated as: damage*100/(100+armor) for hp
        currentShield -= damage;
        if (currentShield < 0)
        {
            currentHealth += currentShield * 100 / (100 + armor);
            currentShield = 0;
        }
        // object will bump a bit after being hit
        rb.AddForce(transform.up * 300 * rb.mass);

        //TODO: assign hurted animation

        return true;
    }

    // Destroy current object mechanics on death, leave body for interaction
    private void Die()
    {
        // TODO: Assign death animation
        Debug.Log(this.gameObject.name + "is dead!");
        MaterialProperty property= this.gameObject.AddComponent<MaterialProperty>();
        // TODO: assign material property read from json
        property.materialName = "Meat";
        this.gameObject.tag = "Material";
        Destroy(this.gameObject.GetComponent<AnimalGravity>());
        Destroy(this.gameObject.GetComponent<AnimalMovement>());
        Destroy(this.gameObject.GetComponent<AttackSystem>());
        Destroy(this);
    }
}
