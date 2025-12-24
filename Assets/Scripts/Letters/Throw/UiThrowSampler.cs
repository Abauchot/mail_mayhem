using System.Collections.Generic;
using UnityEngine;

namespace Letters.Throw
{
    public sealed class UiThrowSampler
    {
        private struct Sample
        {
            public Vector2 screenPos;
            public float time;
        }

        private readonly List<Sample> _samples = new();
        private Sample _grabSample;
        private int _maxSamples;

        public UiThrowSampler(int maxSamples)
        {
            _maxSamples = Mathf.Max(2, maxSamples);
        }

        public void SetMaxSamples(int maxSamples) => _maxSamples = Mathf.Max(2, maxSamples);

        public void Reset()
        {
            _samples.Clear();
            _grabSample = default;
        }

        public void BeginGrab(Vector2 screenPos, float time)
        {
            _samples.Clear();
            _grabSample = new Sample { screenPos = screenPos, time = time };
            AddSample(screenPos, time);
        }

        public void AddSample(Vector2 screenPos, float time)
        {
            _samples.Add(new Sample { screenPos = screenPos, time = time });
            while (_samples.Count > _maxSamples)
                _samples.RemoveAt(0);
        }

        public Vector2 ComputeVelocity(RectTransform parentRect, Canvas parentCanvas, float flickWeight, float throwScale)
        {
            if (parentRect == null || _samples.Count < 2) return Vector2.zero;

            Camera cam = null;
            if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                cam = parentCanvas.worldCamera;

            int n = _samples.Count;
            var a = _samples[Mathf.Max(0, n - 3)];
            var b = _samples[n - 1];

            float dtFlick = Mathf.Max(0.0001f, b.time - a.time);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, a.screenPos, cam, out var aLocal);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, b.screenPos, cam, out var bLocal);
            Vector2 vFlick = (bLocal - aLocal) / dtFlick;

            float dtDrag = Mathf.Max(0.0001f, b.time - _grabSample.time);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, _grabSample.screenPos, cam, out var gLocal);
            Vector2 vDrag = (bLocal - gLocal) / dtDrag;

            return Vector2.Lerp(vDrag, vFlick, Mathf.Clamp01(flickWeight)) * throwScale;
        }
    }
}
