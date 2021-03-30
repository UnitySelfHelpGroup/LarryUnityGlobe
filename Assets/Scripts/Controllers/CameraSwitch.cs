using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera OrbitCamera;
    public Camera SurfaceCamera;
    public KeyCode SwitchKey;
    public bool camSwitch = false;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(SwitchKey))
        {
            camSwitch = !camSwitch;
            OrbitCamera.gameObject.SetActive(camSwitch);
            SurfaceCamera.gameObject.SetActive(!camSwitch);
        }
    }
}
