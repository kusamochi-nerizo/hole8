using UnityEngine;
using TMPro;
using UniRx;

public class TimeDisplay : MonoBehaviour
{
    [SerializeField] private SimpleTimer simpleTimer;
    [SerializeField] private TextMeshProUGUI timerText;

    void Start()
    {
        // 残り時間が変化するたびにUIを更新
        simpleTimer.RemainingTime
            .Subscribe(time =>
            {
                timerText.text = $"{time:F1}";
            })
            .AddTo(this);
    }
}