using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class Effect8 : EffectStateWatcher
{
    [SerializeField] private Transform charaTransform;
    protected override void OnIncongruityStateChanged()
    {
        charaTransform.DOScale(new Vector3(1.04f, 1.04f, 1.04f), 2.0f); 
    }
}