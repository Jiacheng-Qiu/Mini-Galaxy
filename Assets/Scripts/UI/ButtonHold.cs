using UnityEngine;

// Keep track of player holding one button and return progress
public class ButtonHold
{
    private float requiredTime;
    public float startTime;
    private bool onHold;

    public ButtonHold(float requiredTime)
    {
        NewHold(requiredTime);
    }

    // Start new record
    public void NewHold(float requiredTime)
    {
        startTime = Time.time;
        this.requiredTime = requiredTime;
        onHold = true;
    }

    // Return the current progress in percent, -1 if lost hold
    public float ReportProgress()
    {
        return (onHold)? (Time.time - startTime) / requiredTime : -1;
    }

    // Stop keepin track
    public void StopHold()
    {
        onHold = false;
    }

}
