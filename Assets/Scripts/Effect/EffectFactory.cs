using UnityEngine;
using System.Collections.Generic;

public class EffectFactory : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> effectPrefabs;

    [SerializeField]
    private Transform parentTransform;

    // 重複を許可するかどうかのフラグ
    [SerializeField]
    private bool allowDuplicate = false;

    // 被りなしモードのときに使う
    private HashSet<int> usedIndices = new HashSet<int>();

    public IEffect CreateRandomEffect()
    {
        if (effectPrefabs == null || effectPrefabs.Count == 0)
        {
            Debug.LogWarning("effectPrefabsが未設定です");
            return null;
        }

        int prefabIndex;

        if (allowDuplicate)
        {
            // 重複あり: 全てからランダム
            prefabIndex = Random.Range(0, effectPrefabs.Count);
        }
        else
        {
            // 重複なし: 未使用のみ抽選
            List<int> availableIndices = new List<int>();
            for (int i = 0; i < effectPrefabs.Count; i++)
            {
                if (!usedIndices.Contains(i))
                    availableIndices.Add(i);
            }

            if (availableIndices.Count == 0)
            {
                Debug.LogWarning("すべてのエフェクトが生成済みです");
                return null;
            }

            int randomListIndex = Random.Range(0, availableIndices.Count);
            prefabIndex = availableIndices[randomListIndex];
            usedIndices.Add(prefabIndex);
        }

        return CreateEffectByIndex(prefabIndex);
    }

    // 必要に応じてリセット用
    public void ResetUsedEffects()
    {
        usedIndices.Clear();
    }

    /// <summary>
    /// 指定したインデックスのエフェクトをインスタンス生成して返す
    /// </summary>
    public IEffect CreateEffectByIndex(int index)
    {
        if (effectPrefabs == null || effectPrefabs.Count == 0)
        {
            Debug.LogWarning("effectPrefabsが未設定です");
            return null;
        }
        if (index < 0 || index >= effectPrefabs.Count)
        {
            Debug.LogWarning($"指定されたインデックス({index})が範囲外です");
            return null;
        }

        DestroyAllChildren(parentTransform);
        GameObject prefab = effectPrefabs[index];
        GameObject obj = Instantiate(prefab, parentTransform);
        var effect = obj.GetComponentInChildren<IEffect>();
        return effect;
    }
    
    private void DestroyAllChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}