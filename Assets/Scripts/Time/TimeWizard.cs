using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWizard : MonoBehaviour
{
    public static TimeWizard i;

    public float gameTimeScale;

    public float DeltaTime
    {
        get { return Time.deltaTime * gameTimeScale; }
    }
    public float FixedDeltaTime
    {
        get { return Time.fixedDeltaTime * gameTimeScale; }
    }

    void Awake()
    {
        if (i == null) i = this;
        else throw new Exception("There can only be 1 TimeWizard in the scene!");
    }

}