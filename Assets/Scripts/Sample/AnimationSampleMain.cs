using Cysharp.Threading.Tasks;
using UnityEngine;

public class AnimationSampleMain : MonoBehaviour
{
    [SerializeField] private AnimationPlayer animationPlayer;
    async void Start()
    {
        await Play();
    }

    private async UniTask Play()
    {
        await animationPlayer.PlayOnce();
        // await UniTask.Delay(System.TimeSpan.FromSeconds(5));
        // animationPlayer.Stop();
    }
}
