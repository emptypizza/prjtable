using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private Vector3 originalPos;
    private RectTransform rectTransform;

    private void Awake()
    {
        // UI 요소이므로 RectTransform을 가져옵니다.
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            originalPos = rectTransform.anchoredPosition;
        }
        else
        {
            originalPos = transform.position;
        }
    }

    // 공격받았을 때 흔들리는 효과
    public void PlayShakeEffect()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine(0.5f, 10f));
    }

    // 말할 때 살짝 튀어오르는 효과
    public void PlayPopEffect()
    {
        StopAllCoroutines();
        StartCoroutine(PopCoroutine(0.2f, 1.1f));
    }

    // 흔들림 코루틴
    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            if (rectTransform != null)
                rectTransform.anchoredPosition = (Vector2)originalPos + new Vector2(x, y);
            else
                transform.position = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 원위치 복귀
        if (rectTransform != null) rectTransform.anchoredPosition = originalPos;
        else transform.position = originalPos;
    }

    // 팝업 코루틴
    private IEnumerator PopCoroutine(float duration, float scaleFactor)
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * scaleFactor;

        float halfDuration = duration / 2;
        float elapsed = 0f;

        // 커지기
        while (elapsed < halfDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        // 작아지기
        while (elapsed < halfDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }
}