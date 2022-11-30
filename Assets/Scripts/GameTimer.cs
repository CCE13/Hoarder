using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public int hoursPast, minutesPast, secondsPast;
    public float _time;
    public bool startTimer;
    public TMP_Text timerText;

    [Header("Difficulty Slider")]
    public Slider difficultySlider;
    public int initialDiffInterval;
    public float difficultyIntervalScaling;

    public TMP_Text levelTxt;

    private float _diffInterval;
    private DungeonController inst; 

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        inst = DungeonController.instance;

        initialDiffInterval = PlayerPrefs.GetInt("Difficulty Interval");
        difficultyIntervalScaling = PlayerPrefs.GetFloat("Difficulty Scaling");

        _diffInterval = initialDiffInterval * (difficultyIntervalScaling + (inst.difficultyLevel / 3));
        difficultySlider.maxValue = _diffInterval;
        difficultySlider.value = _diffInterval;
        levelTxt.text = inst.difficultyLevel.ToString();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        startTimer = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (startTimer && inst.startGame)
        {
            TimerRun();
        }
    }

    private void TimerRun()
    {
        _time += Time.deltaTime;
        minutesPast = (int)(_time / 60f) % 60;
        secondsPast = (int)(_time % 60f);
        hoursPast = (int)(_time / 3600f);
        timerText.text = $"{hoursPast.ToString("00")}:{minutesPast.ToString("00")}:{secondsPast.ToString("00")}";

        _diffInterval -= Time.deltaTime;
        difficultySlider.value = _diffInterval;

        if (_diffInterval <= 0)
        {
            inst.difficultyLevel++;
            levelTxt.text = inst.difficultyLevel.ToString();
            _diffInterval = initialDiffInterval * (difficultyIntervalScaling + (inst.difficultyLevel / 10));
            difficultySlider.maxValue = _diffInterval;
        }

    }

}
