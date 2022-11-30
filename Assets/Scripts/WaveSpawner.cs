using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{

    public enum SpawnState { Spawning, Waiting, Counting, Complete };
    private SpawnState state = SpawnState.Counting;

    [System.Serializable]
    public class Wave
    {
        public string name;
        public int count;
        public float rate;
    }
    [SerializeField] private List<Transform> enemySpawnLocations = new List<Transform>();
    public Wave[] waves;

    [Header("Countdowns")]
    public float timeBetweenWaves;
    public float waveCountdown;

    
    private int _currentWaveIndex = 0;
    private float _searchCountdown = 1f;



    void Start()
    {
        waveCountdown = timeBetweenWaves;


    }
    void Update()
    {

        if (state == SpawnState.Waiting)
        {
            if (!EnemyIsAlive())
            {
                //begin a new wave
                WaveCompleted();

            }
            else
            {
                return;
            }
        }

        
        if (state == SpawnState.Complete)
        {
            //if all waves have ben completed, disable this scriipt and allow acces to the other dungeon doors
            Debug.Log("Doors can now be accessed");
        }

        if (waveCountdown <= 0)
        {
            //spawn wave
            if (state != SpawnState.Spawning)
            {
                StartCoroutine(SpawnWave(waves[_currentWaveIndex]));
            }
        }


        //countdown
        else
        {
            waveCountdown -= Time.deltaTime;
        }
    }

    private void WaveCompleted()
    {
        Debug.Log("Wave completed");
        state = SpawnState.Counting;
        waveCountdown = timeBetweenWaves;

        if (_currentWaveIndex + 1 > waves.Length - 1)
        {
            state = SpawnState.Complete;
        }
        else
        {
            _currentWaveIndex++;
        }

    }


    //check if any enemies are alive in the game
    private bool EnemyIsAlive()
    {
        _searchCountdown -= Time.deltaTime;
        if (_searchCountdown <= 0)
        {
            _searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
        return true;

    }

    //spawn a wave
    private IEnumerator SpawnWave(Wave wave)
    {
        state = SpawnState.Spawning;

        //spawn
        for (int i = 0; i < wave.count; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(1f / wave.rate);
        }

        state = SpawnState.Waiting;
        yield break;
    }

    //spawns a enemy
    private void SpawnZombie()
    {
        //spawn enemy
        GameObject enemy = ObjectPool.SharedInstance.GetEnemy(false, 100);
        if (enemy != null)
        {
            Spawner(enemy);
        }
        else
        {
            return;
        }

    }


    private void Spawner(GameObject enemyToSpawn)
    {
        int _eCountSpawn = waves[_currentWaveIndex].count;
        int i = 0;
        while (i < _eCountSpawn && _eCountSpawn != 0)
        {
            foreach (Transform spawnPos in enemySpawnLocations)
            {
                int _toSpawn = Random.Range(0, 2);
                Debug.Log(_toSpawn + " in " + spawnPos);

                if(_toSpawn > 0)
                {
                    // List of enemy type.
                    // Enemy sets should change based on level, wait for me to model.

                    i++;

                    // Test
                    enemyToSpawn.transform.position = spawnPos.position;
                    enemyToSpawn.SetActive(true);
                }
            }
        }
    }

}
