using UnityEngine;
using UniRx;
using System;

public class SimpleTimer : MonoBehaviour
{
    public float duration = 9.99f;
    private float startTime;
    private bool isRunning = false;
    private float lastElapsedTime = 0f;

    private Subject<Unit> onTimerStopped = new Subject<Unit>();
    public IObservable<Unit> OnTimerStopped => onTimerStopped;

    private ReactiveProperty<float> remainingTime = new ReactiveProperty<float>();
    public IReadOnlyReactiveProperty<float> RemainingTime => remainingTime;

    public void StartTimer()
    {
        startTime = Time.time;
        isRunning = true;
        remainingTime.Value = duration;
    }

    public void StopTimer()
    {
        isRunning = false;
        // remainingTime.Value = 0f;
        onTimerStopped.OnNext(Unit.Default);
    }

    void Update()
    {
        if (!isRunning) return;

        float timeLeft = Mathf.Max(0f, duration - (Time.time - startTime));
        remainingTime.Value = timeLeft;
        lastElapsedTime = Time.time - startTime;

        if (timeLeft <= 0f)
        {
            isRunning = false;
            onTimerStopped.OnNext(Unit.Default);
        }
    }

    public float GetElapsedTime()
    {
        return lastElapsedTime;
    }

    public float GetRemainingTime()
    {
        return remainingTime.Value;
    }
}