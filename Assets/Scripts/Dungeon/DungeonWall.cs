using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonWall : MonoBehaviour
{
    public GameObject[] wallDoor;
    public int runAlr = 0;

    public void Switch()
    {
        if (runAlr >= 3) return;

        foreach (GameObject obj in wallDoor)
        {
            if(obj != this)
            {
                obj.SetActive(!obj.activeSelf);
            }
        }

        runAlr++;

        gameObject.SetActive(false);
    }

    public void SwitchCollider()
    {
        foreach (GameObject obj in wallDoor)
        {
            if (obj != this)
            {
                obj.SetActive(!obj.activeSelf);
            }
        }

        runAlr++;

        gameObject.GetComponent<BoxCollider>().enabled = !gameObject.GetComponent<BoxCollider>().enabled;
    }
}
