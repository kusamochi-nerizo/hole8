using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("BGM Clips (enumと順番を揃えて登録)")]
    [SerializeField] private AudioClip[] bgmClips;
    [Header("SE Clips (enumと順番を揃えて登録)")]
    [SerializeField] private AudioClip[] seClips;

    private AudioSource bgmSource;
    private AudioSource seSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            seSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // BGMをenumで再生
    public void PlayBGM(BGM bgm, float volume = 0.3f)
    {
        int index = (int)bgm;
        if (index < 0 || index >= bgmClips.Length) return;
        bgmSource.clip = bgmClips[index];
        bgmSource.volume = volume;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // SEをenumで再生
    public void PlaySE(SE se, float volume = 0.5f)
    {
        int index = (int)se;
        if (index < 0 || index >= seClips.Length) return;
        seSource.PlayOneShot(seClips[index], volume);
    }
}