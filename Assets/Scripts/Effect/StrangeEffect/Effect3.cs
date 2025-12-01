using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
public class Effect3 : EffectStateWatcher
{
    [SerializeField] private Animator animator;
    
    protected override void OnIncongruityStateChanged()
    {
        if (animator == null)
        {
            Debug.LogWarning("Animatorがアサインされていません。");
            return;
        }

        int faceLayerIndex = animator.GetLayerIndex("face");
        if (faceLayerIndex < 0)
        {
            Debug.LogWarning("faceレイヤーがAnimatorに存在しません。");
            return;
        }

        animator.Play("smile", faceLayerIndex);
    }
}