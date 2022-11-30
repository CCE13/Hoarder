using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUp : MonoBehaviour
{
    public Color crit;
    public Color normal;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [HideInInspector]
    public bool isCrit;

    private TMP_Text text;
    private bool _isBoss;
    // Start is called before the first frame update
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        normal = text.color;
    }

    public void isBoss()
    {
        minX *= 2;
        maxX *= 2;
        minY *= 2;
        maxY *= 2;
        _isBoss = true;
    }

    private void OnEnable()
    {
        StartCoroutine(Pop());
    }

    /// <summary>
    /// Gets a random x and y value and moves the gameobject towards that value + their current transform position
    /// </summary>
    /// <returns></returns>
    public IEnumerator Pop()
    {
        text.color = normal;
        text.fontSize = 12;

        if(isCrit)
        {
            text.color = crit;
            text.text += "!";
            text.fontSize = 14;
        }

        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        Vector3 posToMoveTo = new Vector3(x, y, 0f) + transform.position;
        Color color = new Color(text.color.r, text.color.g, text.color.b,0);
        for (float i = 0; i < 2f; i += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(transform.position, posToMoveTo, i);
            text.color = Color.Lerp(text.color, color, i/50);
            yield return null;
        }

        if(_isBoss)
        {
            minX /= 2;
            maxX /= 2;
            minY /= 2;
            maxY /= 2;
            _isBoss = false;
        }

        gameObject.SetActive(false);
    }
}
