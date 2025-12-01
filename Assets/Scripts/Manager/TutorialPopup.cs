using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks; // UniTaskを使う

public class TutorialPopup : MonoBehaviour
{
    public static TutorialPopup Instance { get; private set; }

    [SerializeField] private TutorialPages[] tutorialPagesArray;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button skipButton;

    private int currentPage = 0;
    private TutorialPages currentTutorial;

    // 非同期完了通知用
    private UniTaskCompletionSource _tcs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        nextButton.onClick.AddListener(OnNextClicked);
        closeButton.onClick.AddListener(OnCloseClicked);
        skipButton.onClick.AddListener(OnSkipClicked);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 非同期でチュートリアルを表示し、終了まで待機できる
    /// </summary>
    public async UniTask ShowTutorialAsync(int tutorialIndex)
    {
        // すでに待機中なら前のをキャンセル
        if (_tcs != null)
        {
            _tcs.TrySetCanceled();
            _tcs = null;
        }
        _tcs = new UniTaskCompletionSource();

        if (tutorialIndex < 0 || tutorialIndex >= tutorialPagesArray.Length)
        {
            Debug.LogWarning("指定されたチュートリアルが存在しません");
            _tcs.TrySetResult();
            return;
        }

        currentTutorial = tutorialPagesArray[tutorialIndex];
        ShowPage(0);
        
        // 猫の鳴き声SEを再生
        SoundManager.Instance.PlaySE(SE.Cat);
        gameObject.SetActive(true);

        // ポップアップが閉じられるまで待機
        await _tcs.Task;

        // 待機終了後に非表示（念のため）
        gameObject.SetActive(false);
    }

    private void ShowPage(int page)
    {
        currentPage = page;
        messageText.text = currentTutorial.pages[page];

        bool isLastPage = page == currentTutorial.pages.Length - 1;

        nextButton.gameObject.SetActive(!isLastPage);
        closeButton.gameObject.SetActive(isLastPage);
        skipButton.gameObject.SetActive(!isLastPage);
    }

    private void OnNextClicked()
    {
        if (currentPage < currentTutorial.pages.Length - 1)
        {
            ShowPage(currentPage + 1);
        }
    }

    private void OnCloseClicked()
    {
        gameObject.SetActive(false);
        _tcs?.TrySetResult();
    }

    private void OnSkipClicked()
    {
        gameObject.SetActive(false);
        _tcs?.TrySetResult();
    }

    // もし従来のShowTutorialも残すなら
    public void ShowTutorial(int tutorialIndex)
    {
        if (tutorialIndex < 0 || tutorialIndex >= tutorialPagesArray.Length)
        {
            Debug.LogWarning("指定されたチュートリアルが存在しません");
            return;
        }
        currentTutorial = tutorialPagesArray[tutorialIndex];
        ShowPage(0);
        // 猫の鳴き声SEを再生
        SoundManager.Instance.PlaySE(SE.Cat);
        gameObject.SetActive(true);
    }
}
