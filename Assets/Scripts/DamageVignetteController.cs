using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageVignetteController : MonoBehaviour
{
    public Image vignetteImage;
    public float flashDuration = 0.3f;
    public float maxAlpha = 0.6f;

    private Coroutine flashRoutine;

    void Start()
    {
        SetAlpha(0f);
    }

    public void Flash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashEffect());
    }

    IEnumerator FlashEffect()
    {
        // 淡入
        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(0f, maxAlpha, t / flashDuration));
            yield return null;
        }

        // 淡出
        t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(maxAlpha, 0f, t / flashDuration));
            yield return null;
        }

        SetAlpha(0f);
    }

    void SetAlpha(float alpha)
    {
        if (vignetteImage == null) return;

        Color c = vignetteImage.color;
        c.a = alpha;
        vignetteImage.color = c;
    }
}
