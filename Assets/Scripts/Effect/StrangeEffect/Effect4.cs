public class Effect4 : EffectStateWatcher
{
    protected override void OnIncongruityStateChanged()
    {
        SoundManager.Instance.StopBGM();
    }
}