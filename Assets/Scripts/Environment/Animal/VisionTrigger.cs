using UnityEngine;

public class VisionTrigger : MonoBehaviour
{
    private AnimalMovement movement;
    private bool isCarnivore;
    private int visionRange;
    // The tag of objects that can provide food
    private string foodTag;
    // The name of the food
    private string foodName;
    public void Init(AnimalMovement animalMovement, bool isCarnivore, string foodTag, string foodName, int visionRange)
    {
        movement = animalMovement;
        this.isCarnivore = isCarnivore;
        this.foodTag = foodTag;
        this.visionRange = visionRange;
        this.foodName = foodName;
        transform.localScale = new Vector3(5, 5, visionRange);
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
    // Priorizitize target that can be eaten, Herbivore feed on plants, carnivore feed on animals
    private void OnTriggerEnter(Collider other)
    {
        // Triggers happeneing before the class init is self, which won't count
        if (movement == null)
            return;

        GameObject hitObject = other.gameObject;
        // If directly found food, stop any other behavior
        if (hitObject.tag == "Material" && hitObject.GetComponent<MaterialProperty>().materialName == foodName)
        {
            movement.target = hitObject;
            return;
        }

        // Always prioritize player if player enters range
        if (isCarnivore && other.gameObject.tag == "Player")
        {
            movement.target = hitObject;
        }
        // Check if hungry and target is food (with food tag, and specific name)
        else if (movement.target == null && hitObject != transform.parent.gameObject && hitObject.tag == foodTag)
        {
            // Check if the object can provide food, if so mark as target
            if (hitObject.GetComponent<EnvironmentComponent>().productName == foodName)
                movement.target = hitObject;
        }
    }
}
