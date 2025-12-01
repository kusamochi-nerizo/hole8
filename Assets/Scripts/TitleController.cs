using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    // インスペクタでボタンを指定する
    [SerializeField] private Button startButton;

    void  Start()
    {
        // ボタンが指定されていればクリックイベントを登録
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        else
        {
            Debug.LogWarning("startButtonがインスペクタで指定されていません。");
        }
    }

    // ボタンがクリックされた時に呼ばれるメソッド
    private async void OnStartButtonClicked()
    {
        startButton.gameObject.SetActive(false);
        await FadeManager.Instance.FadeOut(0.5f);
        SceneManager.LoadScene("Game"); // シーン名は適宜変更
        await FadeManager.Instance.FadeIn(0.5f);
    }
}