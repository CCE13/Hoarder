using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTile : MonoBehaviour
{
    [Header("ID")]
    public int tileID;
    public bool isMonument;

    [Header("Enemy Logistics")]
    public int enemyCount;

    [SerializeField] private List<Transform> enemySpawnLocations = new List<Transform>(); 
    public List<DungeonGen> _dungeonGens = new List<DungeonGen>();

    private DungeonController inst;

    [HideInInspector]
    public int _bossdgValue;
    [HideInInspector]
    public bool _nextTileNoEnemies;

    private void Start()
    {
        inst = DungeonController.instance;

        var _generatedTiles = inst.tilesGenerated;

        // Get ID
        if (_generatedTiles.Count < 1)
        {
            return;
        }

        for (int i = 0; i < _generatedTiles.Count; i++)
        {
            if (_generatedTiles[i].name == name)
            {
                tileID = i + 1;
                break;
            }
        }
    }

    public void CreateTile()
    {
        for (int i = 0; i < _dungeonGens.Count; i++)
        {
            _dungeonGens[i].CreateTile();
        }
    }

    public void CreateBossRoom(int dungeonGens)
    {
        // In case that this script did not successfully get the dungeon controller instance.
        if (!inst)
        {
            inst = DungeonController.instance;
        }

        if (dungeonGens > _dungeonGens.Count - 1)
        { 
            Debug.Log("The boss room was unable to spawn in Tile " + tileID);
            inst._bossRoomSpawnerID++;
            inst.BossRoomSpawn();
            return;
        }
        _dungeonGens[dungeonGens].CreateBossTile();
    }

    public void EnemyCheck()
    {
        if (enemyCount > 0 || enemyCount < 0) return;

        foreach(DungeonGen dG in _dungeonGens)
        {
            if (dG.nextTile)
            {
                dG.nextTile.SetActive(true);

                var nextDT = dG.nextTile.GetComponent<DungeonTile>();

                nextDT.SwitchWall();

                if (nextDT.isMonument)
                {
                    nextDT.NextTile();
                    // Disables the door so that the _player can pass.
                    dG.gameObject.SetActive(false);
                    return;
                }

                // SCALE ENEMY COUNT PER ROOM HERE.
                inst.enemysToSpawn = 1 + inst.difficultyLevel;
                inst.enemysToSpawn += 0.3f;

                nextDT.SpawnEnemy(Mathf.RoundToInt(inst.enemysToSpawn));
                nextDT.enemyCount = Mathf.RoundToInt(inst.enemysToSpawn);

                nextDT.EnemyCheck();

                // Disables the door so that the _player can pass.
                dG.gameObject.SetActive(false);
            }
        }
        
    }

    public void NextTile()
    {
        // Same code as EnemyCheck, just without the checking.
        for (int i = 0; i < _dungeonGens.Count; i++)
        {
            if (_dungeonGens[i].nextTile)
            {
                var nextDT = _dungeonGens[i].nextTile.GetComponent<DungeonTile>();

                nextDT.gameObject.SetActive(true);

                nextDT.SwitchWall();

                inst.tilesExplored++;

                if (_nextTileNoEnemies && i == 0)
                {

                    nextDT.NextTile();
                    // Disables the door so that the _player can pass.
                    _dungeonGens[i].gameObject.SetActive(false);

                }

                if (nextDT.isMonument)
                {
                    nextDT.NextTile();
                    // Disables the door so that the _player can pass.
                    _dungeonGens[i].gameObject.SetActive(false);
                }
                else
                {
                    nextDT.SpawnEnemy(Mathf.RoundToInt(inst.enemysToSpawn));
                    nextDT.enemyCount = Mathf.RoundToInt(inst.enemysToSpawn);

                    nextDT.EnemyCheck();

                    // Disables the door so that the _player can pass.
                    _dungeonGens[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void SpawnEnemy(int amount)
    {
        if (enemySpawnLocations.Count < 1) return;

        // Spawns enemy on creation.
        for (int i = 0; i < amount; i++)
        {
            // Get a random spawn location.
            int _toSpawn = Random.Range(0, enemySpawnLocations.Count);

            // List of enemy type.
            int enemyVar = Random.Range(0, 2);
            bool enemyType = true;

            if(enemyVar == 0)
            {
                enemyType = false;
            }

            var _spawnedEnemy = ObjectPool.SharedInstance.GetEnemy(enemyType, 100 + (50 * inst.difficultyLevel));
            var _deComponent = _spawnedEnemy.GetComponent<DungeonEnemy>();
            _deComponent._Tile = this;

            _spawnedEnemy.transform.position = enemySpawnLocations[_toSpawn].position;
            _spawnedEnemy.transform.rotation = Quaternion.Euler(RandomVector3(new Vector3(0, 360, 0), Vector3.zero, false));

            _spawnedEnemy.SetActive(true);
        }
    }

    public void CheckForWall()
    {
        foreach(DungeonGen _dg in _dungeonGens)
        {
            _dg.WallCheck();
        }
    }

    public void SwitchWall()
    {
        foreach (DungeonGen _dg in _dungeonGens)
        {
            _dg.SwitchWall();
        }
    }
    public void SwitchWallCol()
    {
        foreach (DungeonGen _dg in _dungeonGens)
        {
            _dg.SwitchWallCol();
        }
    }

    public void CreateEnvironmentals()
    {
        // In case that this script did not successfully get the dungeon controller instance.
        if (!inst)
        {
            inst = DungeonController.instance;
        }

        // Spawn chest

        int _chestAmt = Random.Range(1, 5);

        for (int i = 0; i < _chestAmt; i++)
        {
            var _chest = Instantiate(
                            inst.chest,
                            RandomVector3(new Vector3(12, 0.5f, 12), transform.position, true),
                            Quaternion.Euler(RandomVector3(new Vector3(0, 360, 0), Vector3.zero, false))
                        );
            _chest.transform.parent = transform;
        }

        // Spawn environmental props

        if (inst.enviromentalProps.Count < 1) return;

        int _environmentPropAmt = Random.Range(0, 4);

        for (int i = 0; i < _environmentPropAmt; i++)
        {
            var _prop = Instantiate(
                inst.enviromentalProps[Random.Range(0, inst.enviromentalProps.Count)],
                RandomVector3(new Vector3(10, 1, 10), transform.position , true),
                Quaternion.Euler(RandomVector3(new Vector3(0, 360, 0), Vector3.zero, false))
                );

            _prop.transform.parent = transform;
        }
    }

    private Vector3 RandomVector3(Vector3 size, Vector3 position, bool staticY)
    {
        Vector3 _output = Vector3.zero;

        float x = Random.Range(-size.x, size.x + 1);
        float y = Random.Range(-size.y, size.y + 1);
        float z = Random.Range(-size.z, size.z + 1);

        if (staticY) y = size.y;
        _output = new Vector3(position.x + x, position.y + y, position.z + z);

        return _output;
    }
}
