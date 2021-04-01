using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Handler_DateUI : MonoBehaviour
{
    public Text textRealTime;
    public Text textSeconds;
    public Text textMinutes;
    public Text textHours;
    public Text textDays;
    public Text textWeeks;
    public Text textMonths;
    public Text textYears;


    // Update is called once per frame
    void Update()
    {
        textRealTime.text = "Real time - " + DateManager.realTime.ToString();
        textSeconds.text = "Seconds - " + DateManager.timeStamp.ToString();
        textMinutes.text = "Minutes - " + DateManager.minutes.ToString();
        textHours.text = "Hours - " + DateManager.hours.ToString();
        textDays.text = "Days - " + DateManager.days.ToString();
        textWeeks.text = "Weeks - " + DateManager.weeks.ToString();
        textMonths.text = "Months - " + DateManager.months.ToString();
        textYears.text = "Years - " + DateManager.years.ToString();
    }
}

