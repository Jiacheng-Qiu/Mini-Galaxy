using System.Collections;
using UnityEngine;

public class AnimalMovement : MonoBehaviour
{
    private float travelled = 0; // The distance walking is recorded, rest after an amount is reached
    private float distBeforeIdle = 40;
    private float nextChangeDirTime; // Record next change direction time
    public float walkSpeed = 6; // Used when there are no targets
    public float runSpeed = 12; // Used when any target is available
    public bool isCarnivore = false;
    private float lastEat; // Record last eating time
    public int basicFeedRate = 30; // Recording eating cd (in seconds)
    private float feedCooldown; // Actual eating rate vary as 1-3 times of basic rate
    public int visionRange = 15;
    public GameObject target; // target is set using vision script
    private HealthSystem health; // Used to keep track of last attacker, can also be used to check self HP

    private void Start()
    {
        // TODO: load all info from json
        health = this.gameObject.GetComponent<HealthSystem>();
        lastEat = Time.time;
        feedCooldown = basicFeedRate * Random.Range(1f, 3f);
        // After finishing loading everything, load vision
        this.transform.Find("Vision").GetComponent<VisionTrigger>().Init(this, isCarnivore, (isCarnivore)? "Animal" : "Environment", (isCarnivore) ? "Meat" : "Wood", visionRange);
    }

    private void Update()
    {
        // Being attacked is prioritized
        if (health.lastAttacker != null)
        {
            // Will stop running if lastAttacker is out of range
            // Carnivores chase attacker, herbivores run away
            if (isCarnivore)
            {
                target = health.lastAttacker;
                Hunt();
            }
            else
            {
                // Rotate and run on the other side the target
                Vector3 targetDirection = (this.transform.position - health.lastAttacker.transform.position).normalized;
                // targetDirection = new Vector3(targetDirection.x, transform.rotation.x, targetDirection.z);
                transform.rotation = Quaternion.FromToRotation(transform.forward, targetDirection) * transform.rotation;

                /*Vector3 from = new Vector3(health.lastAttacker.transform.position.x, 0, health.lastAttacker.transform.position.z);
                Vector3 to = new Vector3(this.transform.position.x, 0, this.transform.position.z);
                transform.rotation = Quaternion.AngleAxis(Vector3.Angle(from, to), transform.up) * transform.rotation;*/
                transform.Translate(0, 0, Time.deltaTime * runSpeed);
            }

            // Will stop running if lastAttacker is out of range
            if (Vector3.Distance(this.transform.position, health.lastAttacker.transform.position) > 2 * visionRange)
            {
                health.lastAttacker = null;
            }
        }
        else
        {
            // Movement control
            if (Time.time - lastEat > feedCooldown && target != null)
            {
                Hunt();
            }
            else
            {
                Move();
            }
        }
    }

    // Hunt behavior for attacking target, 
    private void Hunt()
    {
        
        if (Vector3.Distance(transform.position, target.transform.position) < 2)
        {
            // If the target is food already, eat it instead
            if (target.tag == "Material")
            {
                Eat();
            } else
            {   // Otherwise it's not yet food, Attack target if in range
                this.gameObject.GetComponent<AttackSystem>().Attack(target);
                StartCoroutine(WaitTime(Random.Range(1, 2)));
            }
            return;
        }
        // Rotate and run towards the target
        Vector3 targetDirection = (target.transform.position - this.transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(transform.forward, targetDirection) * transform.rotation;

        /*Vector3 dir = transform.position - target.transform.position;
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, transform.up) * transform.rotation;*/
        transform.Translate(0, 0, Time.deltaTime * runSpeed);
    }

    // Move behavior with random direction and random movement
    private void Move()
    {
        float walkAmount = Time.deltaTime * walkSpeed;
        transform.Translate(0, 0, walkAmount);
        travelled += walkAmount;
        // Can change direction after a certain period of time
        if (Time.time >= nextChangeDirTime)
        {
            transform.rotation = Quaternion.AngleAxis(Random.Range(-40, 40), transform.up) * transform.rotation;
            nextChangeDirTime = Time.time + Random.Range(1.0f, 3.0f);
        }

        // When unit reach its destination, idle for some time and find a new destination
        if (travelled >= 0)
        {
            Idle();
            travelled = - distBeforeIdle * Random.Range(1f, 10f);
        }
    }

    // Just standing there doing nothing for a random amount of time
    private void Idle()
    {
        StartCoroutine(WaitTime(Random.Range(3, 10)));
        return;
    }

    // Eat behavior happen when there is enough food to eat
    private void Eat()
    {
        // Destroy the target after eat
        Destroy(target);
        lastEat = Time.time;
        feedCooldown = basicFeedRate * Random.Range(1f, 3f);
    }

    private IEnumerator WaitTime(int second)
    {
        yield return new WaitForSeconds(second);
    }
}