using System;
using UniRx;

public interface IEffect
{
    void PlayEffect();
    void StopEffect();
    bool IsSuccess();
    string GetDescription();

    IObservable<bool> OnIncongruityStateChanged { get; }
    IObservable<bool> OnTimeout { get; }
}