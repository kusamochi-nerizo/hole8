using Cysharp.Threading.Tasks;
using UnityEngine;

public class EffectSampleMain : MonoBehaviour
{
    [SerializeField] private EffectBase effect;
    async void Start()
    {
        await Play();
    }

    private async UniTask Play()
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.5));
        effect.PlayEffect();
    }
}
