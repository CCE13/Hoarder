using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOptions : MonoBehaviour
{
    DungeonController inst;

    public void ContinueRun()
    {
        inst = DungeonController.instance;
        GameObject.FindGameObjectWithTag("MainBGM").GetComponent<SoundPlayer>().PlayLAudio();
        LoadingScreen.instance.Restart();
        inst.tilesToGenerate += 5;
        inst.RestartGeneration(inst.tilesToGenerate);

        gameObject.SetActive(false);
    }

    public void EndRun()
    {
        Camera.main.GetComponent<EndGame>().End();
        gameObject.SetActive(false);
    }
}
