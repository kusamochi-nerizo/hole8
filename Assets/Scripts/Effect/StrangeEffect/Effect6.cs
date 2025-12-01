using UnityEngine;
public class Effect6 : EffectStateWatcher
{
    [SerializeField] private ParticleSystem particle;

    protected override void OnIncongruityStateChanged()
    {
        particle.Play();
    }
}