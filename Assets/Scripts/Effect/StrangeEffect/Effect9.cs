using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
public class Effect9 : EffectStateWatcher
{
    [SerializeField] private SimpleTimer simpleTimer;
    [SerializeField] private TextMeshProUGUI timerText;

    private bool isIncongruity = false;
    protected override void Start()
    {
        base.Start();
        // 残り時間が変化するたびにUIを更新
        simpleTimer.RemainingTime
            .Subscribe(time =>
            {
                int intPart = Mathf.FloorToInt(time); // 整数部分
                if (isIncongruity && intPart > 0)
                {
                    float decimalPart = 1-(time - intPart);   // 小数部分
                    timerText.text = $"{intPart+decimalPart:F1}";
                    return;
                }
                timerText.text = $"{time:F1}";
            })
            .AddTo(this);
    }
    protected override void OnIncongruityStateChanged()
    {
        isIncongruity = true;
    }
}