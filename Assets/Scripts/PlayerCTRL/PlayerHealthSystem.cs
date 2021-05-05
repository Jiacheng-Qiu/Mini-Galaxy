using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerHealthSystem : HealthSystem
{
    public Image health;
    public Image shield;
    public Image energy;

    public float currentEnergy = 100;
    public float maxEnergy = 100;
    public float energyRecover = 10;
    public float energyUse = 20;
    public float runCD = 2;
    private float lastRunAttempt;
    private bool isRun = false;

    void FixedUpdate()
    {
        if (currentShield < maxShield)
        {
            currentShield = currentShield + shieldHeal * Time.deltaTime;
        }
        if (currentShield > maxShield)
        {
            currentShield = maxShield;
        }
        // Recover energy
        if (isRun)
        {
            currentEnergy = currentEnergy - energyUse * Time.deltaTime;
        }
        else if (currentEnergy < maxEnergy)
        {
            currentEnergy = currentEnergy + energyRecover * Time.deltaTime;
        }

        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
        health.fillAmount = currentHealth * 1f / maxHealth;
        shield.fillAmount = currentShield * 1f / maxShield;
        energy.fillAmount = currentEnergy * 1f / maxEnergy;
    }

    public bool run(bool run)
    {
        // If new attempt is shorter than CD, refuse to run
        if (lastRunAttempt + runCD > Time.time)
        {
            isRun = false;
            return isRun;
        }
        isRun = run;
        // Run method will only be true after energy minimum amount check
        if (currentEnergy < 1 && isRun)
        {
            // if player still want to run while no energy, seton a CD
            lastRunAttempt = Time.time;
            isRun = false;
        }
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
