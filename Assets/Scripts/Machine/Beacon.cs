using UnityEngine;

public class Beacon : Machine
{
    // Defines the length of beacon relative to player
    public GameObject player;

    private void FixedUpdate()
    {
        if (player == null)
            return;
        // The further the player is, the longer the line, and the larger the entire scale is
        float distance = Vector3.Distance(transform.position, player.transform.position);
        // change entire scale
        float scale = 1 + distance / 40;
        transform.localScale = new Vector3(scale, scale, scale);
        // change line length, and then head position
        transform.Find("Line").localScale = new Vector3(1, scale, 1);
        transform.Find("Head").localPosition = new Vector3(0, 1.5f + scale / 2, 0);

        // And make the beacon face player
        
        transform.rotation = Quaternion.FromToRotation(transform.forward, player.transform.forward) * transform.rotation;
    }

    public void SetColor(Color color)
    {
        transform.Find("Line").GetComponent<SpriteRenderer>().color = color;
        transform.Find("Head").GetComponent<SpriteRenderer>().color = color;
    }
}
