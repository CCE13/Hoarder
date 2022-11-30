using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class DungeonBossTile : MonoBehaviour
{
    public SoundPlayer bossMusic;
    public SoundPlayer mainMusic;
    public List<GameObject> toToggle;
    public List<GameObject> AOElocations;

    private SwordMan swordMan;
    private Ranger ranger;

    private DungeonController inst;

    private bool _hasEntered = false;
    private GameObject bossUI;
    private GameObject coinUI;
    private CameraFollow cameraFollow;

    private void Start()
    {
        mainMusic = GameObject.FindGameObjectWithTag("MainBGM").GetComponent<SoundPlayer>();
        inst = DungeonController.instance;

        swordMan = inst.player1.GetComponent<SwordMan>();
        ranger = inst.player2.GetComponent<Ranger>();
        bossUI = Camera.main.transform.GetChild(0).GetChild(0).Find("Boss UI").gameObject;
        coinUI = Camera.main.transform.GetChild(0).GetChild(0).Find("Coin UI").gameObject;
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
    }

    private void FixedUpdate()
    {
        if (_hasEntered) return;

        if (!ranger.isDead && !swordMan.isDead)
        {
            if (
                Vector3.Distance(swordMan.transform.position, gameObject.transform.position) < 45 &&
                Vector3.Distance(ranger.transform.position, gameObject.transform.position) < 45 &&
                !_hasEntered)
            {
                _hasEntered = true;
                // Use here to run any function you wanna run when both players are inside the boss tile.
                bossUI.SetActive(true);
                coinUI.SetActive(false);
                StartCoroutine(CameraZoom());
                TurnOffEverything();
            }
        }

        if (ranger.isDead)
        {
            if (
                Vector3.Distance(swordMan.transform.position, gameObject.transform.position) < 45 &&
                !_hasEntered)
            {
                _hasEntered = true;
                // Use here to run any function you wanna run when both players are inside the boss tile.
                bossUI.SetActive(true);
                coinUI.SetActive(false);
                StartCoroutine(CameraZoom());
                TurnOffEverything();
            }
        }

        if (swordMan.isDead)
        {
            if (
                Vector3.Distance(ranger.transform.position, gameObject.transform.position) < 45 &&
                !_hasEntered)
            {
                _hasEntered = true;
                // Use here to run any function you wanna run when both players are inside the boss tile.
                bossUI.SetActive(true);
                coinUI.SetActive(false);
                StartCoroutine(CameraZoom());
                TurnOffEverything();
            }
        }
    }

    private IEnumerator CameraZoom()
    {
        for (float i = 0; i < 1f; i += Time.deltaTime)
        {
            cameraFollow.camDistance = Mathf.MoveTowards(cameraFollow.camDistance, 20f, i);
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void RandomAOE()
    {
        int aoeCount = Random.Range(3, AOElocations.Count + 1);
        float time = Random.Range(0, 2f);

        // Prenerf.
        //for (int i = 0; i < aoeCount; i++)
        //{
        //    StartCoroutine(AOEing(time, i));
        //}

        StartCoroutine(AOEing(time, aoeCount));
    }

    private IEnumerator AOEing(float t, int aoeCount)
    {
        //// Prenerf.
        //AOElocations[aoeCount].GetComponent<Animator>().Play("AOE");
        //yield return null;

        int aoeDone = 0;
        while(aoeDone < aoeCount)
        {
            AOElocations[aoeDone].GetComponent<Animator>().Play("AOE");
            yield return new WaitForSeconds(t);
            aoeDone++;
        }

    }

    private void TurnOffEverything()
    {
        // Stops enemies spawning on waves,
        // Turns off any enemy present on scene.

        mainMusic.StopAudios();
        bossMusic.PlayLAudio();

        inst.StopAllCoroutines();

        for (int i = 0; i < inst.tilesGenerated.Count - 1; i++)
        {
            inst.tilesGenerated[i].SetActive(false);
        }

        for (int i = 0; i < ObjectPool.SharedInstance.enemyMeleePool.Count; i++)
        {
            ObjectPool.SharedInstance.enemyMeleePool[i].SetActive(false);
        }

        for (int i = 0; i < ObjectPool.SharedInstance.enemyRangedPool.Count; i++)
        {
            ObjectPool.SharedInstance.enemyRangedPool[i].SetActive(false);
        }

        for (int i = 0; i < ObjectPool.SharedInstance.enemyProjectilePool.Count; i++)
        {
            ObjectPool.SharedInstance.enemyProjectilePool[i].SetActive(false);
        }

        foreach (GameObject obj in toToggle)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }
}
