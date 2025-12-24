namespace ComfyLib;

using UnityEngine;
using UnityEngine.UI;

public static class Tweener {
  public static Tween TweenMove(this Transform target, Vector3 endValue, float duration) {
    return new Vector3Tween(target, target.position, endValue, duration, x => target.position = x)
        .TweenStart();
  }

  public static Tween TweenMove(this RectTransform target, Vector2 endValue, float duration) {
    return new Vector3Tween(target, target.anchoredPosition, endValue, duration, x => target.anchoredPosition = x)
        .TweenStart();
  }

  public static Tween TweenAnchoredPosition(this RectTransform target, Vector2 endValue, float duration) {
    return new Vector3Tween(target, target.anchoredPosition, endValue, duration, x => target.anchoredPosition = x)
        .TweenStart();
  }

  public static Tween TweenScale(this Transform target, Vector3 endValue, float duration) {
    return new Vector3Tween(target, target.localScale, endValue, duration, x => target.localScale = x)
        .TweenStart();
  }

  public static Tween TweenScale(this Transform target, float endValue, float duration) {
    return new Vector3Tween(target, target.localScale, Vector3.one * endValue, duration, x => target.localScale = x)
        .TweenStart();
  }

  public static Tween TweenFade(this CanvasGroup target, float endValue, float duration) {
    return new FloatTween(target, target.alpha, endValue, duration, x => target.alpha = x)
        .TweenStart();
  }
  
  public static Tween TweenFade(this Image target, float endValue, float duration) {
    float startAlpha = target.color.a;

    return new FloatTween(target, startAlpha, endValue, duration, a => {
        Color c = target.color;
        c.a = a;
        target.color = c;
    }).TweenStart();
  }

  public static Tween TweenStart(this Tween tween) {
    TweenManager.Add(tween);
    return tween;
  }

  public static void TweenKill(this Component target) {
    TweenManager.KillTweens(target);
    TweenManager.KillTweens(target.gameObject);
    TweenManager.KillTweens(target.transform);
    if(target is Graphic g) TweenManager.KillTweens(g.rectTransform);
  }
}
