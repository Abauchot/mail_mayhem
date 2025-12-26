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
        
        public static Vector2 GetLocalPointIn(RectTransform destinationSpace, RectTransform target, Canvas canvas)
        {
            if (destinationSpace == null || target == null) return Vector2.zero;

            Camera cam = null;
            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                cam = canvas.worldCamera;
            
            Vector3 worldCenter = target.TransformPoint(target.rect.center);
            
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, worldCenter);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(destinationSpace, screen, cam, out var local);

            return local;
        }
    }
}
