using DG.Tweening;
using UnityEngine;

namespace Letters.Animations
{
    
    public static class UiLetterTweens
    {
        public static Sequence ArcTo(
            RectTransform rt,
            Vector2 start,
            Vector2 end,
            float duration,
            float arcHeight,
            float sideSign,
            Ease ease = Ease.OutCubic)
        {
            rt.anchoredPosition = start;
            
            Vector2 control = (start + end) * 0.5f
                              + Vector2.up * arcHeight
                              + Vector2.right * (sideSign * arcHeight * 0.35f);

            var seq = DOTween.Sequence();
            
            seq.Join(rt.DOAnchorPos(end, duration).SetEase(ease));
            
            seq.Join(
                rt.DOAnchorPos(control, duration * 0.5f)
                    .SetEase(Ease.OutQuad)
                    .SetLoops(2, LoopType.Yoyo)
            );

            return seq;
        }
    }
}