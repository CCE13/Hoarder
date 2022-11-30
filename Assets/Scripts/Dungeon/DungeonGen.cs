using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGen : MonoBehaviour
{
    public bool mustSpawn;

    [Header("Generation")]
    [Range(0, 50)]
    public int checkRange;

    public List<GameObject> objToToggle;
    public GameObject collidedObj;

    [HideInInspector]
    public GameObject nextTile;

    private DungeonController inst;

    private int ID;

    public enum doorFacing
    {
        forward,
        left,
        right,
        back
    }

    public doorFacing dir;

    private Vector3 _castDir;
    private DungeonTile parentTile;

    private DungeonWall _wallToChange;

    private void Awake()
    {
        // Gets the direction where the door needs to check relative to the tile it's a child under.
        _castDir = (transform.position - transform.parent.position).normalized;
        if (_castDir.x > 0.5) _castDir.x = 1;
        if (_castDir.z > 0.5) _castDir.z = 1;
        if (_castDir.x < -0.5) _castDir.x = -1;
        if (_castDir.z < -0.5) _castDir.z = -1;
        _castDir.y = 0;
        //Debug.Log("Direction of door " + name + " " + _castDir);

        parentTile = transform.parent.GetComponent<DungeonTile>();

        // Dungeon door randomizer with buffers.
        if (mustSpawn)
        {
            parentTile._dungeonGens.Add(this);
            return;
        }

        if (Random.Range(0, 10) > 8)
        {
            parentTile._dungeonGens.Add(this);
            return;
        }

        foreach (GameObject obj in objToToggle)
        {
            obj.SetActive(!obj.activeSelf);
        }
        gameObject.SetActive(false);
    }

    private void Start()
    {
        inst = DungeonController.instance;
        GetID();
        CreateTile();
        //Invoke("LateStart", 0.1f);
    }

    //private void LateStart()
    //{
    //    GetID();
    //    CreateTile();
    //}

    public void GetID()
    {
        ID = inst.tilesGenerated.Count;
    }

    //private void OnDrawGizmos()
    //{
    //    // CAN BE REMOVED WHENEVER, USED FOR DEBUGGING.
    //    if (Application.isPlaying)
    //    {
    //        Gizmos.color = Color.red;

    //        var _nextTileCenter = _castDir * checkRange + transform.position - new Vector3(0, transform.position.y, 0);

    //        _nextTileCenter.x = Mathf.RoundToInt(_nextTileCenter.x);
    //        _nextTileCenter.y = Mathf.RoundToInt(_nextTileCenter.y);
    //        _nextTileCenter.z = Mathf.RoundToInt(_nextTileCenter.z);

    //        var _thisPositionOffset = new Vector3(transform.position.x, 0, transform.position.z);

    //        Gizmos.DrawLine(_thisPositionOffset, _nextTileCenter);
    //    }    
    //}

    public void CreateTile()
    {
        // In case that this script did not successfully get the dungeon controller instance.
        if (!inst)
        {
            inst = DungeonController.instance;
        }
        // Called upon when no enemies are left in room.
        if (DungeonController.instance.tilesGenerated.Count >= DungeonController.instance.tilesToGenerate)
        {
            return;
        }

        // Gets the center of the next tile and round it.
        var _nextTileCenter = _castDir * checkRange + transform.position - new Vector3(0, transform.position.y, 0);

        _nextTileCenter.x = Mathf.RoundToInt(_nextTileCenter.x);
        _nextTileCenter.y = Mathf.RoundToInt(_nextTileCenter.y);
        _nextTileCenter.z = Mathf.RoundToInt(_nextTileCenter.z);

        if (checkForGround()) return;

        // Check if the next tile should be a monument

        if(Random.Range(0, inst.monumentRate) == inst.monumentRate - 1)
        {
            int _mToSpawn = Random.Range(0, inst.monuments.Count);

            nextTile = Instantiate(inst.monuments[_mToSpawn], _nextTileCenter, Quaternion.identity);

            int _nextMID = ID + 1;

            nextTile.name = inst.monuments[_mToSpawn].name + " " +_nextMID;

            inst.tilesGenerated.Add(nextTile);

            // Add time to the failsafe so that it doesn't restart the generation.
            inst._failSafeCheckDuration += 1f;

            // Bypass the next chunk of code.
            return;
        }

        // Randomizes the next tile to spawn and set its ID.
        int _tileToSpawn = Random.Range(0, inst.tileSet.Count);

        // Checks for tile buffer so that there would not be dead ends early.
        if(inst.tilesGenerated.Count < inst.tilesBuffer)
        {
            _tileToSpawn = 0;
        }

        nextTile = Instantiate(inst.tileSet[_tileToSpawn], _nextTileCenter, transform.rotation);

        int _nextTileID = ID + 1;

        nextTile.name = "Tile " + _nextTileID;

        inst.tilesGenerated.Add(nextTile);

        // Chance if next tile does not contain any enemies.
        if(Random.Range(0, inst.restTileRate) == inst.restTileRate - 1)
        {
            parentTile._nextTileNoEnemies = true;
        }

        // Add time to the failsafe so that it doesn't restart the generation.
        inst._failSafeCheckDuration += 1f;
    }

    public void CreateBossTile()
    {
        // In case that this script did not successfully get the dungeon controller instance.
        if (!inst)
        {
            inst = DungeonController.instance;
        }

        // Gets the center of the next tile and round it.
        var _nextTileCenter = _castDir * (checkRange + 31) + transform.position - new Vector3(0, transform.position.y, 0);

        _nextTileCenter.x = Mathf.RoundToInt(_nextTileCenter.x);
        _nextTileCenter.y = Mathf.RoundToInt(_nextTileCenter.y);
        _nextTileCenter.z = Mathf.RoundToInt(_nextTileCenter.z);

        if (checkForGroundBoss())
        {
            parentTile._bossdgValue++;
            parentTile.CreateBossRoom(parentTile._bossdgValue);
            return;
        }

        nextTile = Instantiate(inst.bossTile, _nextTileCenter, transform.rotation);

        int _nextTileID = inst.tilesGenerated.Count + 1;

        nextTile.name = "BossTile " + _nextTileID;

        Debug.Log("Boss tile generated from Tile " + parentTile.tileID);

        inst.tilesGenerated.Add(nextTile);

        inst.GenerateNavMesh();
    }

    public void WallCheck()
    {
        // RAYCAST OUT TO CHECK FOR WALLS AND REPLACE.
        RaycastHit hit;

        LayerMask _wall = LayerMask.GetMask("Wall");

        var _thisPositionOffset = new Vector3(transform.position.x, 2, transform.position.z);
        Physics.Raycast(_thisPositionOffset, _castDir, out hit, checkRange, _wall);

        if (hit.collider != null && hit.collider.CompareTag("Wall"))
        {
            _wallToChange = hit.collider.GetComponent<DungeonWall>();
            _wallToChange.SwitchCollider();
            return;
        }

        if (hit.collider != null && hit.collider.CompareTag("Door"))
        {
            hit.collider.GetComponent<DungeonGen>().RemoveFromtile();
            return;
        }
    }

    public void SwitchWall()
    {
        if (!_wallToChange) return;
        _wallToChange.Switch();
    }

    public void SwitchWallCol()
    {
        if (!_wallToChange) return;
        _wallToChange.SwitchCollider();
    }

    private bool checkForGround()
    {
        // RAYCAST OUT TO CHECK FOR TILES
        // *** DO NOT TOUCH MAN IT TOOK ME WAY TOO LONG TO GET THIS TO WORK ***
        RaycastHit hit;

        LayerMask _ground = LayerMask.GetMask("Ground");

        var _thisPositionOffset = new Vector3(transform.position.x, 0, transform.position.z);

        Physics.Raycast(_thisPositionOffset, _castDir, out hit, checkRange, _ground);

        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            collidedObj = hit.collider.gameObject;
            return true;
        }

        return false;
    }

    private bool checkForGroundBoss()
    {
        // RAYCAST OUT TO CHECK FOR TILES
        // *** DO NOT TOUCH MAN IT TOOK ME WAY TOO LONG TO GET THIS TO WORK ***

        RaycastHit hit, hit2, hit3, hit4, hit5, hit6, hit7;

        LayerMask _ground = LayerMask.GetMask("Ground");

        var _thisPositionOffset = new Vector3(transform.position.x, 0, transform.position.z);

        // Change the +30 in check range to however much larger is the boss tile.
        // Source raycast.
        Physics.Raycast(_thisPositionOffset, _castDir, out hit, checkRange + 60, _ground);

        // Gets the direction perpendicular to the cast direction.
        var _castDirPerpendicular = new Vector3(_castDir.z, 0, _castDir.x);

        // Finds where the center of the boss tile should be.

        // True center
        var _bossTileCentre = _thisPositionOffset + (_castDir * (checkRange + 31));
        // Pre center
        var _bossTileCentre1 = _thisPositionOffset + (_castDir * checkRange);
        // Post center
        var _bossTileCentre2 = _thisPositionOffset + (_castDir * (checkRange + 62));

        // Chungen if you're reading this, don't
        // At Pre, True and Post center of the boss tile, cast two raycast at 90 and -90 degrees from each point
        // So that to detect and obstructing tiles in a grid-like pattern.
        //
        // ----- Post -----   Raycast diagram.
        //        |
        // ----- True -----   << This indicates a boss tile.
        //        |
        // ----- Pre  -----   They will all return a true whenever they detect anything that is Ground mask and Ground tag.
        //        |
        //      Source
        //

        Physics.Raycast(_bossTileCentre, _castDirPerpendicular, out hit2, 45, _ground);
        Physics.Raycast(_bossTileCentre1, _castDirPerpendicular, out hit3, 45, _ground);
        Physics.Raycast(_bossTileCentre2, _castDirPerpendicular, out hit4, 45, _ground);
        Physics.Raycast(_bossTileCentre, -_castDirPerpendicular, out hit5, 45, _ground);
        Physics.Raycast(_bossTileCentre1, -_castDirPerpendicular, out hit6, 45, _ground);
        Physics.Raycast(_bossTileCentre2, -_castDirPerpendicular, out hit7, 45, _ground);

        #region // DO NOT OPEN FOR THE LOVE OF GOD.

        // You have been warned.

        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            collidedObj = hit.collider.gameObject;
            return true;
        }

        if (hit2.collider != null && hit2.collider.CompareTag("Ground"))
        {
            collidedObj = hit2.collider.gameObject;
            return true;
        }

        if (hit3.collider != null && hit3.collider.CompareTag("Ground"))
        {
            collidedObj = hit3.collider.gameObject;
            return true;
        }

        if (hit4.collider != null && hit4.collider.CompareTag("Ground"))
        {
            collidedObj = hit4.collider.gameObject;
            return true;
        }

        if (hit5.collider != null && hit5.collider.CompareTag("Ground"))
        {
            collidedObj = hit5.collider.gameObject;
            return true;
        }

        if (hit6.collider != null && hit6.collider.CompareTag("Ground"))
        {
            collidedObj = hit6.collider.gameObject;
            return true;
        }

        if (hit7.collider != null && hit7.collider.CompareTag("Ground"))
        {
            collidedObj = hit7.collider.gameObject;
            return true;
        }

        // This was the only way.

        #endregion

        return false;
    }

    private void OnValidate()
    {
        switch(dir)
        {
            case doorFacing.forward:
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case doorFacing.left:
                transform.eulerAngles = new Vector3(0, -90, 0);
                break;
            case doorFacing.right:
                transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            case doorFacing.back:
                transform.eulerAngles = new Vector3(0, 180, 0);
                break;
        }
    }
    
    public void RemoveFromtile()
    {
        parentTile._dungeonGens.Remove(this);
        gameObject.SetActive(false);
    }
}
