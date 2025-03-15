namespace ComfyLib;

public static class ColorExtensions {
  public static string GetColorHtmlString(this UnityEngine.Color color) {
    return color.a == 1f
        ? UnityEngine.ColorUtility.ToHtmlStringRGB(color)
        : UnityEngine.ColorUtility.ToHtmlStringRGBA(color);
  }

  public static UnityEngine.Color SetAlpha(this UnityEngine.Color color, float alpha) {
    color.a = alpha;
    return color;
  }
}

public static class ComponentExtensions {
  public static bool TryGetComponentInChildren<T>(
      this UnityEngine.GameObject gameObject, out T component) where T : UnityEngine.Component {
    component = gameObject.GetComponentInChildren<T>();
    return component;
  }

  public static bool TryGetComponentInParent<T>(
      this UnityEngine.GameObject gameObject, out T component) where T : UnityEngine.Component {
    component = gameObject.GetComponentInParent<T>();
    return component;
  }
}


public static class UnityExtensions {
  public static T Ref<T>(this T unityObject) where T : UnityEngine.Object {
    return unityObject ? unityObject : default;
  }
}
