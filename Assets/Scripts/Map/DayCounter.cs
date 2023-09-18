using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCounter : MonoBehaviour
{
    public enum Phase
    {
        Day = 1,
        Night = 3,
    }

    private int timeCount = 0;
    public int TimeCount { get { return timeCount; } }

    public int Count { get { return timeCount / 2; } }

    public Phase TimePhase { get { return timeCount % 2 == 0 ? Phase.Day : Phase.Night; } }

    public static DayCounter Instance { get; private set; }

    public event EventHandler DayIncremented;
    public event EventHandler PhaseIncremented;

    private void Awake ()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void IncrementPhase ()
    {
        timeCount++;
        PhaseIncremented?.Invoke(this, new EventArgs());
        if(timeCount % 2  == 0)
        {
            DayIncremented?.Invoke(this, new EventArgs());
        }
    }
}
