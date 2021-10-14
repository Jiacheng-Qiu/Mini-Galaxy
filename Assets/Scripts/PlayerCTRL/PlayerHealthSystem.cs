using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerHealthSystem : HealthSystem
{
    public float currentOxygen = 100;
    public float maxOxygen = 100;
    public float oxygenRecover;
    public float oxygenConsume = 1;
    private bool oxygenProvided;
    public float runOxygenConsume = 2;
    public float runCD = 2;
    private bool isRun = false;


    private InteractionAnimation uiAnimation;


    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        uiAnimation = gameObject.GetComponent<InteractionAnimation>();
        currentHealth = maxHealth;
        currentShield = maxShield;
        oxygenProvided = false;
    }

    void FixedUpdate()
    {
        // Oxygen consumption is different on movement
        if (isRun)
        {
            currentOxygen -= runOxygenConsume * Time.deltaTime;
        }
        else
        {
            currentOxygen -= oxygenConsume * Time.deltaTime;
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
        else if (currentOxygen <= 0)
        {
            Choke();
        }

        // REDO
        if (currentShield < maxShield)
        {
            currentShield = currentShield + shieldHeal * Time.deltaTime;
        }
        if (currentShield > maxShield)
        {
            currentShield = maxShield;
        }

    }

    // When player is out of oxygen, keep losing health
    private void Choke()
    {
        currentHealth -= 10 * Time.deltaTime;
        gameObject.GetComponent<InteractionAnimation>().HurtAnimation();
    }

    
    public new bool Hurt(GameObject attacker, float damage)
    {
        if (immunity)
            return false;
        lastAttacker = attacker;
        // Damage taken is calculated as: damage*100/(100+armor) for hp

        float prevShield = currentShield;
        currentShield -= damage;
        if (currentShield < 0)
        {
            // Exceeding damage on shield will be dealt to hp
            currentHealth += currentShield * 100 / (100 + armor);
            // If low on health, also call warn animation
            uiAnimation.Warning(true);
            uiAnimation.NewHealth(currentHealth / maxHealth);
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
