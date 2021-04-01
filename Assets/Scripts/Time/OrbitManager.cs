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
    private Vector3 revolution = new Vector3(0.0f, 360.0f, 0.0f);

    //Private Variables
    private float stepAngleYear;
    private float stepAngleDay;


    private void Awake()
    {
        float dayTimeInSeconds = yearTimeInSeconds/365.00f;
        stepAngleYear = 360 / yearTimeInSeconds;
        stepAngleDay = 360.00f / dayTimeInSeconds;

        Debug.Log(yearTimeInSeconds);
        Debug.Log(dayTimeInSeconds);
    }
    void FixedUpdate()
    {
        //if (TimeManager.isTimePaused == false)
        {
                //Sun.transform.Rotate(revolution, stepAngleYear * Time.fixedDeltaTime);
                Earth.transform.Rotate(revolution, stepAngleYear * TimeWizard.i.DeltaTime, Space.World);
                Earth.transform.Rotate(revolution, stepAngleDay * TimeWizard.i.DeltaTime, Space.Self);
        }
    }
}
