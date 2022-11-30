using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Text highScore;
    public Animator loadingScreen;

    public static bool isReturningToMainMenu;

    private void Awake()
    {
        highScore.text = "Highscore : " + PlayerPrefs.GetInt("HighScore").ToString();
    }

    private void Start()
    {
        if(isReturningToMainMenu)
        {
            isReturningToMainMenu = false;
            loadingScreen.Play("Exit");
        }    
    }

    public void StartLoading()
    {
        loadingScreen.Play("Enter");
        StartCoroutine(Loading_CO());
    }

    private IEnumerator Loading_CO()
    {
        yield return new WaitForSecondsRealtime(1);
        SceneManager.LoadScene("Dungeon");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ClearSave()
    {
        PlayerPrefs.SetInt("HighScore", 0);
        highScore.text = "Highscore : " + PlayerPrefs.GetInt("HighScore").ToString();
    }
}
