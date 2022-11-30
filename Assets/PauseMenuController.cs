using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : UI.UI
{
    public static bool isPaused;
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject gamePlayUI;

    Controls control;

    private void Awake()
    {
        control = InputManager.instance.control;
    }

    private void Start()
    {
        control.SwordsMan.Pause.started += ctx => PauseGame();
        control.Ranger.Pause.started += ctx => PauseGame();
    }
    private void OnDestroy()
    {
        control.SwordsMan.Pause.started -= ctx => PauseGame();
        control.Ranger.Pause.started -= ctx => PauseGame();
    }
    public void PauseGame()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
    private void Resume()
    {
        Time.timeScale = 1f;
        gamePlayUI.SetActive(true);
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        gamePlayUI?.SetActive(false);
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        isPaused = true;
    }
}
