using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCounter : MonoBehaviour
{
    public AudioSource coinSFX;
    public int currentCoinCount;
    public TMP_Text coinCountText;
    public static int totalCoinsAccumulated;

    private void Start()
    {
        totalCoinsAccumulated = 0;
        SetCoin.addGold += AddGold;
    }

    private void OnDestroy()
    {
        SetCoin.addGold -= AddGold;
    }


    public void AddGold(int goldToAdd)
    {
        totalCoinsAccumulated += goldToAdd;
        StartCoroutine(Adding(goldToAdd));
    }
    public void LoseGold(int goldToLose)
    {
        StartCoroutine(Subtracting(goldToLose));
    }

    private IEnumerator Adding(int goldToAdd)
    {
        coinSFX.Play();
        int currentGold = currentCoinCount;
        int targetGold = goldToAdd + currentCoinCount;
        for (float i = 0; i < 1; i+=Time.deltaTime)
        {
            currentGold = (int)Mathf.Lerp(currentGold, targetGold, i);
            currentCoinCount = currentGold;
            coinCountText.text = $"<sprite=0>{currentCoinCount}";
            yield return null;
        }
        currentCoinCount = targetGold;
        coinCountText.text = $"<sprite=0>{currentCoinCount}";
    }
    private IEnumerator Subtracting(int goldToSubtract)
    {
        int currentGold = currentCoinCount;
        int targetGold = currentCoinCount - goldToSubtract;

        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            currentGold = (int)Mathf.Lerp(currentGold, targetGold, i);
            currentCoinCount = currentGold;
            coinCountText.text = $"<sprite=0>{currentCoinCount}";
            yield return null;
        }
        currentCoinCount = targetGold;
        coinCountText.text = $"<sprite=0>{currentCoinCount}";
    }
}
