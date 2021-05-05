using UnityEngine;

public class VisionTrigger : MonoBehaviour
{
    private AnimalMovement movement;
    private bool isCarnivore;
    private string foodTag;
    private int visionRange;
    public void Init(AnimalMovement animalMovement, bool isCarnivore, string foodTag, int visionRange)
    {
        movement = animalMovement;
        this.isCarnivore = isCarnivore;
        this.foodTag = foodTag;
        this.visionRange = visionRange;
        this.transform.localScale = new Vector3(5, 5, visionRange);
    }

    private void Update()
    {
        // If the previous target is too far away, find a new one
        if (movement.target != null && Vector3.Distance(this.transform.position, movement.target.transform.position) > 2 * visionRange)
        {
            movement.target = null;
        }
    }

    // Record new target if currently don't have one
    private void OnTriggerEnter(Collider other)
    {
        // Triggers happeneing before the class init is self, which won't count
        if (movement == null)
        {
            return;
        }
        // Herbivore feed on plants, carnivore feed on animals
        GameObject hitObject = other.gameObject;
        // Always prioritize player if player enters range
        if (isCarnivore && other.gameObject.tag == "Player")
        {
            movement.target = hitObject;
        }
        // Check if hungry and target is food
        else if (hitObject != transform.parent.gameObject && movement.target == null && hitObject.tag == foodTag)
        {
            movement.target = hitObject;
        }
    }
}
