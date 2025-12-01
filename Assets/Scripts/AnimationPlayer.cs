using UnityEngine;
using Cysharp.Threading.Tasks;

public class AnimationPlayer : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animationStateName = "clear";
    [SerializeField] private string defaultStateName = "idle";

    [Header("ParticleSystem")]
    [SerializeField] private ParticleSystem particleSystemPrefab;
    private ParticleSystem spawnedParticleSystem;

    /// <summary>
    /// アニメーションとエフェクトを再生し、終了まで待機
    /// </summary>
    public async UniTask PlayOnce()
    {
        Play();
        await WaitForAnimationComplete();
        Stop();
    }

    /// <summary>
    /// アニメーションとエフェクトを再生
    /// </summary>
    public void Play()
    {
        if (animator != null && !string.IsNullOrEmpty(animationStateName))
        {
            animator.Play(animationStateName, 0, 0f);
        }

        if (particleSystemPrefab != null)
        {
            if (spawnedParticleSystem != null)
            {
                Destroy(spawnedParticleSystem.gameObject);
            }
            spawnedParticleSystem = Instantiate(particleSystemPrefab);
            spawnedParticleSystem.Play();
        }
    }

    /// <summary>
    /// アニメーションとエフェクトを停止
    /// </summary>
    public void Stop()
    {
        if (animator != null)
        {
            animator.Play(defaultStateName, 0, 0f);
        }

        if (spawnedParticleSystem != null)
        {
            Destroy(spawnedParticleSystem.gameObject);
            spawnedParticleSystem = null;
        }
    }

    /// <summary>
    /// アニメーション終了を待機（ループ防止対策あり）
    /// </summary>
    private async UniTask WaitForAnimationComplete()
    {
        if (animator == null) return;

        await UniTask.WaitUntil(() =>
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            return state.IsName(animationStateName) && 
                   state.normalizedTime >= 1f &&
                   !animator.IsInTransition(0);
        });
    }
}
