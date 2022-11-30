using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Vector3 camPivot = Vector3.zero;
    public Vector3 camRotation = new Vector3(45, 35, 0);

    public float camSpeed = 5.0f;
    public float camDistance = 5.0f;
    public Vector3 camOffset;

    public GameObject swordsMan;
    public GameObject ranger;

    [HideInInspector]
    public Player.SwordMan _swordMan;
    [HideInInspector]
    public Player.Ranger _ranger;

    private Vector3 target;

    private Vector3 newPos;
    private Camera mainCamera;

    private bool _runAlr;

    private void Start()
    {
        Application.targetFrameRate = 60;
        mainCamera = GetComponent<Camera>();
        _swordMan = swordsMan.GetComponent<Player.SwordMan>();
        _ranger = ranger.GetComponent<Player.Ranger>();
    }
    private void Update()
    {
        // Ends game when both players are dead.
        if(_swordMan.isDead && _ranger.isDead && !_runAlr)
        {
            _runAlr = true;
            GetComponent<EndGame>().End();
        }

        //moves the camera to the target position.
        if(!_swordMan.isDead && !_ranger.isDead)
        {
            target = (swordsMan.transform.position + ranger.transform.position) / 2;
        }

        if (_swordMan.isDead) target = ranger.transform.position;
        if (_ranger.isDead) target = swordsMan.transform.position;

        camPivot = target;
        newPos = camPivot;

        //sets the rotation of the camera.
        transform.eulerAngles = camRotation;


        if (mainCamera.orthographic)
        {
            newPos += -transform.forward * camDistance * 4F;
            newPos += camOffset;
            mainCamera.orthographicSize = camDistance;
        }

        //moves the camera to the new position which takes in a offset as well
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * camSpeed);

    }
}