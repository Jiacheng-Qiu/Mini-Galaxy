using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePause : MonoBehaviour
{
    public Canvas pauseMenu;
    private bool isActive = false;
    private void FixedUpdate()
    {
        // Constantly checking for ` button, if pressed enter pause menu and pause game
        if(!isActive && Input.GetKeyUp(KeyCode.Tab))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isActive = true;
        pauseMenu.enabled = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.enabled = false;
        isActive = false;
    }
}
