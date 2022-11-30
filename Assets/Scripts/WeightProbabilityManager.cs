using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeightProbabilityManager : MonoBehaviour
{
    public ModifierCollectableHolder collectableHolder;
    public static List<List<ModiferCollectable>> listOfRarities;


    [Header("Rarity chances")]
    public int commonRarityChance;
    public int uncommonRarityChance;
    public int legendaryRarityChance;

    private static int _sumOfChances;
    private static List<int> weights;
    private static GameObject previousModifier;
    private static ModifierCollectableHolder _collectablHolder;
    private void Start()
    {
    //store the sum of all the chances of each rarity.
        listOfRarities = new List<List<ModiferCollectable>>();


        listOfRarities.Add(collectableHolder.commonModifiers);
        listOfRarities.Add(collectableHolder.rareModifiers);
        listOfRarities.Add(collectableHolder.legendaryModifiers);

        weights = new List<int>();
        weights.Add(commonRarityChance);
        weights.Add(uncommonRarityChance);
        weights.Add(legendaryRarityChance);

        _sumOfChances = commonRarityChance + uncommonRarityChance + legendaryRarityChance;
        _collectablHolder = collectableHolder;
    }
    
    //returns a list of a random rarity base don the weighted probabilities.
    public static List<ModiferCollectable> RandomRarity()
    {
        int randomChance = Random.Range(0, _sumOfChances);
        for (int i = 0; i < weights.Count; i++)
        {
            randomChance -= weights[i];
            if (randomChance < 0)
            {
                return listOfRarities[i];
            }
        }
        Debug.Log("noob");
        return listOfRarities[0];
    }
    
    public static int CoinRarityCost(List<ModiferCollectable> rarityList)
    {
        if(rarityList == _collectablHolder.commonModifiers)
        {
            return _collectablHolder.commonCoinCost;
        }
        if (rarityList == _collectablHolder.rareModifiers)
        {
            return _collectablHolder.rareCoinCost;
        }
        if (rarityList == _collectablHolder.legendaryModifiers)
        {
            return _collectablHolder.legendaryCoinCost;
        }
        return 0;
    }


    /// <summary>
    /// Returns a gameobject of random based on a rarity weighted probability.
    /// </summary>
    public static GameObject RandomCollectibleOfRarity()
    {

        List<ModiferCollectable> listSelected = RandomRarity();
        int index = Random.Range(0, listSelected.Count);
        GameObject modifierSeleced = listSelected[index].gameObject;
        return modifierSeleced;
    }
    public static GameObject RandomCollectibleOfRarity(List<ModiferCollectable> rarity) 
    {

        List<ModiferCollectable> listSelected = rarity;
        int index = Random.Range(0, listSelected.Count);
        GameObject modifierSeleced = listSelected[index].gameObject;
        return modifierSeleced;
    }
}
