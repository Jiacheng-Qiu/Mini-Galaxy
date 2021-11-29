using UnityEngine;
using UnityEngine.UI;

// A loading bar that works based on time
public class LoadingBar : MonoBehaviour
{
    private float startTime;
    private float period;
    private Image image;

    private void Start()
    {
        image = gameObject.GetComponent<Image>();
    }

    public void StartLoad(float period)
    {
        startTime = Time.time;
        this.period = period;
        image.fillAmount = 0;
        image.enabled = true;
    }


    public void StopLoad()
    {
        startTime = -1;
        image.enabled = false;
    }

    private void FixedUpdate()
    {
        if (Time.time <= startTime + period)
        {
            image.fillAmount = (Time.time - startTime) / period;
        } 
        else if (image.IsActive())
        {
            image.enabled = false;
        }
    }
}
