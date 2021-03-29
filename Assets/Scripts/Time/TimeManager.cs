using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    static public bool isTimePaused;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space") && !isTimePaused)
        {
            Time.timeScale = 0.0f;
            Time.fixedDeltaTime = 0.0f;
            isTimePaused = true;
        }
        else if (Input.GetKeyDown("space") && isTimePaused)
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f;
            isTimePaused = false;
            Debug.Log("Time resumes!");
        }
    }
}
