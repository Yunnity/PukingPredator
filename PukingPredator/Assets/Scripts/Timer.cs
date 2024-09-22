using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public float duration = 0f;
    private float timeRemaining;

    public EventHandler OnTimerComplete;

    public void Init(float timerDuration)
    {
        duration = timerDuration;
        timeRemaining = duration;
    }


    public void StartTimer()
    {
        StartCoroutine(TimerComplete());
    }


    IEnumerator TimerComplete()
    {
        yield return new WaitForSeconds(duration);

        OnTimerComplete?.Invoke(this, EventArgs.Empty);
    }
}
