using UnityEngine;
using UnityEngine.UI;

public class TutorialSampleMain : MonoBehaviour
{
    [SerializeField] private Button tutorialButton; // Inspectorでボタンをアサイン

    void Start()
    {
        // ボタンがInspectorでセットされていればリスナーを追加
        if (tutorialButton != null)
        {
            tutorialButton.onClick.AddListener(ShowTutorialPopup);
        }
        else
        {
            Debug.LogWarning("tutorialButtonがセットされていません。");
        }
    }

    public async void ShowTutorialPopup()
    {
        if (TutorialPopup.Instance != null)
        {
           await TutorialPopup.Instance.ShowTutorialAsync(0);
           Debug.Log("閉じました");
        }
        else
        {
            Debug.LogWarning("TutorialPopupがシーンに存在しません。");
        }
    }
}