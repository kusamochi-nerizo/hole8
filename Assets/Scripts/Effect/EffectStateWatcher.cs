using UnityEngine;
using TMPro;
using UniRx;
using UnityEngine.Serialization;

public class EffectStateWatcher : MonoBehaviour
{
   private EffectBase effect;

    protected virtual void Start()
    {
        // effectが未設定の場合、自動でGetComponentする
        if (effect == null)
        {
            effect = GetComponent<EffectBase>();
        }

        if (effect != null)
        {
            effect.OnIncongruityStateChanged
                .Subscribe(isIncongruityActive =>
                {
                    if(!isIncongruityActive){
                        return;
                    }
                    OnIncongruityStateChanged();
                })
                .AddTo(this);
        }
        else
        {
            Debug.LogWarning($"{nameof(EffectBase)} が見つかりませんでした。");
        }
    }
    
    protected virtual void OnIncongruityStateChanged()
    {
        // 派生クラスで必要に応じて
    }
}