using UnityEngine;
using UnityEngine.InputSystem;
public class Effect5 : EffectStateWatcher
{
    [SerializeField] private Animator animator;
    [SerializeField] private float speed = 1.0f; // 歩く速さ

    void Update()
    {
        // 前方に移動
        animator.transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // アニメーションのSpeedパラメータを調整
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }
    }
}