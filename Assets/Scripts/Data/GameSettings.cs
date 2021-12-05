using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public int volume;
    public bool isFullScreen;
    public float sensx;
    public float sensy;
    public int fov;

    void Awake()
    {
        // Making data persistant throughout scenes
        DontDestroyOnLoad(transform.gameObject);
        volume = 0;
        isFullScreen = true;
        sensx = 1;
        sensy = 1;
        fov = 60;
    }
}
