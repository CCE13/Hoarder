using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Player;

public class EndGame : MonoBehaviour
{
    public GameObject gameplayUI;
    public GameObject finishUI;
    public ModifierUI swordsmanModiUI;
    public ModifierUI rangerModiUI;

    public Animator loadingScreen;

    private CameraFollow _cam;

    private void Awake()
    {
        _cam = GetComponent<CameraFollow>();
    }

    public void FixedUpdate()
    {
        if(FindObjectOfType<SwordMan>().isDead && FindObjectOfType<Ranger>().isDead)
        {
            End();
        }
    }

    public void End()
    {
        swordsmanModiUI.ReloadList(_cam._swordMan);
        rangerModiUI.ReloadList(_cam._ranger);
        gameplayUI.SetActive(false);
        finishUI.SetActive(true);
    }

    public void ToMainMenu()
    {
        loadingScreen.Play("Enter");
        StartCoroutine(Loading_CO());
    }

    private IEnumerator Loading_CO()
    {
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1;
        MainMenu.isReturningToMainMenu = true;
        SceneManager.LoadScene("MainMenu");
    }

}
