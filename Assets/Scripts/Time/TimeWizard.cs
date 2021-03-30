using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWizard : MonoBehaviour
{
    static public bool isTimePaused;

    // Update is called once per frame
    void Update()
    {
        if (isTimePaused == true)
        {
            Time.timeScale = 0.0f;
            Time.fixedDeltaTime = 0.0f;
        }
        if (isTimePaused == false)
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f;
        }
    }

}
