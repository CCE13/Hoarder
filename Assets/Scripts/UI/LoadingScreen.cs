using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;
    public TMP_Text loadingTips;
    public Slider loadingSlider;
    public TMP_Text sliderTxt;
    public Animator anim;

    [TextArea(0,10)]
    public List<string> dungeonTips;

    private DungeonController inst;
    private bool done = true;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        anim = GetComponent<Animator>();
        inst = DungeonController.instance;
        loadingSlider.maxValue = 100;

        int randomTip = Random.Range(0, dungeonTips.Count);
        loadingTips.text = dungeonTips[randomTip];
        InvokeRepeating(nameof(Tips), 3, 3);
        StartCoroutine(Loading());
    }

    private void Tips()
    {
        int randomTip = Random.Range(0, dungeonTips.Count);
        loadingTips.text = dungeonTips[randomTip];
    }

    private void Update()
    {
        if(inst && inst.startGame && !done)
        {
            done = true;
            StopAllCoroutines();
            loadingSlider.value = 100;
            sliderTxt.text = "100%";
            anim.Play("Exit");
            inst.StartGame();
        }
    }

    private IEnumerator Loading()
    {
        float timePassed = 0;
        float value = 0;

        while (timePassed < 5)
        {
            value = Mathf.Lerp(0, 80, timePassed / 5);
            loadingSlider.value = value;
            sliderTxt.text = value.ToString("F0") + "%";
            timePassed += Time.deltaTime;
            yield return null;
        }

        loadingSlider.value = 80;
        done = false;
    }

    public void Restart()
    {
        loadingSlider.value = 0;
        anim.Play("Enter");
        StartCoroutine(Loading());
    }
}
