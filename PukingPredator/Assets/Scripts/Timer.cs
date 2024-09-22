using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public float duration = 0f;
    private bool isRunning = false;
    private float timeRemaining;

    public EventHandler OnTimerComplete;

    public void Init(float timerDuration)
    {
        duration = timerDuration;
        timeRemaining = duration;
    }

    void Update()
    {
        if (isRunning)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isRunning = false;
                OnTimerComplete?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void StartTimer()
    {
        isRunning = true;
    }
}
