using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateManager : MonoBehaviour
{
    //Defines the length of a day in seconds
    public static float dayLength = 12;
    //Defines the length of a year in days
    public static float yearLength = 365.25f;
    //Defines the seconds in a day, and the days in a week
    public static int secondsPerDay = 86400;
    public static int daysInWeek = 7;


    [HideInInspector]
    public static int currentYear;
    public static int currentMonth;
    public static int currentDay;
    
    public string dateFormatDMY = ($"{currentDay}.{currentMonth}.{currentYear}");
    public string dateFormatMDY = ($"{currentMonth}.{currentDay}.{currentYear}");
    public static double timeStamp;
    public static int minutes;
    public static int hours;
    public static int days;
    public static int weeks;
    public static int months;
    public static int years;
    public static float realTime;

    // Start is called before the first frame update    

    // Update is called once per frame
    void Update()
    {

        realTime = Time.deltaTime;
        timeStamp += TimeWizard.i.DeltaTime*secondsPerDay/dayLength;
        minutes = (int)timeStamp / 60;
        hours = minutes/60;
        days = hours/24;
        weeks = days/daysInWeek;
        months = days/30;
        years = months/12;
    }
}
