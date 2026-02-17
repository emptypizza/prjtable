using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private Vector3 originalPos;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null) originalPos = rectTransform.anchoredPosition;
        else originalPos = transform.position;
    }

    public void PlayShakeEffect()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine(0.5f, 10f));
    }

    public void PlayPopEffect()
    {
        StopAllCoroutines();
        StartCoroutine(PopCoroutine(0.2f, 1.1f));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            if (rectTransform) rectTransform.anchoredPosition = (Vector2)originalPos + new Vector2(x, y);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (rectTransform) rectTransform.anchoredPosition = originalPos;
    }

    private IEnumerator PopCoroutine(float duration, float scaleFactor)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = originalScale * scaleFactor;
        float halfDuration = duration / 2;
        float elapsed = 0f;

        while (elapsed < halfDuration) {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < halfDuration) {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }
}