using UnityEngine;

public class GamePause : MonoBehaviour
{
    public Canvas pauseMenu;
    private bool isActive = false;
    private void FixedUpdate()
    {
        // Constantly checking for esc button, if pressed enter pause menu and pause game
        if(!isActive && Input.GetKeyUp(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        PlayerStatus.attackDisabled = true;
        PlayerStatus.moveDisabled = true;
        isActive = true;
        pauseMenu.enabled = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerStatus.attackDisabled = false;
        PlayerStatus.moveDisabled = false;
        Time.timeScale = 1;
        pauseMenu.enabled = false;
        isActive = false;

        gameObject.GetComponent<PlayerMovement>().ChangeSettings();
    }
}
