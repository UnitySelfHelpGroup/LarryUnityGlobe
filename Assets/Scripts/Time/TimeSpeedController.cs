using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSpeedController : MonoBehaviour
{
    [Header("Game Speeds")]
    public float speed1 = 1;
    public float speed2 = 3;
    public float speed3 = 5;
    public float speedDebug = 100;
    [Header("Game Speed Keys")]
    public KeyCode pauseKey = KeyCode.Space;
    public KeyCode speed1Key = KeyCode.Alpha1;
    public KeyCode speed2Key = KeyCode.Alpha2;
    public KeyCode speed3Key = KeyCode.Alpha3;
    public KeyCode speedDebugKey = KeyCode.Alpha4;
    [HideInInspector]
    public float lastPressed;
    private bool isPausing;


    public void PauseGame()
    {
        if (isPausing)
        {
            if (TimeWizard.i.gameTimeScale == 0)
            {
                TimeWizard.i.gameTimeScale = lastPressed;
                isPausing = false;
            }
            else if (TimeWizard.i.gameTimeScale > 0)
            {
                TimeWizard.i.gameTimeScale = 0;
                isPausing = false;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            isPausing = true;
            PauseGame();
        }
        if (Input.GetKeyDown(speed1Key))
        {
            TimeWizard.i.gameTimeScale = speed1;
            lastPressed = speed1;
        }
       if (Input.GetKeyDown(speed2Key))
        {
            TimeWizard.i.gameTimeScale = speed2;
            lastPressed = speed2;
        }
        if (Input.GetKeyDown(speed3Key))
        {
            TimeWizard.i.gameTimeScale = speed3;
            lastPressed = speed3;
        }
        if (Input.GetKeyDown(speedDebugKey))
        {
            TimeWizard.i.gameTimeScale = speedDebug;
            lastPressed = speedDebug;
        }
        
    }
}
