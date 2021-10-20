﻿using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerHealthSystem : HealthSystem
{
    public float currentOxygen;
    public float maxOxygen;
    public float oxygenRecover;
    public float oxygenConsume;
    private bool oxygenProvided;
    public float runOxygenConsume;
    public float runCD;
    private bool isRun;

    // Recorded time for last hurt to trigger recovery
    private float lastHit;
    private float recoverTimeAfterHit;


    private InteractionAnimation uiAnimation;


    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        uiAnimation = gameObject.GetComponent<InteractionAnimation>();
        currentHealth = maxHealth;
        currentShield = maxShield;
        oxygenProvided = false;
        isRun = false;
        recoverTimeAfterHit = 3;
    }

    void FixedUpdate()
    {
        OxygenChange();

        // Shield Recovery
        ShieldRecover();

    }
    
    private void OxygenChange()
    {
        // Oxygen consumption is different on movement
        if (currentOxygen > 0)
        {
            if (isRun)
            {
                currentOxygen -= runOxygenConsume * Time.deltaTime;
            }
            else
            {
                currentOxygen -= oxygenConsume * Time.deltaTime;
            }
        }
        else
        {
            Choke();
            Hurt(null, 5 * Time.time, true);
        } 
        // Recover oxygen if provider nearby
        if (oxygenProvided && currentOxygen < maxOxygen)
        {
            currentOxygen += oxygenRecover * Time.deltaTime;
            if (currentOxygen > maxOxygen)
            {
                currentOxygen = maxOxygen;
            }
        }
        uiAnimation.OxygenChange(currentOxygen / maxOxygen);
    }

    // When player is out of oxygen, keep losing health
    private void Choke()
    {
        currentHealth -= 10 * Time.deltaTime;
        uiAnimation.HurtAnimation();
    }

    public void ShieldRecover()
    {
        if (Time.time > lastHit + recoverTimeAfterHit && currentShield < maxShield)
        {
            float prevShield = currentShield;
            currentShield = currentShield + shieldHeal * Time.deltaTime;
            if (currentShield > maxShield)
            {
                currentShield = maxShield;
                uiAnimation.ShieldFull();
            }
            else
            {
                uiAnimation.ShieldChange((currentShield - prevShield) / maxShield);
            }
        }
    }

    public bool HealthRecover(float amount)
    {
        if (currentHealth >= maxHealth)
        {
            return false;
        }
        currentHealth += amount;
        if (currentHealth >= 0.5f * maxHealth)
        {
            uiAnimation.Warning(false);
        }
        uiAnimation.HealthChange(currentHealth / maxHealth);
        return true;
    }

    public bool Hurt(GameObject attacker, float damage, bool directToHealth)
    {
        if (immunity)
            return false;
        // Damage taken from environment
        if (directToHealth)
        {
            currentHealth -= damage;
            if (currentHealth < 0.5f * maxHealth)
            {
                uiAnimation.Warning(true);
            }
            uiAnimation.HealthChange(currentHealth / maxHealth);
            return true;
        }
        
        lastAttacker = attacker;
        lastHit = Time.time;
        // Damage taken is calculated as: damage*100/(100+armor) for hp

        float prevShield = currentShield;
        currentShield -= damage;
        if (currentShield < 0)
        {
            // Exceeding damage on shield will be dealt to hp
            currentHealth += currentShield * 100 / (100 + armor);
            // If low on health, also call warn animation
            if (currentHealth < 0.5f * maxHealth)
            {
                uiAnimation.Warning(true);
            }
            uiAnimation.HealthChange(currentHealth / maxHealth);
            currentShield = 0;
        }

        uiAnimation.ShieldChange((currentShield - prevShield) / maxShield);

        uiAnimation.HurtAnimation();
        return true;
    }

    // Called from movement script to control run behavior
    public bool Run(bool run)
    {
        isRun = run;
        return isRun;
    }

    // While dead, destroy player, and create a crate on ground with everything in backpack
    private void Die()
    {
        Debug.Log("player is dead!");
        // TODO: add crate on ground
        Destroy(this.gameObject);
    }
}
