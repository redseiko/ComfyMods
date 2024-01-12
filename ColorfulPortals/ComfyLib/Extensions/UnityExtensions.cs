using UnityEngine;

namespace ComfyLib {
  public static class ColorExtensions {
    public static string GetColorHtmlString(this Color color) {
      return color.a == 1f
          ? ColorUtility.ToHtmlStringRGB(color)
          : ColorUtility.ToHtmlStringRGBA(color);
    }

    public static Color SetAlpha(this Color color, float alpha) {
      color.a = alpha;
      return color;
    }
  }

  public static class UnityExtensions {
    public static T Ref<T>(this T gameObject) where T : UnityEngine.Object {
      return gameObject ? gameObject : default;
    }

    public static bool TryGetComponentInParent<T>(
        this UnityEngine.GameObject gameObject, out T component) where T : UnityEngine.Component {
      component = gameObject.GetComponentInParent<T>();
      return component;
    }
  }
}
