using UnityEngine;
using UniRx;
using System;

public class EffectBase : MonoBehaviour, IEffect
{
    [Header("Incongruity Settings")]
    [SerializeField] protected float incongruityTime = 2.0f;      // 違和感が発生し始める時間
    [SerializeField] protected float incongruityDuration = 1.0f;  // 違和感の持続時間

    [Header("Timer")]
    [SerializeField] protected SimpleTimer timer;
    [Header("違和感解説")]
    [SerializeField] public String description;

    private bool isIncongruityActive = false;

    protected Subject<bool> onIncongruityStateChanged = new Subject<bool>();
    public IObservable<bool> OnIncongruityStateChanged => onIncongruityStateChanged;

    // タイムアウト時に成功かどうかを通知する
    protected Subject<bool> onTimeout = new Subject<bool>();
    public IObservable<bool> OnTimeout => onTimeout;

    public float IncongruityTime => incongruityTime;
    public float IncongruityDuration => incongruityDuration;

    protected virtual void Awake()
    {
        if (timer != null)
        {
            timer.RemainingTime
                .Subscribe(CheckIncongruityState)
                .AddTo(this);

            timer.OnTimerStopped
                .Subscribe(_ =>
                {
                    // 成功判定
                    bool isSuccess = IsSuccessOnTimeout();
                    onTimeout.OnNext(isSuccess);
                })
                .AddTo(this);
        }
        else
        {
            Debug.LogWarning($"{nameof(EffectBase)}: Timerがセットされていません。");
        }
    }
    
    private void CheckIncongruityState(float remainingTime)
    {
        // 持続時間が0=違和感なしの場合は何もしない
        if (incongruityDuration == 0f)
        {
            return;
        }
        float elapsed = timer?.GetElapsedTime() ?? 0f;
        bool newState = elapsed >= incongruityTime && elapsed <= incongruityTime + incongruityDuration;

        if (newState != isIncongruityActive)
        {
            if (isIncongruityActive)
            {
                Debug.Log("違和感発生中");
            }
            else
            {
                Debug.Log("違和感終了");
            }
           
            isIncongruityActive = newState;
            onIncongruityStateChanged.OnNext(isIncongruityActive);
        }
    }

    public string GetDescription()
    {
        return description;
    }

    public virtual bool IsSuccess()
    {
        return isIncongruityActive;
    }

    /// <summary>
    /// タイムアウト時の成功判定
    /// </summary>
    protected virtual bool IsSuccessOnTimeout()
    {
        return incongruityDuration == 0f;
    }

    public virtual void PlayEffect()
    {
        SoundManager.Instance.PlayBGM(BGM.Game);
        timer?.StartTimer();
    }

    public virtual void StopEffect()
    {
        SoundManager.Instance.StopBGM();
        timer?.StopTimer();
    }
}
