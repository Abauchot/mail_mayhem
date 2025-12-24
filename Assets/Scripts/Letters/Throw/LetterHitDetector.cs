using UnityEngine;

namespace Letters.Throw
{
    public sealed class LetterHitDetector
    {
        private readonly Boxes.BoxesRegistry _registry;

        public LetterHitDetector(Boxes.BoxesRegistry registry)
        {
            _registry = registry;
        }

        public Boxes.ServiceBox FindHit(RectTransform letterRt)
        {
            if (_registry == null || letterRt == null) return null;

            Rect letterRect = GetWorldRect(letterRt);

            foreach (var box in _registry.Boxes)
            {
                if (!box) continue;
                Rect boxRect = GetWorldRect(box.RectTransform);
                if (letterRect.Overlaps(boxRect, true))
                    return box;
            }

            return null;
        }

        private static Rect GetWorldRect(RectTransform rt)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            Vector2 min = corners[0];
            Vector2 max = corners[2];
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
    }
}
