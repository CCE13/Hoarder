using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class PostProcessing : MonoBehaviour
{
    private DepthOfField _depth;
    private VolumeProfile _volumeProfile;
    // Start is called before the first frame update
    void Start()
    {
        _volumeProfile = GetComponent<Volume>()?.profile;

        if (!_volumeProfile.TryGet(out _depth)) throw new System.NullReferenceException(nameof(_depth));

        PauseMenuController.isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenuController.isPaused)
        {
            _depth.active = true;
        }
        else
        {
            _depth.active = false;
        }
    }
}
