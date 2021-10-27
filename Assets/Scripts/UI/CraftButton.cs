
using UnityEngine;

public class CraftButton : MonoBehaviour
{
    public Crafting craft;

    public void ChangeView()
    {
        craft.OnDetailPage(int.Parse(gameObject.name));
    }
}
