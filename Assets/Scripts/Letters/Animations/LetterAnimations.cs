using System.Collections;
using UnityEngine;

namespace Letters.Animations
{
    public static class LetterAnimations
    {
        public static IEnumerator ReturnTo(RectTransform rt, Vector2 start, Vector2 end, float duration)
        {
            float t = 0f;
            duration = Mathf.Max(0.0001f, duration);

            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float a = Mathf.Clamp01(t / duration);
                a = 1f - Mathf.Pow(1f - a, 3f); // ease-out
                rt.anchoredPosition = Vector2.LerpUnclamped(start, end, a);
                yield return null;
            }

            rt.anchoredPosition = end;
        }

        public static IEnumerator Shake(RectTransform rt, float duration, float amplitude, float frequency)
        {
            Vector2 basePos = rt.anchoredPosition;
            float t = 0f;
            float phase = Random.value * Mathf.PI * 2f;
            Vector2 dir = Vector2.right;

            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float a01 = Mathf.Clamp01(t / Mathf.Max(0.0001f, duration));
                float envelope = 1f - a01;

                float s = Mathf.Sin((t * frequency) + phase);
                Vector2 offset = dir * (s * amplitude * envelope);

                rt.anchoredPosition = basePos + offset;
                yield return null;
            }

            rt.anchoredPosition = basePos;
        }
        
        public static IEnumerator ArcTo(RectTransform rt, Vector2 start, Vector2 end, float duration, float arcHeight, float sideSign)
        {
            float t = 0f;
            duration = Mathf.Max(0.0001f, duration);

            Vector2 mid = (start + end) * 0.5f;
            
            Vector2 control = mid + Vector2.up * arcHeight + Vector2.right * (sideSign * arcHeight * 0.35f);


            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float a = Mathf.Clamp01(t / duration);
                a = 1f - Mathf.Pow(1f - a, 3f);

                Vector2 p0 = start;
                Vector2 p1 = control;
                Vector2 p2 = end;

                Vector2 pos =
                    (1 - a) * (1 - a) * p0 +
                    2 * (1 - a) * a * p1 +
                    a * a * p2;

                rt.anchoredPosition = pos;
                yield return null;
            }

            rt.anchoredPosition = end;
        }
    }
}
