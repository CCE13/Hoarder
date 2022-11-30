using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemMenu : MonoBehaviour
{
    public TMP_Text _name;
    public string nameStr;
    public Color nameColor = Color.white;

    public Image modifier;
    public Sprite sprite;

    public TMP_Text rarity;
    public string rarityStr;
    public TMP_Text effectorType;
    public string effectorTypeStr;
    public Color effectorTypeColor = Color.white;
    public TMP_Text description;
    [TextArea(1,100)]
    public string descriptionStr;

    private void OnValidate()
    {
        name = nameStr;
        _name.text = nameStr;
        _name.color = nameColor;

        modifier.sprite = sprite;

        rarity.text = rarityStr;
        rarity.color = nameColor;

        effectorType.text = effectorTypeStr;
        effectorType.color = effectorTypeColor;

        description.text = descriptionStr;
    }
}
