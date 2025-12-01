using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }
    private Image fadeImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateFadeImage();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CreateFadeImage()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        // if (canvas == null)
        // {
            GameObject canvasObj = new GameObject("FadeCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        // }
        DontDestroyOnLoad(canvasObj);
        // ★ここでSorting Orderを明示的に大きくする
        canvas.sortingOrder = 100;

        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvas.transform, false);
        fadeImage = imageObj.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);

        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        fadeImage.raycastTarget = false;
        fadeImage.enabled = false;
    }


    public async UniTask FadeOut(float duration)
    {
        fadeImage.enabled = true;
        var color = fadeImage.color;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            color.a = Mathf.Lerp(0f, 1f, t);
            fadeImage.color = color;
            await UniTask.Yield();
            elapsed += Time.deltaTime;
        }
        color.a = 1f;
        fadeImage.color = color;
    }

    public async UniTask FadeIn(float duration)
    {
        fadeImage.enabled = true;
        var color = fadeImage.color;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            color.a = Mathf.Lerp(1f, 0f, t);
            fadeImage.color = color;
            await UniTask.Yield();
            elapsed += Time.deltaTime;
        }
        color.a = 0f;
        fadeImage.color = color;
        fadeImage.enabled = false;
    }
}
