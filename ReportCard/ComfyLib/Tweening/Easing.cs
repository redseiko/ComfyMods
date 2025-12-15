namespace ComfyLib;

using UnityEngine;

public enum Ease {
  Linear,
  OutQuad,
  InQuad,
  InOutQuad,
  OutBack,
  InBack,
  InOutBack,
  OutElastic,
}

public static class Easing {
  public static float Evaluate(float t, Ease ease) {
    if (t < 0f) {
      return 0f;
    }

    if (t > 1f) {
      return 1f;
    }

    switch (ease) {
      case Ease.Linear:
        return t;

      case Ease.OutQuad:
        return t * (2 - t);

      case Ease.InQuad:
        return t * t;

      case Ease.InOutQuad:
        return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;

      case Ease.OutBack: 
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);

      case Ease.InBack:
        float c2 = 1.70158f;
        float c4 = c2 + 1;
        return c4 * t * t * t - c2 * t * t;

      default:
        return t;
    }
  }
}
