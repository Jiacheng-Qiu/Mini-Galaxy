using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerHealthSystem : HealthSystem
{
    public float currentOxygen;
    public float maxOxygen;
    public float oxygenRecover;
    public float oxygenConsume;
    private bool oxygenProvided;
    public float runCD;
    private bool isRun;
    private bool isWalk;
    public int heartrate;
    private float lastRunHeart;
    private int cachedHeart;
    private float lastHeartUpdate;
    private float lastHeartbeat;
    // Recorded time for last hurt to trigger recovery
    private float lastHit;
    private float recoverTimeAfterHit;
    private InteractionAnimation uiAnimation;

    private void Start()
    {
        heartrate = 60;
        rb = gameObject.GetComponent<Rigidbody>();
        uiAnimation = gameObject.GetComponent<InteractionAnimation>();
        oxygenProvided = false;
        isRun = false;
        isWalk = false;
        recoverTimeAfterHit = 3;
    }

    public void LoadStatus(SaveFormat save)
    {
        this.currentHealth = save.hp;
        this.maxHealth = save.maxHp;
        this.currentShield = save.shield;
        this.maxShield = save.maxShield;
        this.currentOxygen = save.oxygen;
        this.maxOxygen = save.maxOxygen;
        this.armor = save.armor;
        this.heartrate = save.heartrate;
    }

    void FixedUpdate()
    {
        OxygenChange();

        // Shield Recovery
        ShieldRecover();
        
        // Every 3 second in run, increase heartrate by randomly 3-8 till 160; elsewise
        if (Time.time > lastRunHeart)
        {
            if (isRun && heartrate < 120)
            {
                cachedHeart += Random.Range(8, 15);
            }
            else if (!isRun && isWalk)
            {
                if (heartrate < 90)
                    cachedHeart += Random.Range(5, 10);
                else
                    cachedHeart -= Random.Range(5, 10);
            }
            else if (!isRun && !isWalk && heartrate > 60)
            {
                cachedHeart -= 5;
            }
            lastRunHeart = Time.time + 3;
        }

        // Update heartrate every second, and send to animation
        if (cachedHeart != 0 && Time.time > lastHeartUpdate)
        {
            int shiftAmount = (cachedHeart > 2) ? cachedHeart / 3 : cachedHeart;
            cachedHeart -= shiftAmount;
            heartrate += shiftAmount;
            lastHeartUpdate = Time.time + 1;
            uiAnimation.HeartrateChange(heartrate);
        }

        // Call heartbeat function in animation
        if (Time.time > lastHeartbeat + 60f / heartrate)
        {
            uiAnimation.HeartBeat();
            lastHeartbeat = Time.time;
        }
    }
    
    private void OxygenChange()
    {
        // Oxygen consumption is proportional to heartrate
        if (currentOxygen > 0)
        {
            currentOxygen -= oxygenConsume * Time.deltaTime * heartrate / 60f;
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
        if (currentHealth < 0)
            return;
        currentHealth -= 10 * Time.deltaTime;
        heartrate += 10;
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
            cachedHeart += 5;
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

    public bool Walk(bool walk)
    {
        isWalk = walk;
        return isWalk;
    }

    // While dead, destroy player, and create a crate on ground with everything in backpack
    private void Die()
    {
        Debug.Log("player is dead!");
        cachedHeart = 0;
        heartrate = 0;
        // TODO: add crate on ground
        Destroy(this.gameObject);
    }
}
