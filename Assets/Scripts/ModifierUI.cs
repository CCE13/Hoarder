using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Player;

public class ModifierUI : MonoBehaviour
{
    public List<Image> slots;

    public void ReloadList(PlayerController player)
    {
        Dictionary<string, int> dict = player.modifier;
        List<Modifier> modifierCollected = player.modifiersCollected;
        List<Modifier> modifierToShow = new List<Modifier>();

        //List<Modifier> modifierChecked = new List<Modifier>();
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].sprite = null;
            slots[i].color = Color.clear;
            slots[i].transform.GetChild(0).GetComponent<TMP_Text>().text = string.Empty;
        }

        for (int i = 0; i < modifierCollected.Count; i++)
        {
            if(!modifierToShow.Contains(modifierCollected[i]))
            {
                modifierToShow.Add(modifierCollected[i]);
            }
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (i >= modifierToShow.Count) return;

            int modifierCount = dict[modifierToShow[i].name];

            slots[i].sprite = modifierToShow[i].spriteToShow;
            slots[i].color = Color.white;
            slots[i].transform.GetChild(0).GetComponent<TMP_Text>().text = $"x{modifierCount}";
        }
    }
}
