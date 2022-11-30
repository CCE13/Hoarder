using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public abstract class UI : MonoBehaviour
    {
        public string mainMenu;
        public void QuitGame()
        {
            Application.Quit();
            Debug.Log("Quitting");
        }
        public void ReturnToMainMenu()
        {   
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(mainMenu);
            Debug.Log("Returning to main menu");
        }
    }
}

