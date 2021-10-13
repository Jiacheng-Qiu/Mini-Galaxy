using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerHealthSystem : HealthSystem
{
    public Image shield;
    public Image energy;

    public float currentOxygen = 100;
    public float maxOxygen = 100;
    public float oxygenRecover = 0;
    public float oxygenConsume = 1;
    public float runOxygenConsume = 2;
    public float runCD = 2;
    private float lastRunAttempt;
    private bool isRun = false;

    public float heartrate = 60;
    private float lastHeartBeat = 0f;
    private float heartAmplitude;
    public GameObject heartSample;

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
        if (isRun)
        {
            currentOxygen = currentOxygen - runOxygenConsume * Time.deltaTime;
        }
        else
        {
            currentOxygen = currentOxygen - oxygenConsume * Time.deltaTime;
        }

        currentOxygen = currentOxygen + oxygenRecover * Time.deltaTime;
        if (currentOxygen > maxOxygen)
        {
            currentOxygen = maxOxygen;
        } 
        else if(currentOxygen <= 0)
        {
            Choke();
        }


        heartAmplitude = (currentHealth <= 0)? 0 : 0.9f * currentHealth + 30;
        shield.fillAmount = currentShield * 1f / maxShield;
        energy.fillAmount = currentOxygen * 1f / maxOxygen;

        // Adjust heartrate based on running state
        if (isRun && heartrate < 150)
        {
            heartrate += 2 * Time.deltaTime;
        } 
        else if (!isRun && heartrate > 60)
        {
            heartrate -= 1 * Time.deltaTime;
        }


        // HeartBeat();
    }

    // Heartbeat is decided based on how severe the movement of player is
    private void HeartBeat()
    {
        if (Time.time > lastHeartBeat + 60f / heartrate)
        {
            GameObject newBeat = Instantiate(heartSample);
            newBeat.transform.parent = GameObject.Find("PlayerUI").transform.Find("Heart sensor");
            newBeat.GetComponent<HeartBeatCtrl>().Init(heartAmplitude);
            lastHeartBeat = Time.time;
        }
    }

    // When player is out of oxygen, keep losing health
    private void Choke()
    {
        currentHealth -= 10 * Time.deltaTime;
        gameObject.GetComponent<InteractionAnimation>().HurtAnimation();
    }

    public bool Run(bool run)
    {
        // If new attempt is shorter than CD, refuse to run
        if (lastRunAttempt + runCD > Time.time)
        {
            isRun = false;
            return isRun;
        }
        isRun = run;
        // Run method will only be true after energy minimum amount check
        if (currentOxygen < 1 && isRun)
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
