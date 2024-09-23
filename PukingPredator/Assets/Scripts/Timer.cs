using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    /// <summary>
    /// The time assigned at the start.
    /// </summary>
    private float initialDuration;

    /// <summary>
    /// If the timer is currently running
    /// </summary>
    private bool isRunning = false;

    /// <summary>
    /// Triggered when the timer finishes.
    /// </summary>
    public event Action onTimerComplete;

    /// <summary>
    /// The percentage of the timer that has been completed.
    /// </summary>
    public float percentComplete => 1f - timeRemaining/initialDuration;

    /// <summary>
    /// The percentage of the timer left before it is completed.
    /// </summary>
    public float percentRemaining => timeRemaining / initialDuration;

    /// <summary>
    /// The time left on the timer.
    /// </summary>
    public float timeRemaining { get; private set; }



    void Update()
    {
        if (!isRunning) { return; }

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            isRunning = false;
            onTimerComplete?.Invoke();
        }
    }



    /// <summary>
    /// Set up the timer.
    /// </summary>
    /// <param name="duration"></param>
    public void Initialize(float duration)
    {
        initialDuration = duration;
        timeRemaining = duration;
    }

    /// <summary>
    /// Makes the timer start running.
    /// </summary>
    public void StartTimer()
    {
        isRunning = true;
    }
}
