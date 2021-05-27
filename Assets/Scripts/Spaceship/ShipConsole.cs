using UnityEngine;

public class ShipConsole : MonoBehaviour
{
    public Transform aimer;
    public float border = 0.1f;
    public float sensitivity = 50;

    public void MoveAimer(Vector2 position)
    {
        position.x = Mathf.Clamp(position.x / sensitivity, -border, border);
        position.y = Mathf.Clamp(position.y / sensitivity, -border, border);
        aimer.localPosition = position;
    }
}
