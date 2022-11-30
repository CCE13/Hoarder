using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CustomisationController : MonoBehaviour
{
    public bool requireFloats = false;

    public TMP_Text variableToControl;

    [Tooltip("Max int it can go for a starting number")]
    public float maxValuePossible;

    [Tooltip("Min int it can go for a starting number")]
    public float minValuePosisble;

    private float currentValue;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey(variableToControl.name))
        {
            if (requireFloats)
            {
                currentValue = PlayerPrefs.GetFloat(variableToControl.name);

            }
            else
            {
                currentValue = PlayerPrefs.GetInt(variableToControl.name);
            }
            
        }
        else
        {
            currentValue = minValuePosisble;
            if (requireFloats)
            {
                PlayerPrefs.SetFloat(variableToControl.name, currentValue);
            }
            else
            {
                PlayerPrefs.SetInt(variableToControl.name, (int)currentValue);
            }            
        }
        
        variableToControl.text = currentValue.ToString();
    }

    public void Minus()
    {
        if (requireFloats)
        {
            currentValue -= 0.1f;
            
        }
        else
        {
            currentValue--;
        }
        currentValue = Mathf.Clamp(currentValue, minValuePosisble, maxValuePossible);

        if (requireFloats)
        {
            PlayerPrefs.SetFloat(variableToControl.name, currentValue);
            variableToControl.text = currentValue.ToString("f1");
        }
        else
        {
            PlayerPrefs.SetInt(variableToControl.name, (int)currentValue);
            variableToControl.text = currentValue.ToString();
        }
    }
    public void Add()
    {
        if (requireFloats)
        {
            currentValue += 0.1f;
           
        }
        else
        {
            currentValue++;
        }
        currentValue = Mathf.Clamp(currentValue, minValuePosisble, maxValuePossible);

        if (requireFloats)
        {
            PlayerPrefs.SetFloat(variableToControl.name, currentValue);
            variableToControl.text = currentValue.ToString("f1");
        }
        else
        {
            PlayerPrefs.SetInt(variableToControl.name, (int)currentValue);
            variableToControl.text = currentValue.ToString();
        }
        
    }
}
