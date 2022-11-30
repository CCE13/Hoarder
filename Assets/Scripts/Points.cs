using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Player;

public class Points : MonoBehaviour
{

    public int enemyPointsWorth;
    public int PointsAccumulated;

    public List<GameObject> textToDisable;

    [Header("Difficuly Settings")]

    public TMP_Text startingDifficulty;
    public TMP_Text spawnRate;
    public TMP_Text tileNumber;
    public TMP_Text difficultyInterval;
    public TMP_Text difficultyScaling;




    [Header(" Coin Count")]
    public TMP_Text coinCount;
    public TMP_Text pointsFromCoinCount;

    [Header("Enemies Killed")]
    public TMP_Text enemiesKilled;
    public TMP_Text pointsFromEnemiesKilled;

    [Header("Total Points")]
    public TMP_Text totalPoints;

    [Header("Total Damage Dealt")]
    public TMP_Text totalDamageDealt;
    public TMP_Text pointsFromDamageDealt;


    [Space(20)]
    public GameObject goNextButton;
    public GameObject newHighscore;

    public AudioSource points1;
    public AudioSource points2;


    public static int enemyKill;

    private void Awake()
    {

        startingDifficulty.text = PlayerPrefs.GetInt("Difficulty").ToString();
        spawnRate.text = PlayerPrefs.GetInt("Spawn Rate").ToString();
        tileNumber.text = PlayerPrefs.GetInt("Tile Number").ToString();
        difficultyInterval.text = PlayerPrefs.GetInt("Difficulty Interval").ToString();
        difficultyScaling.text = PlayerPrefs.GetFloat("Difficulty Scaling").ToString();

        foreach (GameObject text in textToDisable)
        {
            text.SetActive(false);
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        points1.Play();
        int points = CoinAccumulatedCalculation();

        
        yield return new WaitForSeconds(2f);
        points1.Play();
        int enemyPoints = EnemiesAccumulatedCalculation();

        yield return new WaitForSeconds(2f);
        points1.Play();
        int damageDealt = DamageDealtCalculation();

        yield return new WaitForSeconds(2f);
        points2.Play();
        totalPoints.gameObject.SetActive(true);


        int currentPoints = 0;
        int targetPoints = points + enemyPoints + damageDealt;


        for (float i = 0; i < 1f; i += Time.deltaTime)
        {
            currentPoints = (int)Mathf.Lerp(currentPoints, targetPoints, i);
            PointsAccumulated = currentPoints;
            totalPoints.text = $"{PointsAccumulated} pts";
            yield return null;
        }
        PointsAccumulated = targetPoints;
        totalPoints.text = $"{PointsAccumulated} pts";


        //if the points accumulated is higher than the hgihscore saved.
        CheckHighscore();
    }

    private int DamageDealtCalculation()
    {
        int damageDealt = PlayerController.damageDealt;
        totalDamageDealt.text = $"{damageDealt}";
        pointsFromDamageDealt.text = $"{damageDealt} pts";
        totalDamageDealt.gameObject.SetActive(true);
        pointsFromDamageDealt.gameObject.SetActive(true);
        return damageDealt;
    }

    private void CheckHighscore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore");

        if (highScore < PointsAccumulated)
        {
            PlayerPrefs.SetInt("HighScore", PointsAccumulated);
            //call input
            newHighscore.SetActive(true);
            goNextButton.SetActive(true);
        }
        else goNextButton.SetActive(true);
    }

    private int EnemiesAccumulatedCalculation()
    {
        enemiesKilled.text = enemyKill.ToString();
        var enemyPoints = enemyKill * enemyPointsWorth;
        pointsFromEnemiesKilled.text = $"{enemyPoints} pts";
        enemiesKilled.gameObject.SetActive(true);
        pointsFromEnemiesKilled.gameObject.SetActive(true);
        return enemyPoints;
    }

    private int CoinAccumulatedCalculation()
    {
        coinCount.text = CoinCounter.totalCoinsAccumulated.ToString();
        var points = (int)(CoinCounter.totalCoinsAccumulated * 1.5f);
        pointsFromCoinCount.text = $"{points} pts";
        coinCount.gameObject.SetActive(true);
        pointsFromCoinCount.gameObject.SetActive(true);
        return points;
    }
}
