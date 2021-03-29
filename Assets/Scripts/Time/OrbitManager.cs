using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OrbitManager : MonoBehaviour
{
    [Header("Celestial Bodies")]
    public GameObject Sun;
    public GameObject Earth;
    [Header("Celestial Translation")]
    public float axialTilt = 23f;
    [Header("Time Values")]
    public int yearTimeInSeconds = 20;
    private Vector3 yearAngle = new Vector3(0.0f, 360.0f, 0.0f);

    //Private Variables
    private float stepAngle;

    private void Awake()
    {
        stepAngle = 360 / ((yearTimeInSeconds*1000) / 1000f);
    }
    void FixedUpdate()
    {
        if (TimeManager.isTimePaused == false)
        {
                Sun.transform.Rotate(yearAngle, stepAngle * Time.fixedDeltaTime);
        }
    }
}
