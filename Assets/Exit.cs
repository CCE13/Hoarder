using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    private GameObject OptionsUI;

    private void Start()
    {
        OptionsUI = Camera.main.transform.GetChild(0).Find("End Options").gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        OptionsUI.SetActive(true);
    }
}
