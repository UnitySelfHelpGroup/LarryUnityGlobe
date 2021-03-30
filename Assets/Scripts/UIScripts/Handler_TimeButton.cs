using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class Handler_TimeButton : MonoBehaviour
{
    public void PauseButtonClicked()
    {
        {
            if (TimeWizard.isTimePaused == false)
            {
                TimeWizard.isTimePaused = true;
                Debug.Log("Time should be paused");
            }
            else if (TimeWizard.isTimePaused == true)
            {
                TimeWizard.isTimePaused = false;
                Debug.Log("Time should be running");
            }
        }
    }
}
