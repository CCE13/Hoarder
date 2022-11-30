using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[DefaultExecutionOrder(-1000)]

public class DungeonController : MonoBehaviour
{
    [Header("Sets")]
    public List<GameObject> tileSet;
    public List<GameObject> monuments;
    public GameObject bossTile;
    [Space(10)]
    public List<GameObject> enviromentalProps;
    public GameObject chest;

    [Header("Players")]
    [Space(10)]
    public GameObject player1;
    public GameObject player2;

    [Header("Logistics")]
    public float enemysToSpawn;
    [Space(10)]
    public int tilesToGenerate;
    public int tilesBuffer;
    [Tooltip("The upper limit of randomRange.")]
    public int monumentRate;
    [Tooltip("The upper limit of randomRange.")]
    public int restTileRate;
    public List<GameObject> tilesGenerated = new();

    [Space (5)]
    [Tooltip("Place starting tile here.")]
    public DungeonTile rootTile;

    public int tilesExplored;

    public static DungeonController instance;

    private NavMeshSurface _navSurface;
    private Vector3 _p1StartPos;
    private Vector3 _p2StartPos;

    private bool _doneGenerating;
    private bool _hasRan;

    [HideInInspector]
    public bool startGame;

    public float _failSafeCheckDuration;

    [HideInInspector]
    public int _bossRoomSpawnerID = 1;

    public int difficultyLevel;

    [HideInInspector]
    public List<Transform> spawnLocations = new List<Transform>();

    private bool _runAlr;

    private void Awake()
    {
        difficultyLevel = PlayerPrefs.GetInt("Difficulty");
        enemysToSpawn = PlayerPrefs.GetInt("Spawn Rate");
        tilesToGenerate = PlayerPrefs.GetInt("Tile Number");

        instance = this;
        _navSurface = GetComponent<NavMeshSurface>();
        _p1StartPos = player1.transform.position;
        _p2StartPos = player2.transform.position;
    }

    private void TileCheck()
    {
        if(tilesGenerated.Count < tilesToGenerate + 1)
        {       
            RestartGeneration(tilesToGenerate);
        }
    }

    private void Update()
    {
        // Failsafe so that if the dungeon does not generate to required amount, regenerate.
        if (!_doneGenerating && _failSafeCheckDuration > 0)
        {
            _failSafeCheckDuration -= 0.1f;

            if (_failSafeCheckDuration > 15)
            {
                _failSafeCheckDuration = 7;
            }

            if (_failSafeCheckDuration < 0.4f)
            {
                TileCheck();
            }
        }

        // When the dungeon has generated to the required amount, spawn the boss tile.
        if(tilesGenerated.Count == tilesToGenerate && !_hasRan)
        {
            _hasRan = true;

            if (tilesGenerated.Count == tilesToGenerate)
            {
                tilesGenerated[^_bossRoomSpawnerID].GetComponent<DungeonTile>().CreateBossRoom(0);
            }
        }
    }

    private IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(20);

        int _toSpawn = Mathf.RoundToInt(enemysToSpawn);

        while (_toSpawn > 0)
        {
            if (spawnLocations.Count == 0) break;
            // List of enemy type.
            int enemyVar = Random.Range(0, 2);
            bool enemyType = true;

            int i = Random.Range(0, spawnLocations.Count);

            if (enemyVar == 0)
            {
                enemyType = false;
            }
            else enemyType = true;

            var _spawnedEnemy = ObjectPool.SharedInstance.GetEnemy(enemyType, 100 + (50 * difficultyLevel));

            _spawnedEnemy.transform.SetPositionAndRotation(spawnLocations[i].position, Quaternion.Euler(new Vector3(0, Random.Range(0, 361), 0)));

            _spawnedEnemy.SetActive(true);

            _toSpawn--;
        }

        StartCoroutine(SpawnEnemies());

    }

    public void BossRoomSpawn()
    {
        // This function is ONLY called when the last tile is unable to spawn the boss room.
        tilesGenerated[^_bossRoomSpawnerID].GetComponent<DungeonTile>().CreateBossRoom(0);

        // This prevents the need to unnecessarily restart the dungeon generation.
    }

    public void GenerateNavMesh()
    {
        for (int i = 0; i < tilesGenerated.Count - 1; i++)
        {
            var _tile = tilesGenerated[i].GetComponent<DungeonTile>();
            if (_tile == null) break;
            _tile.CheckForWall();
            _tile.CreateEnvironmentals();
        }

        rootTile.CheckForWall();

        // Creates a walkable surface for enemies to path towards _player.
        _navSurface.BuildNavMesh();

        // When the navmeshSurface is done generating, hide the tiles.
        if(_navSurface.navMeshData)
        {
            TurnOffTiles();
            StartCoroutine(SpawnEnemies());
        }

    }

    private void TurnOffTiles()
    {
        foreach(GameObject tiles in tilesGenerated)
        {
            tiles.GetComponent<DungeonTile>().SwitchWallCol();
            tiles.SetActive(false);
        }

        _doneGenerating = true;

        // Shift this to a sperate function so that _player can start when they're ready.
        startGame = true;
    }

    public void StartGame()
    {
        rootTile.NextTile();
    }

    public void RestartGeneration(int _tilesToGenerate)
    {
        startGame = false;
        // Destroy all the tiles and clear the list.
        foreach (GameObject tile in tilesGenerated)
        {
            Destroy(tile);
        }

        tilesGenerated.Clear();
        tilesToGenerate = _tilesToGenerate;

        // Bring the players to the start position.
        player1.transform.position = _p1StartPos;
        player2.transform.position = _p2StartPos;

        _doneGenerating = false;
        _failSafeCheckDuration = 5;
        _bossRoomSpawnerID = 1;
        _hasRan = false;

        Player.Ranger ranger = player2.GetComponent<Player.Ranger>();
        Player.SwordMan swordman = player1.GetComponent<Player.SwordMan>();

        if (ranger.isDead) ranger.Revived(ranger, player2.transform);
        if (swordman.isDead) swordman.Revived(swordman, player1.transform);

        // Start the generation.
        Invoke("CreateTile", 0.2f);
    }

    private void CreateTile()
    {
        rootTile.CreateTile();
    }
}
