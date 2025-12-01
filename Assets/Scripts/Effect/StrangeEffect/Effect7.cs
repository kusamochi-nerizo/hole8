using System;
using TMPro;
using UnityEngine;
public class Effect7 : EffectStateWatcher
{
    protected override void OnIncongruityStateChanged()
    {
        // DetectButtonタグを持つGameObjectを1つ取得
        GameObject button = GameObject.FindWithTag("DetectButton");
        if (button == null)
        {
            Debug.LogWarning("DetectButtonタグのオブジェクトが見つかりませんでした。");
            return;
        }

        // 子オブジェクトからTextMeshProUGUIコンポーネントを取得（UIの場合）
        TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = "押せ！";
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUIコンポーネントが見つかりませんでした。");
        }
    }
}