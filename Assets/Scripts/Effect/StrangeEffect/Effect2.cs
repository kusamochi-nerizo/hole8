using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class Effect2 : EffectStateWatcher
{
    [FormerlySerializedAs("timerText")] [SerializeField] private TextMeshProUGUI timerText; 
    
    protected override void OnIncongruityStateChanged()
    {
        timerText.color = Color.red;
    }
}
