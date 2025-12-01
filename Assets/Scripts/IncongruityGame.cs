using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class IncongruityGame : MonoBehaviour
{
    [SerializeField] private Button detectButton;
    [SerializeField] private Button nextStageButton;
    [SerializeField] private Button gameOverButton;
    [SerializeField] private EffectFactory effectFactory;
    [SerializeField] private int maxStageCount = 8;
    [SerializeField] private AnimationPlayer startAnimationPlayer;
    [SerializeField] private AnimationPlayer successAnimationPlayer;
    [SerializeField] private AnimationPlayer failedAnimationPlayer;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI descriptionText; // 違和感の内容表示


    private IEffect currentEffect;
    private int stageCount = 1;
    private bool isGameActive = false;
    private readonly CompositeDisposable effectDisposable = new CompositeDisposable();

    private void Awake()
    {
        detectButton.onClick.AddListener(() => OnDetectButtonPressed().Forget());
        nextStageButton.onClick.AddListener(() => OnNextStageButtonPressed().Forget());
        gameOverButton.onClick.AddListener(OnGameOverButtonClicked);
        detectButton.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
        stageText.gameObject.SetActive(false);
        HideNextStageButton();
        HideGameOverButton();
        UpdateStageText(); // ★初期化時のみ
    }

    private async void Start()
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.1));
        if (TutorialPopup.Instance != null)
        {
            await TutorialPopup.Instance.ShowTutorialAsync(0);
        }
        await StartPerformance();
    }

    public async UniTask StartPerformance()
    {
        descriptionText.gameObject.SetActive(false);
        detectButton.gameObject.SetActive(false);
        HideNextStageButton();
        HideGameOverButton();

        await FadeManager.Instance.FadeOut(0.5f);
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.1));
        stageText.gameObject.SetActive(true);
        DestroyCurrentEffect();

        if (stageCount == 1)
        {
            currentEffect = effectFactory.CreateEffectByIndex(0);
        }
        else
        {
            currentEffect = effectFactory.CreateRandomEffect();
        }
       
        effectDisposable.Clear();

        currentEffect.OnTimeout
            .Subscribe(isSuccess => OnTimeout(isSuccess).Forget())
            .AddTo(effectDisposable);

        await FadeManager.Instance.FadeIn(0.5f);

        if (startAnimationPlayer != null)
        {
            await startAnimationPlayer.PlayOnce();
        }
        ResetGameState();
        currentEffect.PlayEffect();
    }

    private async UniTask OnDetectButtonPressed()
    {
        if (!isGameActive) return;
        EndEffect();

        if (currentEffect.IsSuccess())
        {
            await HandleSuccess();
        }
        else
        {
            await HandleFailure();
        }
    }

    private void EndEffect()
    {
        isGameActive = false;
        currentEffect.StopEffect();
        detectButton.gameObject.SetActive(false);
    }

    private async UniTask OnNextStageButtonPressed()
    {
        stageCount++;
        UpdateStageText();
        successAnimationPlayer.Stop();
        HideNextStageButton();
        await StartPerformance();
    }

    private async UniTask OnTimeout(bool isSuccess)
    {
        if (!isGameActive) return;
        await HandleTimeout(isSuccess);
        isGameActive = false;
    }

    private void DestroyCurrentEffect()
    {
        effectDisposable.Clear();
        if (currentEffect is MonoBehaviour mb)
        {
            Destroy(mb.gameObject);
            currentEffect = null;
        }
    }

    private void ResetGameState()
    {
        isGameActive = true;
        
        detectButton.gameObject.SetActive(true);
        TextMeshProUGUI tmp = detectButton.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = "PUSH!";
        }
    }

    private bool IsGameClear()
    {
        return stageCount >= maxStageCount;
    }

    // --- UI操作の共通化 ---
    private void ShowNextStageButton()
    {
        nextStageButton.gameObject.SetActive(true);
    }

    private void HideNextStageButton()
    {
        nextStageButton.gameObject.SetActive(false);
    }

    private void ShowGameOverButton()
    {
        gameOverButton.gameObject.SetActive(true);
        detectButton.gameObject.SetActive(false);
        HideNextStageButton();
    }

    private void HideGameOverButton()
    {
        gameOverButton.gameObject.SetActive(false);
    }

    // --- ステージ数表示の更新 ---
    private void UpdateStageText()
    {
        if (stageText != null)
        {
            var money = (maxStageCount - stageCount + 1) * 1000;
            stageText.text = $"借金\n{money}万";
        }
    }

    // --- メッセージ表示 ---
    private void ShowMessage(string message)
    {
        Debug.Log(message);
    }

    // --- 成功・失敗・時間切れ・クリア時の処理 ---
    private async UniTask HandleSuccess()
    {
        ShowMessage("成功！");
        await PlaySuccessAnimation();
        if (IsGameClear())
        {
            await HandleGameClear();
        }
        else
        {
            ShowNextStageButton();
        }
    }

    private async UniTask HandleFailure()
    {
        ShowMessage("失敗…");
        SoundManager.Instance.PlaySE(SE.Incorrect);
        if (failedAnimationPlayer != null)
        {
            failedAnimationPlayer.Play();
        }
        await UniTask.Delay(System.TimeSpan.FromSeconds(2));
        if (stageCount == 1)
        {
            await TutorialPopup.Instance.ShowTutorialAsync(1);
            failedAnimationPlayer.Stop();
            await OnNextStageButtonPressed();
        }
        else
        {
            ShowGameOverButton();
        }
    }

    private async UniTask HandleTimeout(bool isSuccess)
    {
        ShowMessage("時間切れ");
        EndEffect();
        if (isSuccess)
        {
            await HandleSuccess();
        }
        else
        {
            await HandleFailure();
        }
    }

    private async UniTask HandleGameClear()
    {
        stageCount++;
        UpdateStageText();
        await UniTask.Delay(System.TimeSpan.FromSeconds(2));
        await TutorialPopup.Instance.ShowTutorialAsync(2);
        await FadeManager.Instance.FadeOut(0.5f);
        SceneManager.LoadScene("Title");
        await FadeManager.Instance.FadeIn(0.5f);
    }

    private async UniTask PlaySuccessAnimation()
    {
        if (successAnimationPlayer != null)
        {
            SoundManager.Instance.PlaySE(SE.Correct);
            await UniTask.Delay(System.TimeSpan.FromSeconds(2));
            SoundManager.Instance.PlaySE(SE.Clear);
            successAnimationPlayer.Play();
            
            // 成功時は違和感の内容を表示
            await UniTask.Delay(System.TimeSpan.FromSeconds(2));
            descriptionText.text = currentEffect.GetDescription();
            descriptionText.gameObject.SetActive(true);

        }
        else
        {
            Debug.LogWarning("successAnimationPlayerがセットされていません。");
        }
    }

    // ゲームオーバーボタンが押されたらタイトルシーンに遷移
    private void OnGameOverButtonClicked()
    {
        SceneManager.LoadScene("Title");
    }
}
