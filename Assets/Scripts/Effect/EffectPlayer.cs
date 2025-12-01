using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    [SerializeField] private Transform parentTransform; // インスペクタで親を指定

    private IEffect effectInstance;

    /// <summary>
    /// 指定パスのPrefabをResourcesからロードし、親を指定して生成して演出再生
    /// </summary>
    /// <param name="effectPrefabPath">Resourcesからの相対パス（拡張子不要）</param>
    public void LoadAndPlayEffect(string effectPrefabPath)
    {
        DestroyAllChildEffects();
        
        GameObject prefab = Resources.Load<GameObject>(effectPrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Prefabが見つかりません: {effectPrefabPath}");
            return;
        }

        // 親を指定してInstantiate（親がnullならルート階層に生成）
        GameObject obj = Instantiate(prefab, parentTransform);

        effectInstance = obj.GetComponent<IEffect>();
        if (effectInstance == null)
        {
            Debug.LogError("IIncongruityEffectがアタッチされていません");
            return;
        }

        effectInstance.PlayEffect();
    }
    
    public void DestroyAllChildEffects()
    {
        // 後ろから削除しないとインデックスがずれるので注意
        for (int i = parentTransform.childCount - 1; i >= 0; i--)
        {
            Transform child = parentTransform.GetChild(i);
            GameObject.Destroy(child.gameObject);
        }
    }
}